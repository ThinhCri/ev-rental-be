# Dockerfile cho EV Rental System
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EV_RENTAL_SYSTEM.csproj", "."]
RUN dotnet restore "./EV_RENTAL_SYSTEM.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EV_RENTAL_SYSTEM.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EV_RENTAL_SYSTEM.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EV_RENTAL_SYSTEM.dll"]
