$ErrorActionPreference = "Stop"

$OpenApiUrl = "https://demo.church.tools/system/runtime/swagger/openapi.json"
$EtagFile = ".openapi-etag"
$OpenApiFile = "openapi.json"
$OutputFolder = "src/Fegmm.ChurchTools/Generated"
$Namespace = "Fegmm.ChurchTools"
$ClientClassName = "ChurchToolsClient"

Write-Host "Checking for updates to the ChurchTools OpenAPI specification..."

# Perform a HEAD request to get the ETag and Last-Modified headers
$headRequest = Invoke-WebRequest -Uri $OpenApiUrl -Method Head -UseBasicParsing
$currentEtag = $headRequest.Headers["ETag"]

# If no ETag is provided, we can fallback to Last-Modified
if ([string]::IsNullOrEmpty($currentEtag)) {
    $currentEtag = $headRequest.Headers["Last-Modified"]
}

if ([string]::IsNullOrEmpty($currentEtag)) {
    Write-Warning "No ETag or Last-Modified header found in the response. Will download anyway."
} else {
    Write-Host "Server ETag/Last-Modified: $currentEtag"
    if (Test-Path $EtagFile) {
        $savedEtag = Get-Content $EtagFile -Raw
        if ($savedEtag.Trim() -eq $currentEtag.Trim()) {
            Write-Host "The OpenAPI specification has not changed (ETag matches). Skipping generation."
            exit 0
        }
    }
}

Write-Host "Downloading the latest OpenAPI specification..."
Invoke-WebRequest -Uri $OpenApiUrl -OutFile $OpenApiFile -UseBasicParsing

Write-Host "Generating Kiota C# Client..."
# Ensure the output directory is clean if needed, Kiota usually overwrites but it's safe to just run it:
kiota generate -l CSharp -c $ClientClassName -n $Namespace -d $OpenApiFile -o $OutputFolder --clean-output

if ($LASTEXITCODE -ne 0 -and $LASTEXITCODE -ne $null) {
    Write-Error "Kiota generation failed."
    exit $LASTEXITCODE
}

Write-Host "Saving the new ETag..."
if (-not [string]::IsNullOrEmpty($currentEtag)) {
    Set-Content -Path $EtagFile -Value $currentEtag
}

Write-Host "Client generation successful!"
