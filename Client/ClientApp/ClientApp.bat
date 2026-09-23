@echo off
chcp 65001 > nul
title WMS Client
echo ====================================
echo      ЗАПУСК КЛІЄНТА (ClientApp)
echo ====================================
dotnet run --project "Client/ClientApp/ClientApp.csproj" -f net8.0-windows10.0.19041.0
pause
