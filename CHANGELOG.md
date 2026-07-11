# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-07-11

### ⚠️ Breaking Changes

- **API Target URL Changed**: The API client now targets `fegmm.church.tools` instead of `demo.church.tools`.
  - This means some fields may not be available for other users.
  - Users should use the API carefully or consider creating a fork and adapting the target URL inside enerate-client.ps1`.

- **Major Version Upgrade**: Upgraded all dependencies to their newest major versions.
  - `Microsoft.Kiota.Abstractions` from 1.x to 2.0.0
  - `Microsoft.Kiota.Http.HttpClientLibrary` from 1.x to 2.0.0
  - `Microsoft.Kiota.Serialization.Form` from 1.x to 2.0.0
  - `Microsoft.Kiota.Serialization.Json` from 1.x to 2.0.0
  - `Microsoft.Kiota.Serialization.Multipart` from 1.x to 2.0.0
  - `Microsoft.Kiota.Serialization.Text` from 1.x to 2.0.0
  - `Microsoft.Extensions.*` packages from 10.0.3 to 10.0.9

### 📦 Dependency Upgrades

- All NuGet packages have been updated to their latest major versions.
- Security vulnerabilities addressed in `Microsoft.Kiota.Abstractions` (GHSA-7j59-v9qr-6fq9).
