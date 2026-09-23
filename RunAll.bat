@echo off
chcp 65001 > nul
title WMS System Launch
echo Запуск сервера...
start "WMS Server" dotnet run --project "ServerApp/ServerApp.csproj"
timeout /t 3 /nobreak > nul
echo Запуск клієнта...
start "WMS Client" dotnet run --project "Client/ClientApp/ClientApp.csproj" -f net8.0-windows10.0.19041.0
