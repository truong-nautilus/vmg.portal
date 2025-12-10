# Use .NET 8.0 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY NetCore.PortalAPI/ServerCore.PortalAPI/*.csproj NetCore.PortalAPI/ServerCore.PortalAPI/
COPY ServerCore.Utilities/*.csproj ServerCore.Utilities/
COPY Netcore.Utils/Netcore.Utils/*.csproj Netcore.Utils/Netcore.Utils/
COPY Netcore.Chat/*.csproj Netcore.Chat/
COPY Netcore.Notification/*.csproj Netcore.Notification/

# Restore dependencies
RUN dotnet restore NetCore.PortalAPI/ServerCore.PortalAPI/ServerCore.PortalAPI.csproj

# Copy all source code
COPY . .

# Build and Publish
WORKDIR /src/NetCore.PortalAPI/ServerCore.PortalAPI
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "ServerCore.PortalAPI.dll"]
