# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ApiTalento.Web/ApiTalento.Web.csproj", "ApiTalento.Web/"]
RUN dotnet restore "ApiTalento.Web/ApiTalento.Web.csproj"

# Copy everything else and build
COPY ApiTalento.Web/ ApiTalento.Web/
WORKDIR "/src/ApiTalento.Web"
RUN dotnet build "ApiTalento.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ApiTalento.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5108

# Install fonts for PDF generation
RUN apt-get update && apt-get install -y \
    fontconfig \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiTalento.Web.dll"]
