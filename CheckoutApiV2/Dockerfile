# Diese Stufe wird verwendet, wenn sie von VS im Schnellmodus ausgeführt wird (Standardeinstellung für Debugkonfiguration).
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Diese Stufe wird zum Erstellen des Dienstprojekts verwendet.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CheckoutApiV2.csproj", "CheckoutApiV2/"]
RUN dotnet restore "CheckoutApiV2/CheckoutApiV2.csproj" -r linux-arm64

WORKDIR "/src/CheckoutApiV2"
COPY . .
WORKDIR "/src/CheckoutApiV2"
RUN dotnet build "./CheckoutApiV2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CheckoutApiV2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Diese Stufe wird in der Produktion oder bei Ausführung von VS im regulären Modus verwendet (Standard, wenn die Debugkonfiguration nicht verwendet wird).
FROM base AS final
WORKDIR /app

# Kopiere das veröffentlichte Projekt aus der vorherigen Phase
COPY --from=publish /app/publish .

# Setze den Einstiegspunkt auf das veröffentlichte Assembly
ENTRYPOINT ["dotnet", "CheckoutApiV2.dll"]
CMD []