FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY WebApi/*.csproj ./WebApi/
RUN dotnet restore ./WebApi/WebApi.csproj

COPY WebApi/ ./WebApi/
WORKDIR /app/WebApi
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/WebApi/out .

# Create a non-root user
RUN adduser --disabled-password --gecos '' --shell /bin/bash --home /home/appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WebApi.dll"]