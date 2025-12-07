# qweCMS Docker Deployment

## 📁 Структура директорий

```
deployment/
├── docker-compose.yml          # Основной compose файл
├── docker/                     # Docker конфигурации
│   ├── backend/
│   │   └── Dockerfile         # Dockerfile для .NET API
│   ├── frontend/
│   │   └── Dockerfile         # Dockerfile для Angular приложения
│   └── nginx/
│       └── nginx.conf         # Конфигурация Nginx reverse proxy
├── scripts/
│   └── mongo-init.js          # Инициализация MongoDB
├── config/
│   ├── .env                   # Переменные окружения
│   └── .env.example           # Пример переменных окружения
├── docs/
│   └── README.md              # Эта документация
└── .dockerignore              # Исключения для Docker
```

## 🚀 Быстрый старт

1. **Настройка переменных окружения**
   ```bash
   cp deployment/config/.env.example deployment/config/.env
   # Отредактируйте deployment/config/.env при необходимости
   ```

2. **Запуск всех сервисов**
   ```bash
   docker-compose -f deployment/docker-compose.yml up -d
   ```

3. **Проверка статуса**
   ```bash
   docker-compose -f deployment/docker-compose.yml ps
   ```

## 🏗️ Компоненты

### Backend (.NET 8 API)
- **Dockerfile**: `deployment/docker/backend/Dockerfile`
- **Порт**: 5000
- **Особенности**: Multi-stage build, безопасность через non-root user

### Frontend (Angular + Nginx)
- **Dockerfile**: `deployment/docker/frontend/Dockerfile`
- **Порт**: 80
- **Особенности**: Оптимизированная сборка production

### Database (MongoDB 7.0)
- **Порт**: 27017
- **Инициализация**: `deployment/scripts/mongo-init.js`
- **Особенности**: Автоматическое создание коллекций и индексов

### Reverse Proxy (Nginx)
- **Конфигурация**: `deployment/docker/nginx/nginx.conf`
- **Порты**: 8080 (HTTP), 443 (HTTPS)
- **Профиль**: `production`

## ⚙️ Конфигурация

### Переменные окружения

Основные настройки в `deployment/config/.env`:

```bash
# MongoDB
MONGO_ROOT_USERNAME=qweadmin
MONGO_ROOT_PASSWORD=your-secure-password
MONGO_DATABASE=qwecms

# JWT
JWT_SECRET=your-super-secret-jwt-key
JWT_EXPIRATION_MINUTES=60

# Приложение
ASPNETCORE_ENVIRONMENT=Production
API_BASE_URL=http://localhost:5000
```

### Настройки безопасности

Для production:
1. Измените пароли по умолчанию
2. Используйте сильный JWT секрет
3. Настройте HTTPS
4. Ограничьте доступ к MongoDB

## 🔧 Управление

### Запуск
```bash
# Базовый запуск
docker-compose -f deployment/docker-compose.yml up -d

# С пересборкой
docker-compose -f deployment/docker-compose.yml up -d --build

# Production режим
docker-compose -f deployment/docker-compose.yml --profile production up -d
```

### Остановка
```bash
# Остановка
docker-compose -f deployment/docker-compose.yml down

# С удалением volumes
docker-compose -f deployment/docker-compose.yml down -v
```

### Логи
```bash
# Все сервисы
docker-compose -f deployment/docker-compose.yml logs

# Конкретный сервис
docker-compose -f deployment/docker-compose.yml logs backend
```

### Мониторинг
```bash
# Статус контейнеров
docker-compose -f deployment/docker-compose.yml ps

# Использование ресурсов
docker stats
```

## 🗄️ Работа с базой данных

### Подключение
```bash
# Внутри контейнера
docker-compose -f deployment/docker-compose.yml exec mongodb mongosh -u qweadmin -p your-password --authenticationDatabase admin qwecms

# С локальной машины
mongosh mongodb://qweadmin:your-password@localhost:27017/qwecms?authSource=admin
```

### Резервное копирование
```bash
# Бэкап
docker-compose -f deployment/docker-compose.yml exec mongodb mongodump --uri="mongodb://qweadmin:your-password@localhost:27017/qwecms?authSource=admin" --out /backup

# Восстановление
docker-compose -f deployment/docker-compose.yml exec mongodb mongorestore --uri="mongodb://qweadmin:your-password@localhost:27017/qwecms?authSource=admin" /backup/qwecms
```

## 🌐 Production развертывание

### HTTPS настройка
1. Поместите SSL сертификаты в `deployment/docker/nginx/ssl/`
2. Запустите с production профилем:
   ```bash
   docker-compose -f deployment/docker-compose.yml --profile production up -d
   ```

### Внешняя база данных
Измените `ConnectionStrings__MongoDB` в `deployment/config/.env` для использования внешней MongoDB.

### Мониторинг
- Настройте логирование в файлы
- Используйте системы мониторинга
- Настройте алерты

## 🔍 Поиск проблем

### Диагностика
```bash
# Проверка логов
docker-compose -f deployment/docker-compose.yml logs

# Вход в контейнер
docker-compose -f deployment/docker-compose.yml exec backend sh

# Проверка сети
docker network ls
docker network inspect qweCMS_qwecms-network
```

### Частые проблемы
1. **Порты заняты** - измените порты в docker-compose.yml
2. **Недостаточно памяти** - увеличьте RAM или используйте swap
3. **Проблемы с правами** - проверьте права доступа к директориям

### Полная переустановка
```bash
docker-compose -f deployment/docker-compose.yml down -v
docker system prune -a
docker volume prune
docker-compose -f deployment/docker-compose.yml up -d --build
```

## 📚 Дополнительная информация

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [MongoDB Documentation](https://docs.mongodb.com/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)

## 🆘 Поддержка

При проблемах:
1. Проверьте логи: `docker-compose -f deployment/docker-compose.yml logs`
2. Убедитесь, что требования выполнены
3. Попробуйте полную переустановку
4. Создайте issue в репозитории проекта