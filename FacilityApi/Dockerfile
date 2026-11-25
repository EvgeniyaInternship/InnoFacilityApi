FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY FacilityApi.Domain/FacilityApi.Domain.csproj FacilityApi.Domain/
COPY FacilityApi.Application/FacilityApi.Application.csproj FacilityApi.Application/
COPY FacilityApi.Infrastructure/FacilityApi.Infrastructure.csproj FacilityApi.Infrastructure/
COPY FacilityApi.Api/FacilityApi.Api.csproj FacilityApi.Api/
RUN dotnet restore FacilityApi.Api/FacilityApi.Api.csproj
COPY . .
RUN dotnet publish FacilityApi.Api/FacilityApi.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends socat \
    && rm -rf /var/lib/apt/lists/* \
    && printf '%s\n' \
        '#!/bin/sh' \
        'socat TCP-LISTEN:8080,fork,bind=127.0.0.1,reuseaddr TCP:keycloak:8080 &' \
        'exec dotnet FacilityApi.Api.dll' \
        > /entrypoint.sh \
    && chmod +x /entrypoint.sh
USER app
ENV ASPNETCORE_URLS=http://+:5035
EXPOSE 5035
ENTRYPOINT ["/entrypoint.sh"]
