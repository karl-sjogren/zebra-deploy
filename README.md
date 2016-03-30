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
      <stopService name="BackendService" />
      <stopAppPool name="DefaultAppPool" />
      <clean path="X:\zebra-deploy\website">
        <exclude name="images"/>
      </clean>
      <output path="X:\zebra-deploy\website"/>
      <startAppPool name="DefaultAppPool" />
      <startService name="BackendService" />
      <reporters>
        <hipchat room="https://zebra.hipchat.com/v2/room/123456789/notification?auth_token=" success="true" failure="true" />
      </reporters>
    </stripe>
  </stripes>
  <reporters>
    <hipchat room="https://zebra.hipchat.com/v2/room/123456789/notification?auth_token=" success="true" failure="true" />
  </reporters>
</configuration>
```

**BasePath**
The path which will be watched for new files.

**Stripes**
All zebras have some stripes which in this case would be each website you want to deploy.

**Reporters**
Global reporters that will report on all stripes.

### Stripe configuration

**File**
The name of the file to watch for in the base-path.

**StartAppPool/StopAppPool**
Starts or stops the IIS application pool specified by **name**. Great for applications that keep locking files.

**StartWebsite/StopWebsite**
Starts or stops the IIS website specified by **name**. Great if you want a site to fall through to a "Deploying new stuff"-page.

**Clean**
Removes all files from the specified **path**, can exclude files/folders with the **exclude** element.

**Output**
Extracts the contents for the website into the specified **path** overwriting any existing files.

**Reporters**
Stripe specific reporters that will report on all stripes.

### Reporters

Reporters are invoked when a deployment finishes and can be configured to only notify when succeeding or failing.

#### HipChat reporter

Send information to a hipchat room specified by **room**.
