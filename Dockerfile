# Use .NET Core 2.2 SDK for build (amd64 platform)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

# Copy project files
COPY NetCore.PortalAPI/ServerCore.PortalAPI/*.csproj NetCore.PortalAPI/ServerCore.PortalAPI/
COPY ServerCore.Utilities/*.csproj ServerCore.Utilities/
RUN dotnet restore NetCore.PortalAPI/ServerCore.PortalAPI/ServerCore.PortalAPI.csproj

# Copy source code
COPY . .
WORKDIR /src/NetCore.PortalAPI/ServerCore.PortalAPI
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime image (amd64 platform)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "ServerCore.PortalAPI.dll"]
