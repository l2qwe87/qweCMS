# Техническая архитектура qweCMS

## Обзор архитектуры

Система построена по принципу headless CMS с динамической схемой контента на основе JSON Schema.

## Core концепции

### 1. Динамическая схема контента
- **JSON Schema** как основа для определения типов контента
- **Композиция схем** через миксины полей вместо наследования
- **Административный интерфейс** для управления схемами и миксинами
- **Валидация** данных на уровне скомпонованной схемы
- **Автоматическая генерация** форм и представлений на основе схемы
- **Гибкое комбинирование** - любая схема может использовать любые миксины

### 2. Хранилище данных
- **MongoDB** как основная база данных
- **Коллекция `schemas`** - хранение JSON Schema схем контента
- **Коллекция `mixins`** - хранение переиспользуемых миксинов полей
- **Динамические коллекции** - для каждой схемы создается коллекция с префиксом `content_` (например, `content_articles`, `content_products`)
- **Связи между документами** - через ссылки на документы из других коллекций
- **Индексация** по полям связей и часто используемым полям для быстрого поиска

## Структура данных

### Миксины (mixins collection)
```json
// Базовый миксин
{
  "_id": "ObjectId",
  "name": "base_content",
  "displayName": "Базовые поля контента",
  "description": "Стандартные поля для любого контента",
  "schema": {
    "type": "object",
    "properties": {
      "title": {
        "type": "string",
        "title": "Заголовок"
      },
      "slug": {
        "type": "string",
        "title": "URL идентификатор"
      },
      "status": {
        "type": "string",
        "enum": ["draft", "published", "archived"],
        "title": "Статус"
      }
    },
    "required": ["title", "slug"]
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Миксин для контента
{
  "_id": "ObjectId",
  "name": "content_fields",
  "displayName": "Поля контента",
  "description": "Поля для текстового контента",
  "schema": {
    "type": "object",
    "properties": {
      "content": {
        "type": "string",
        "format": "markdown",
        "title": "Содержание"
      },
      "excerpt": {
        "type": "string",
        "title": "Краткое описание"
      }
    },
    "required": ["content"]
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Миксин для SEO метаданных
{
  "_id": "ObjectId",
  "name": "seo_metadata",
  "displayName": "SEO метаданные",
  "description": "SEO поля для контента",
  "schema": {
    "type": "object",
    "properties": {
      "title": {
        "type": "string",
        "title": "SEO заголовок"
      },
      "description": {
        "type": "string",
        "title": "SEO описание"
      },
      "keywords": {
        "type": "array",
        "items": {"type": "string"},
        "title": "SEO ключевые слова"
      }
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Миксин для информации об авторе
{
  "_id": "ObjectId",
  "name": "author_info",
  "displayName": "Информация об авторе",
  "description": "Поля для данных автора",
  "schema": {
    "type": "object",
    "properties": {
      "name": {
        "type": "string",
        "title": "Имя автора"
      },
      "bio": {
        "type": "string",
        "title": "Биография"
      },
      "avatar": {
        "type": "string",
        "format": "uri",
        "title": "URL аватара"
      }
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Миксин для новостей
{
  "_id": "ObjectId",
  "name": "news_fields",
  "displayName": "Поля новостей",
  "description": "Специфические поля для новостей",
  "schema": {
    "type": "object",
    "properties": {
      "priority": {
        "type": "integer",
        "minimum": 1,
        "maximum": 10,
        "title": "Приоритет"
      },
      "expiresAt": {
        "type": "string",
        "format": "date-time",
        "title": "Дата истечения"
      }
    },
    "required": ["priority"]
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

### Схема контента (schemas collection)
```json
// Схема статьи
{
  "_id": "ObjectId",
  "name": "article",
  "displayName": "Статья",
  "description": "Обычная статья",
  "collection": "content_articles",
  "mixins": [
    {
      "name": "base_content",
      "path": "$"
    },
    {
      "name": "content_fields", 
      "path": "$"
    },
    {
      "name": "seo_metadata",
      "path": "$.seo"
    },
    {
      "name": "author_info",
      "path": "$.author"
    }
  ],
  "schema": {
    "type": "object",
    "properties": {
      "author": {
        "type": "object",
        "title": "Автор",
        "qwe:reference": {
          "schema": "content_users",
          "displayField": "name"
        }
      }
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Схема новости
{
  "_id": "ObjectId",
  "name": "news",
  "displayName": "Новость",
  "description": "Новостная статья",
  "collection": "content_news",
  "mixins": [
    {
      "name": "base_content",
      "path": "$"
    },
    {
      "name": "content_fields",
      "path": "$"
    },
    {
      "name": "seo_metadata",
      "path": "$.seo"
    },
    {
      "name": "author_info",
      "path": "$.author"
    },
    {
      "name": "news_fields",
      "path": "$.news"
    }
  ],
  "schema": {
    "type": "object",
    "properties": {
      "author": {
        "type": "object",
        "title": "Автор",
        "qwe:reference": {
          "schema": "content_users",
          "displayField": "name"
        }
      }
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Схема страницы
{
  "_id": "ObjectId",
  "name": "page",
  "displayName": "Страница",
  "description": "Статическая страница",
  "collection": "content_pages",
  "mixins": [
    {
      "name": "base_content",
      "path": "$"
    },
    {
      "name": "content_fields",
      "path": "$"
    },
    {
      "name": "seo_metadata",
      "path": "$.seo"
    }
  ],
  "schema": {
    "type": "object",
    "properties": {
      "template": {
        "type": "string",
        "title": "Шаблон страницы"
      }
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

### Контент (динамические коллекции)
```json
// Коллекция "content_articles"
{
  "_id": "ObjectId",
  // Поля из base_content (path: "$")
  "title": "Название статьи",
  "slug": "nazvanie-stati",
  "status": "published",
  
  // Поля из content_fields (path: "$")
  "content": "Содержание статьи в markdown",
  "excerpt": "Краткое описание",
  
  // Поля из seo_metadata (path: "$.seo")
  "seo": {
    "title": "SEO заголовок статьи",
    "description": "SEO описание статьи",
    "keywords": ["статьи", "блог", "программирование"]
  },
  
  // Поля из author_info (path: "$.author")
  "author": {
    "name": "Иван Петров",
    "bio": "Разработчик и автор статей",
    "avatar": "https://example.com/avatar.jpg",
    "$ref": "content_users",
    "$id": "ObjectId"
  },
  
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Коллекция "content_news"
{
  "_id": "ObjectId",
  // Поля из base_content (path: "$")
  "title": "Важная новость",
  "slug": "vazhnaya-novost",
  "status": "published",
  
  // Поля из content_fields (path: "$")
  "content": "Содержание новости в markdown",
  "excerpt": "Краткое описание новости",
  
  // Поля из seo_metadata (path: "$.seo")
  "seo": {
    "title": "SEO заголовок новости",
    "description": "SEO описание новости",
    "keywords": ["новости", "события", "анонсы"]
  },
  
  // Поля из author_info (path: "$.author")
  "author": {
    "name": "Мария Иванова",
    "bio": "Журналист и редактор",
    "avatar": "https://example.com/avatar2.jpg",
    "$ref": "content_users",
    "$id": "ObjectId"
  },
  
  // Поля из news_fields (path: "$.news")
  "news": {
    "priority": 8,
    "expiresAt": "2024-12-31T23:59:59Z"
  },
  
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}

// Коллекция "content_pages"
{
  "_id": "ObjectId",
  // Поля из base_content (path: "$")
  "title": "О компании",
  "slug": "o-kompanii",
  "status": "published",
  
  // Поля из content_fields (path: "$")
  "content": "Содержание страницы о компании",
  "excerpt": "Краткое описание компании",
  
  // Поля из seo_metadata (path: "$.seo")
  "seo": {
    "title": "SEO заголовок страницы О компании",
    "description": "SEO описание страницы О компании",
    "keywords": ["компания", "о нас", "контакты"]
  },
  
  "template": "about",
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

## Компоненты системы

### Backend (.NET / ASP.NET Core)
- **SchemaService** - управление JSON Schema и коллекциями
- **MixinService** - управление миксинами
- **SchemaCompositionService** - компоновка схем из миксинов с поддержкой путей
- **JsonPointerService** - работа с JSON Pointer для вложенных миксинов
- **ContentService** - CRUD операции с контентом в динамических коллекциях
- **ReferenceService** - работа со связями между документами
- **ValidationService** - валидация данных по скомпонованной схеме
- **RenderService** - подготовка данных для фронтенда с разрешением ссылок
- **MongoDB репозитории** - работа с базой данных и динамическими коллекциями

### Frontend (Angular)
- **Schema Module** - административный интерфейс для управления схемами
- **Content Module** - управление контентом
- **Dynamic Form Component** - генерация форм из JSON Schema
- **Dynamic View Component** - отображение контента по схеме
- **Public Module** - публичная часть сайта

## API эндпоинты

### Управление миксинами
- `GET /api/mixins` - список всех миксинов
- `POST /api/mixins` - создание нового миксина
- `GET /api/mixins/{id}` - получение миксина
- `PUT /api/mixins/{id}` - обновление миксина
- `DELETE /api/mixins/{id}` - удаление миксина (с проверкой использования)

### Управление схемами
- `GET /api/schemas` - список всех схем
- `POST /api/schemas` - создание новой схемы
- `GET /api/schemas/{id}` - получение схемы
- `GET /api/schemas/{id}/composed` - получение скомпонованной схемы с миксинами
- `POST /api/schemas/{id}/validate-mixins` - валидация путей миксинов
- `PUT /api/schemas/{id}` - обновление схемы
- `DELETE /api/schemas/{id}` - удаление схемы (с проверкой зависимостей)

### Управление контентом
- `GET /api/content/{schemaName}` - список контента по типу
- `POST /api/content/{schemaName}` - создание контента
- `GET /api/content/{schemaName}/{id}` - получение конкретного контента
- `PUT /api/content/{schemaName}/{id}` - обновление контента
- `DELETE /api/content/{schemaName}/{id}` - удаление контента
- `GET /api/content/{schemaName}/{id}/references` - получение связанных документов

### Публичный API
- `GET /api/public/{schemaName}` - публичный список контента
- `GET /api/public/{schemaName}/{slug}` - публичный просмотр контента

## Технологический стек

### Backend
- **.NET 8** - основная платформа
- **ASP.NET Core Web API** - REST API
- **MongoDB.Driver** - работа с MongoDB
- **Json.Schema.Net** - работа с JSON Schema
- **AutoMapper** - маппинг сущностей
- **Serilog** - логирование

### Frontend
- **Angular 17+** - основной фреймворк
- **Angular Material** - UI компоненты
- **JSON Schema Form** - генерация форм из схем
- **Marked** - рендеринг Markdown
- **RxJS** - реактивное программирование

### База данных
- **MongoDB 7+** - основное хранилище
- **MongoDB Atlas** (опционально) - облачное решение

## Безопасность

### Аутентификация и авторизация
- **JWT токены** для аутентификации
- **Role-based access control** (RBAC)
- **Разделение прав** на чтение/запись схем и контента

### Валидация и безопасность данных
- **JSON Schema валидация** на сервере
- **Sanitization** пользовательского ввода
- **XSS/CSRF защита**
- **Rate limiting** для API

## Производительность

### Оптимизация запросов
- **Индексы MongoDB** по часто используемым полям
- **Пагинация** для списков контента
- **Кеширование** схем в памяти
- **Lazy loading** для связанных данных

### Frontend оптимизация
- **Code splitting** по модулям
- **Tree shaking** для неиспользуемого кода
- **Lazy loading** компонентов
- **Service Worker** для кеширования

## Масштабируемость

### Горизонтальное масштабирование
- **Stateless API** для легкого масштабирования
- **MongoDB sharding** при необходимости
- **CDN** для статических ресурсов

### Расширение функционала
- **Плагинная архитектура** для кастомных полей
- **Webhooks** для интеграций
- **Custom renderers** для специфических типов контента