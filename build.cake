#addin nuget:?package=Cake.Docker&version=1.0.0

var projectFile = "./src/ContosoPets.Api.csproj";
var version = "0.1.0";  // default semantic version
var publishDirectory = "./src/bin/Release/netcoreapp3.1/win-x64/publish/";
var binDebugDirectory = "./src/bin/Debug/";
var binReleaseDirectory = "./src/bin/Release/";
var dockerFilePath = ".";
var target = Argument("Target", "Build");
var packageOutputDirectory = Argument("Package-Output-Directory", "dist");

Task("Build")
    .Does(() => 
    {
        var dockerBuildSettings = new DockerImageBuildSettings
        {
            Tag = new string[] { "contosopets-api" }
        };
        DockerBuild(dockerBuildSettings, dockerFilePath);
    });

Task("Version")
    .Does(() => 
    {
        var branchName = Argument<string>("Branch-Name");
        version = XmlPeek(projectFile, "/Project/PropertyGroup/Version/text()");

        if(branchName != "master")
        {
            var buildNumber = Argument<string>("Build-Number");
            version += $"-alpha-{buildNumber}";
        }

        Information($"Detected version {version}");
    });

Task("Release")
    .Does(() =>
    {
        var nugetApiKey = Argument<string>("API-Key");
        var settings = new NuGetPushSettings
        {
            Source = "https://nuget.pkg.github.com/Roy19/index.json",
            ApiKey = nugetApiKey,
            SkipDuplicate = true
        };
        
        NuGetPush($"{packageOutputDirectory}/ContosoPets.Api.{version}.nupkg", settings);
    });

Task("CI")
    .IsDependentOn("Build");
    //.IsDependentOn("Version")
    //.IsDependentOn("Package")
    //.IsDependentOn("Release");

RunTarget(target);