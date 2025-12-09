# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install dependencies for System.Drawing.Common if needed
RUN apt-get update && apt-get install -y libgdiplus

COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "ServerCore.PortalAPI.dll"]
