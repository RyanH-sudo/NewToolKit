# NetToolkit Production Docker Container - MACRS Final Remediation
# Multi-stage build optimized for production deployment

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for dependency restoration
COPY ["NetToolkit-Fixed.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Core/NetToolkit.Core/NetToolkit.Core.csproj", "src/Core/NetToolkit.Core/"]
COPY ["src/UI/NetToolkit.UI/NetToolkit.UI.csproj", "src/UI/NetToolkit.UI/"]
COPY ["src/Modules/NetToolkit.Modules.Education/NetToolkit.Modules.Education.csproj", "src/Modules/NetToolkit.Modules.Education/"]
COPY ["src/Modules/NetToolkit.Modules.PowerShell/NetToolkit.Modules.PowerShell.csproj", "src/Modules/NetToolkit.Modules.PowerShell/"]
COPY ["src/Modules/NetToolkit.Modules.AiOrb/NetToolkit.Modules.AiOrb.csproj", "src/Modules/NetToolkit.Modules.AiOrb/"]
COPY ["src/Modules/NetToolkit.Modules.MicrosoftAdmin/NetToolkit.Modules.MicrosoftAdmin.csproj", "src/Modules/NetToolkit.Modules.MicrosoftAdmin/"]
COPY ["src/Modules/NetToolkit.Modules.SecurityScan/NetToolkit.Modules.SecurityScan.csproj", "src/Modules/NetToolkit.Modules.SecurityScan/"]
COPY ["src/Modules/NetToolkit.Modules.ScannerAndTopography/NetToolkit.Modules.ScannerAndTopography.csproj", "src/Modules/NetToolkit.Modules.ScannerAndTopography/"]
COPY ["src/Modules/NetToolkit.Modules.SshTerminal/NetToolkit.Modules.SshTerminal.csproj", "src/Modules/NetToolkit.Modules.SshTerminal/"]
COPY ["src/Modules/NetToolkit.Modules.UiPolish/NetToolkit.Modules.UiPolish.csproj", "src/Modules/NetToolkit.Modules.UiPolish/"]
COPY ["src/Modules/NetToolkit.Modules.SecurityFinal/NetToolkit.Modules.SecurityFinal.csproj", "src/Modules/NetToolkit.Modules.SecurityFinal/"]

# Restore dependencies with optimized caching
RUN dotnet restore "NetToolkit-Fixed.sln" --verbosity minimal

# Copy all source code
COPY ["src/", "src/"]
COPY ["tests/", "tests/"]

# Build application with optimizations
RUN dotnet build "NetToolkit-Fixed.sln" -c Release -o /app/build --no-restore --verbosity minimal

# Test stage (optional, can be skipped for faster builds)
FROM build AS test
RUN dotnet test "NetToolkit-Fixed.sln" --no-build --configuration Release --verbosity minimal

# Publish stage
FROM build AS publish
RUN dotnet publish "src/UI/NetToolkit.UI/NetToolkit.UI.csproj" -c Release -o /app/publish --no-restore --verbosity minimal

# Runtime stage - optimized for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install required system packages for network operations
RUN apt-get update && apt-get install -y \
    iputils-ping \
    nmap \
    curl \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

# Create non-root user for enhanced security
RUN groupadd --system nettoolkit && useradd --system --group nettoolkit

# Create application directories with proper permissions
RUN mkdir -p /app/logs /app/data /app/config \
    && chown -R nettoolkit:nettoolkit /app

# Copy published application
COPY --from=publish --chown=nettoolkit:nettoolkit /app/publish .

# Switch to non-root user
USER nettoolkit

# Configure application ports
EXPOSE 5000 5001 8080

# Add health check endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Application metadata
LABEL maintainer="NetToolkit Team" \
      version="1.0.0-production" \
      description="Enterprise Network Engineering Platform - MACRS Final Remediation" \
      org.label-schema.name="NetToolkit" \
      org.label-schema.description="Revolutionary network engineering platform with 100% production readiness" \
      org.label-schema.version="1.0.0" \
      org.label-schema.schema-version="1.0"

# Entry point with proper signal handling
ENTRYPOINT ["dotnet", "NetToolkit.UI.dll"]