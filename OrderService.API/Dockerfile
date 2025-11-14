# =============================
# Build stage
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY OrderService.API/OrderService.API.csproj OrderService.API/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/

# Restore dependencies
RUN dotnet restore OrderService.API/OrderService.API.csproj

# Copy the full solution
COPY . .

# Build and publish
WORKDIR /src/OrderService.API
RUN dotnet publish -c Release -o /app/publish

# =============================
# Runtime stage
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy build output
COPY --from=build /app/publish .

# Expose API port
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "OrderService.API.dll"]
