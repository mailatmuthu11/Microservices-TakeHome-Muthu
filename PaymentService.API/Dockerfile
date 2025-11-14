# =============================
# Build stage
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY PaymentService.API/PaymentService.API.csproj PaymentService.API/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/

# Restore dependencies
RUN dotnet restore PaymentService.API/PaymentService.API.csproj

# Copy the full solution
COPY . .

# Build and publish
WORKDIR /src/PaymentService.API
RUN dotnet publish -c Release -o /app/publish

# =============================
# Runtime stage
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose API port
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8081
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
