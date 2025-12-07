# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Копируем package files
COPY package*.json ./
COPY src/frontend/package*.json ./src/frontend/

# Устанавливаем зависимости
RUN npm ci --only=production

# Копируем исходный код
COPY . .

# Собираем Angular приложение
WORKDIR /app/src/frontend
RUN npm run build --prod

# Production stage
FROM nginx:alpine

# Копируем собранные файлы в nginx
COPY --from=build /app/src/frontend/dist/qwe-cms /usr/share/nginx/html

# Копируем конфигурацию nginx
COPY deployment/docker/nginx/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]