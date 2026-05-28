# 1. Use the .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the csproj file and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the release build
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Tell ASP.NET Core to look at the PORT environment variable provided by Render
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "jobapptracker.dll"]