using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run <path-to-openapi-file.json> [output-file.json]");
    return;
}

string inputPath = args[0];
string outputPath = args.Length > 1 ? args[1] : inputPath; // Overwrite if no output path provided

if (!File.Exists(inputPath))
{
    Console.WriteLine($"Error: File not found at '{inputPath}'");
    return;
}

try
{
    Console.WriteLine($"Reading OpenAPI spec from: {inputPath}...");
    string jsonString = File.ReadAllText(inputPath);

    // Parse to a mutable DOM using JsonNode
    JsonNode? rootNode = JsonNode.Parse(jsonString, new JsonNodeOptions
    {
        PropertyNameCaseInsensitive = false
    });

    if (rootNode == null)
    {
        Console.WriteLine("Error: Failed to parse JSON content.");
        return;
    }

    Console.WriteLine("Analyzing and transforming 'allOf' schemas...");
    int fixCount = FixAllOfSiblings(rootNode);

    Console.WriteLine($"Transformation complete. Fixed {fixCount} offending schema(s).");

    // Configure output formatting to match typical OpenAPI layout
    var writeOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    Console.WriteLine($"Saving modified OpenAPI spec to: {outputPath}...");
    File.WriteAllText(outputPath, rootNode.ToJsonString(writeOptions));
    Console.WriteLine("Success!");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

/// <summary>
/// Recursively traverses the JSON DOM, targeting objects that contain "allOf"
/// alongside sibling schema definitions ("type", "properties", "required").
/// </summary>
/// <param name="node">The current JSON node being inspected.</param>
/// <returns>The total number of schemas that were transformed.</returns>
static int FixAllOfSiblings(JsonNode? node)
{
    if (node == null) return 0;
    int modifications = 0;

    if (node is JsonObject obj)
    {
        // Copy properties to an array to prevent "Collection was modified" exceptions during recursion
        var properties = obj.ToList();
        foreach (var prop in properties)
        {
            modifications += FixAllOfSiblings(prop.Value);
        }

        // Check if this object contains "allOf" as an array
        if (obj.TryGetPropertyValue("allOf", out var allOfNode) && allOfNode is JsonArray allOfArray)
        {
            // Target specific Kiota-incompatible siblings to move inside 'allOf'
            string[] targetsToMove = { "type", "properties", "required", "additionalProperties" };
            JsonObject? siblingContainer = null;

            foreach (var key in targetsToMove)
            {
                if (obj.TryGetPropertyValue(key, out var siblingValue) && siblingValue != null)
                {
                    if (siblingContainer == null)
                    {
                        siblingContainer = new JsonObject();
                    }

                    // Detach the sibling property from the parent object...
                    obj.Remove(key);

                    // ...and attach it to our new sub-schema container.
                    siblingContainer.Add(key, siblingValue);
                }
            }

            // If we found and harvested any incompatible siblings, push them into 'allOf'
            if (siblingContainer != null)
            {
                allOfArray.Add(siblingContainer);
                modifications++;
            }
        }
    }
    else if (node is JsonArray arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            modifications += FixAllOfSiblings(arr[i]);
        }
    }

    return modifications;
}