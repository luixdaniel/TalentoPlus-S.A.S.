# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TalentoPlus S.A.S.ll.Web/TalentoPlus S.A.S.ll.Web.csproj", "TalentoPlus S.A.S.ll.Web/"]
RUN dotnet restore "TalentoPlus S.A.S.ll.Web/TalentoPlus S.A.S.ll.Web.csproj"

# Copy everything else and build
COPY ["TalentoPlus S.A.S.ll.Web/", "TalentoPlus S.A.S.ll.Web/"]
WORKDIR "/src/TalentoPlus S.A.S.ll.Web"
RUN dotnet build "TalentoPlus S.A.S.ll.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TalentoPlus S.A.S.ll.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5040

# Install fonts for PDF generation
RUN apt-get update && apt-get install -y \
    fontconfig \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus S.A.S.ll.Web.dll"]
