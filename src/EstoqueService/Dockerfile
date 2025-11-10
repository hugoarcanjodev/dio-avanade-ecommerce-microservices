# Base image para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/EstoqueService/EstoqueService.csproj", "src/EstoqueService/"]
RUN dotnet restore "src/EstoqueService/EstoqueService.csproj"
COPY . .
WORKDIR "/src/src/EstoqueService"
RUN dotnet build "EstoqueService.csproj" -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish "EstoqueService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EstoqueService.dll"]