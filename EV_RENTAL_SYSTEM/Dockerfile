# Sử dụng .NET 8 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Sử dụng .NET 8 Runtime để chạy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 80
EXPOSE 443

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "EV_RENTAL_SYSTEM.dll"]
