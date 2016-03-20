# zebra-deploy

Provides a simple way to deploy websites from zip files on IIS.

## Installation

Build the service project and copy the output to your selected location. Execute `ZebraDeploy.Service.exe --install` to install it as a service (has to be run with administrative permissions).

## Configuration

Create a file named config.xml (yes xml, live with it) in your install location. Copy the sample configuration below to start with.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <basePath>X:\zebra-deploy\dropzone</basePath>
  <stripes>
    <stripe file="website.zip">
      <stopAppPool name="DefaultAppPool" />
      <clean path="X:\zebra-deploy\website">
        <exclude name="images"/>
      </clean>
      <output path="X:\zebra-deploy\website"/>
      <startAppPool name="DefaultAppPool" />
    </stripe>
  </stripes>
</configuration>
```

**BasePath**
The path which will be watched for new files.

**Stripes**
All zebras have some stripes which in this case would be each website you want to deploy.

### Stripe configuration

**File**
The name of the file to watch for in the base-path.

**StartAppPool/StopAppPool**
Starts or stops the IIS application pool specified by **name**. Great for applications that keep locking files.

**Clean**
Removes all files from the specified **path**, can exclude files/folders with the **exclude** element.

**Output**
Extracts the contents for the website into the specified **path** overwriting any existing files.
