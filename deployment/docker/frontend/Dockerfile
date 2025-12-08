# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Копируем package files
COPY src/frontend/package*.json ./

# Устанавливаем зависимости
RUN npm ci

# Копируем исходный код
COPY src/frontend/ ./

# Собираем Angular приложение
RUN npm run build --configuration=production

# Production stage
FROM nginx:alpine

# Копируем собранные файлы в nginx
COPY --from=build /app/dist/qwe-cms /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]