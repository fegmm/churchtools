namespace Fegmm.ChurchTools;

public class ChurchToolsOptions
{
    /// <summary>
    /// The API Token used to authenticate against the ChurchTools instance.
    /// </summary>
    public required string ApiToken { get; set; }

    /// <summary>
    /// The base URL of the ChurchTools instance.
    /// </summary>
    public required string BaseUrl { get; set; }
}
