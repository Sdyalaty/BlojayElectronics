# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["BlojayElectronics.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish BlojayElectronics.csproj -c Release -o /app/publish   # ← FIXED

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BlojayElectronics.dll"]