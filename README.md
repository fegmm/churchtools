# ChurchTools .NET API Client
This repository contains an automatically generated .NET 10 API Client for ChurchTools.

Once everyday the newest API definition is conditionally pulled from the ChurchTools demo instance, checking via `ETag` headers to ensure there are true upstream improvements. If there are valid changes in the OpenAPI specification, a new Kiota C# client will be automatically generated.
In this event, a new GitHub Action automated release is built, version bumped, and `.nupkg` published to NuGet.org.

## Used technologies
- GitHub Actions for CI/CD Pipeline Tracking
- Microsoft Kiota to generate the C# `.NET 10` client
- PowerShell scripting outlining the generation, file modifications, diffing, and metadata preparation

## Artifacts
- Nuget package: Fegmm.ChurchTools