@echo off
chcp 65001 > nul
title WMS Server
echo ====================================
echo      ЗАПУСК СЕРВЕРА (ServerApp)
echo ====================================
dotnet run --project "ServerApp/ServerApp.csproj"
pause
