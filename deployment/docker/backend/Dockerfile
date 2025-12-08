# Используем multi-stage build для оптимизации размера образа
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["src/backend/QweCMS.Api/QweCMS.Api.csproj", "src/backend/QweCMS.Api/"]
COPY ["src/backend/QweCMS.Core/QweCMS.Core.csproj", "src/backend/QweCMS.Core/"]
COPY ["src/backend/QweCMS.Infrastructure/QweCMS.Infrastructure.csproj", "src/backend/QweCMS.Infrastructure/"]

RUN dotnet restore "src/backend/QweCMS.Api/QweCMS.Api.csproj"

# Копируем исходный код
COPY . .

# Собираем приложение
WORKDIR "/src/src/backend/QweCMS.Api"
RUN dotnet build "QweCMS.Api.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "QweCMS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# Создаем пользователя для безопасности
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "QweCMS.Api.dll"]