# Test stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test
WORKDIR /src

# Copy all projects
COPY ["ApiTalento.Web/ApiTalento.Web.csproj", "ApiTalento.Web/"]
COPY ["ApiTalento.Tests/ApiTalento.Tests.csproj", "ApiTalento.Tests/"]

# Restore dependencies
RUN dotnet restore "ApiTalento.Tests/ApiTalento.Tests.csproj"

# Copy source code
COPY ApiTalento.Web/ ApiTalento.Web/
COPY ApiTalento.Tests/ ApiTalento.Tests/

# Build and run tests
WORKDIR "/src/ApiTalento.Tests"
RUN dotnet build "ApiTalento.Tests.csproj" -c Release

# Run tests - if they fail, the build will fail
RUN dotnet test "ApiTalento.Tests.csproj" -c Release --no-build --verbosity normal

# This stage is just for running tests
# The actual result will be used by docker-compose to decide if deployment should proceed
