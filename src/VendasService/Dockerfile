# Base image para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/VendasService/VendasService.csproj", "src/VendasService/"]
RUN dotnet restore "src/VendasService/VendasService.csproj"
COPY . .
WORKDIR "/src/src/VendasService"
RUN dotnet build "VendasService.csproj" -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish "VendasService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VendasService.dll"]