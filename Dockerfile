FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

# if there is not change to .csproj then this layer will not be run again
COPY src/*.csproj .
RUN dotnet restore

# copy rest of the files and build them
COPY src/. .
RUN dotnet publish -c release -o /app --no-restore

# finally run the built binary
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ContosoPets.Api.dll"]