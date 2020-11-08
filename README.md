# Deep Secure Threat Removal SDK

This project is a .Net SDK to work with the [Deep Secure Threat Removal API](https://threat-removal.deep-secure.com/).

[![Build Status](https://github.com/baynezy/deep-secure-threat-removal-sdk/workflows/CI%2FCD/badge.svg)](https://github.com/baynezy/deep-secure-threat-removal-sdk/actions?query=workflow%3ACI%2FCD) [![Codacy Badge](https://app.codacy.com/project/badge/Grade/939558c9841e4896a12dea992a514990)](https://www.codacy.com/gh/baynezy/deep-secure-threat-removal-sdk/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=baynezy/deep-secure-threat-removal-sdk&amp;utm_campaign=Badge_Grade)

## Installation

### NuGet

[![NuGet](https://img.shields.io/nuget/v/DeepSecure.ThreatRemoval?style=square)](https://www.nuget.org/packages/DeepSecure.ThreatRemoval/)[![NuGet](https://img.shields.io/nuget/dt/DeepSecure.ThreatRemoval?style=square)](https://www.nuget.org/packages/DeepSecure.ThreatRemoval/)

```posh
dotnet add package DeepSecure.ThreatRemoval
```

## Usage

```csharp
var config = new Config("<url for deep secure instant API>", "Yor API key");
var requester = new Requester(config);
var converter = new ConvertFile(requester);

var path = @"path/to/file.pdf";
var file = File.ReadAllBytes(path);
var response = await converter.Sync(file, MimeType.ImageJpeg);
var convertedFile = response.File;
```

## Contributing

Pull requests are always welcome. Please read the [contributing guidelines](.github/CONTRIBUTING.md).
