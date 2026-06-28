# 1. Etapa de compilación - CAMBIADO A 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos absolutamente todo el código primero
COPY . .

# Eliminamos el archivo .sln dentro del contenedor para evitar líos con los tests
RUN rm -f Conduit.sln

# Restauramos y publicamos usando .NET 9 apuntando directo al proyecto de la API
RUN dotnet restore src/Conduit/Conduit.csproj
RUN dotnet publish src/Conduit/Conduit.csproj -c Release -o /out

# 2. Etapa de ejecución (Fase ligera) - CAMBIADO A 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "Conduit.dll"]