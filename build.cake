var projectFile = "./src/ContosoPets.Api.csproj";
var version = "0.1.0";  // default semantic version
var publishDirectory = "./src/bin/Release/netcoreapp3.1/win-x64/publish/";
var binDebugDirectory = "./src/bin/Debug/";
var binReleaseDirectory = "./src/bin/Release/";
var target = Argument("Target", "Build");
var packageOutputDirectory = Argument("Package-Output-Directory", "dist");

Task("Clean")
    .Does(() => 
    {
        var cleanSettings = new DotNetCoreCleanSettings
        {
            Framework = "netcoreapp3.1",
            Configuration = "Release"
        };
        DotNetCoreClean(projectFile, cleanSettings);

        if(DirectoryExists(binDebugDirectory))
        {
            CleanDirectory(binDebugDirectory);
        }
        if(DirectoryExists(binReleaseDirectory))
        {
            CleanDirectory(binReleaseDirectory);
        }
        if(DirectoryExists(packageOutputDirectory))
        {
            CleanDirectory(packageOutputDirectory);
        }
    });

Task("Compile")
    .Does(() => 
    {
        DotNetCoreRestore();

        var buildSettings = new DotNetCoreBuildSettings
        {
            Framework = "netcoreapp3.1",
            Configuration = "Release",
            NoRestore = true
        };

        DotNetCoreBuild(projectFile, buildSettings);
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

Task("Package")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        var publishSettings = new DotNetCorePublishSettings
        {
            Framework = "netcoreapp3.1",
            Configuration = "Release",
            Runtime = "win-x64"
        };
        DotNetCorePublish(projectFile, publishSettings);

        EnsureDirectoryExists(packageOutputDirectory);

        var nugetPackSettings = new NuGetPackSettings
        {
            Id = "ContosoPets.Api",
            Version = version,
            Title = "ContosoPets.Api",
            Description = "A sample RESTful API using .NET Core",
            Authors = new []{ "Contoso" },
            ProjectUrl = new Uri("https://github.com/Roy19/ContosoPets.Api"),
            Repository = new NuGetRepository
                        {
                            Type = "Git",
                            Url = "https://github.com/Roy19/ContosoPets.Api"
                        },
            Symbols = false,
            Files = new [] {
                new NuSpecContent
                {
                    Source = "*", Target = "bin"
                }
            },
            BasePath = publishDirectory,
            OutputDirectory = $"./{packageOutputDirectory}"
        };

        NuGetPack(nugetPackSettings);
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

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Package");

Task("CI")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Package")
    .IsDependentOn("Release");

RunTarget(target);