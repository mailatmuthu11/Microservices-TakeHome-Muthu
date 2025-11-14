# =============================
# Build stage
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files separately
COPY NotificationService.API/NotificationService.API.csproj NotificationService.API/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/

# Restore dependencies
RUN dotnet restore NotificationService.API/NotificationService.API.csproj

# Copy the full solution
COPY . .

# Build and publish
WORKDIR /src/NotificationService.API
RUN dotnet publish -c Release -o /app/publish

# =============================
# Runtime stage
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8082

ENV ASPNETCORE_URLS=http://+:8082
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
