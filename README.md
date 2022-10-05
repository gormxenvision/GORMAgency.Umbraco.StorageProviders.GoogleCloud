# Umbraco GCS Provider
This repository contains an Umbraco storage provider that can replace the default physical file storage.

## Installation & Configuration
The Google Cloud Storage provider has an implementation of the Umbraco IFileSystem that connects to an Google Cloud Storage bucket.

### Installation

### Configuration

This provider can be added in the `Startup.cs` file:
```diff
public void ConfigureServices(IServiceCollection services)
{
    services.AddUmbraco(_env, _config)
        .AddBackOffice()
        .AddWebsite()
        .AddComposers()
+       .AddGoogleCloudMediaFileSystem() 
        .Build();
}
```

In `appsettings.json`:
```json
{
  "Umbraco": {
    "Storage": {
      "GoogleCloud": {
        "Media": {
          "ConnectionString": "./your-service-file-location.json",
          "BucketName": "your-bucket-name"
        }
      }
    }
  }
}
```