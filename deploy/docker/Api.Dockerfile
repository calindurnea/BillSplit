FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# Installed icu libs for global cultures (missing in alpine)
RUN apk add icu
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR "/src"
COPY ["Directory.Build.props", ""]
COPY ["BillSplit.Api/BillSplit.Api.csproj", "BillSplit.Api/"]
COPY ["BillSplit.Contracts/BillSplit.Contracts.csproj", "BillSplit.Contracts/"]
COPY ["BillSplit.Domain/BillSplit.Domain.csproj", "BillSplit.Domain/"]
COPY ["BillSplit.Infrastructure/BillSplit.Persistence.csproj", "BillSplit.Infrastructure/"]
COPY ["BillSplit.Infrastructure/BillSplit.Persistence.Caching.csproj", "BillSplit.Infrastructure/"]
COPY ["BillSplit.Services/BillSplit.Services.csproj", "BillSplit.Services/"]
COPY ["BillSplit.Services.Abstractions/BillSplit.Services.Abstractions.csproj", "BillSplit.Services.Abstractions/"]

RUN dotnet restore "BillSplit.Api/BillSplit.Api.csproj"
COPY . .
WORKDIR "/src/BillSplit.Api"
RUN dotnet build "BillSplit.Api.csproj" -c Release -o "/app/build"

FROM build AS publish
WORKDIR "/src/BillSplit.Api"
RUN dotnet publish "BillSplit.Api.csproj" -c Release --no-restore -o "/app/publish"

FROM base AS final
WORKDIR "/app"
COPY --from=publish "/app/publish" .

ENTRYPOINT ["dotnet", "BillSplit.Api.dll"]
