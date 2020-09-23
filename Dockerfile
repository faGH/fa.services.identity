# Specify base image.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy everything and restore / publish the solution.
COPY . ./
RUN dotnet build ./FrostAura.Services.Identity.Api/FrostAura.Services.Identity.Api.csproj
RUN dotnet build ./FrostAura.Services.Identity.Core.Tests/FrostAura.Services.Identity.Core.Tests.csproj
RUN dotnet build ./FrostAura.Services.Identity.Data.Tests/FrostAura.Services.Identity.Data.Tests.csproj
RUN dotnet publish ./FrostAura.Services.Identity.Api/FrostAura.Services.Identity.Api.csproj -c Release -o /app/out

# Build runtime image off the correct base.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FrostAura.Services.Identity.Api.dll"]
EXPOSE 80