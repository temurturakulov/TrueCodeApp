# TrueCodeApp — тестовое задание

## Состав
- `TrueCodeApp.User` — сервис пользователей (регистрация, логин, JWT)
- `TrueCodeApp.Currency` — сервис валют (курс, избранные валюты)
- `TrueCodeApp.JobManager` — фоновый сервис обновления курсов
- `TrueCodeApp.Migrator` — миграции БД (FluentMigrator + EF)
- `TrueCodeApp.Tests` — unit-тесты для User и Currency

## Как запустить
1. Установить PostgreSQL и указать строку подключения в `appsettings.json` каждого микросервиса
2. Выполнить миграции:
   запустить консольное приложение в папке Core TrueCodeApp.Migrator
3. Далее запускать микросервисы
