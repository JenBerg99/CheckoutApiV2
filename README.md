
# CheckoutApiV2

## Übersicht
Dieses Projekt ist die CheckoutApiV2, eine API für die Zahlungsabwicklung. Die API ermöglicht die sichere und effiziente Verarbeitung von Zahlungen und stellt eine Reihe von Endpunkten für die Interaktion mit dem Zahlungssystem bereit.

## Projektstruktur
```
CheckoutApiV2/
├── .dockerignore
├── CheckoutApiV2.sln
├── Terraform-Bootstrap/
├── Terraform-Bootstrap/
├── Tests/
│   ├── ServiceIntegrationTests
├── docker-compose.yml
├── launchSettings.json
├── CheckoutApiV2/
│   ├── Dockerfile
│   ├── Makefile
│   ├── Programm.cs
│   ├── appsettings.Development.json
│   ├── Debug/
│       ├── appsettings.Development.json
│       ├── appsettings.json
│       ├── appsettings.Production.json
│       ├── CheckoutApiV2.deps.json
└── ...
```

## Voraussetzungen
- .NET Core SDK
- Docker (optional, falls Docker verwendet wird)
- AWS CLI (für deployment in die cloud)
- GNU Make (für ausführung von Makefiles)


## Einrichtung in AWS 
1. Repository klonen:
```bash
git clone <repository-url>
```
2. Benutzer in AWS erstellen mit genügend Berechtigung (root oder AdministratorAccess)
3. Anmelden in AWS CLI:
```bash
aws configure
```
4. Ins Verzeichniss von makefile wechseln und deployAll aufrufen:
```bash
make deployAll
```
5. Prüfe output nach app_url - zum Beispiel
```bash
app_url = load-balancer-checkout-83493227.eu-north-1.elb.amazonaws.com
```
5. Die API sollte unter http://load-balancer-checkout-83493227.eu-north-1.elb.amazonaws.com:8080/swagger/index.html erreichbar sein

## Docker unterstützung
1. Repository klonen:
```bash
git clone <repository-url>
```
2. Für Erstellung des Dokerfiles in unterverzeichnis springen:
```bash
cd CheckoutApiV2
```
3. Erstellen eines neuen Images:
```bash
docker build -t checkoutapiv2 -f ./Dockerfile .
```
4. Für Erstellung von Docker Cmpose ins Hauptverzeichnis wo die Compose File liegt:
```bash
docker compose -f "docker-compose.yml" -p composecheckout up -d
```
5. Die API sollte unter http://localhost:8080/swagger/index.html erreichbar sein

## Konfiguration
Die Konfigurationsdateien befinden sich im Ordner `bin/Debug/`:
- `appsettings.json` – Allgemeine Konfiguration
- `appsettings.Development.json` – Entwicklungskonfiguration
- `appsettings.Production.json` – Produktionskonfiguration

## Tests
Um die Tests auszuführen, welchsel in das Verzeichnis 'CheckoutApiV2\Tests\CheckoutApiV2.Test' verwende:
```bash
dotnet test
```
