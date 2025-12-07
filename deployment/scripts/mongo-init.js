// Инициализация базы данных qweCMS
// Этот скрипт выполняется при первом запуске MongoDB контейнера

// Переключаемся на базу данных qwecms
db = db.getSiblingDB('qwecms');

// Создаем пользователя для приложения
db.createUser({
  user: 'qwecms_user',
  pwd: 'qwecms_password',
  roles: [
    {
      role: 'readWrite',
      db: 'qwecms'
    }
  ]
});

// Создаем индексы для коллекции schemas
db.createCollection('schemas');
db.schemas.createIndex({ "name": 1 }, { unique: true });
db.schemas.createIndex({ "createdAt": 1 });
db.schemas.createIndex({ "updatedAt": 1 });

// Создаем индексы для коллекции mixins
db.createCollection('mixins');
db.mixins.createIndex({ "name": 1 }, { unique: true });
db.mixins.createIndex({ "createdAt": 1 });
db.mixins.createIndex({ "updatedAt": 1 });

// Создаем базовые миксины
db.mixins.insertMany([
  {
    name: "base_content",
    displayName: "Базовые поля контента",
    description: "Стандартные поля для любого контента",
    schema: {
      type: "object",
      properties: {
        title: {
          type: "string",
          title: "Заголовок"
        },
        slug: {
          type: "string",
          title: "URL идентификатор"
        },
        status: {
          type: "string",
          enum: ["draft", "published", "archived"],
          title: "Статус",
          default: "draft"
        }
      },
      required: ["title", "slug"]
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    name: "content_fields",
    displayName: "Поля контента",
    description: "Поля для текстового контента",
    schema: {
      type: "object",
      properties: {
        content: {
          type: "string",
          format: "markdown",
          title: "Содержание"
        },
        excerpt: {
          type: "string",
          title: "Краткое описание"
        }
      },
      required: ["content"]
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    name: "seo_metadata",
    displayName: "SEO метаданные",
    description: "SEO поля для контента",
    schema: {
      type: "object",
      properties: {
        title: {
          type: "string",
          title: "SEO заголовок"
        },
        description: {
          type: "string",
          title: "SEO описание"
        },
        keywords: {
          type: "array",
          items: {"type": "string"},
          title: "SEO ключевые слова"
        }
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    name: "author_info",
    displayName: "Информация об авторе",
    description: "Поля для данных автора",
    schema: {
      type: "object",
      properties: {
        name: {
          type: "string",
          title: "Имя автора"
        },
        bio: {
          type: "string",
          title: "Биография"
        },
        avatar: {
          type: "string",
          format: "uri",
          title: "URL аватара"
        }
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  }
]);

// Создаем базовые схемы
db.schemas.insertMany([
  {
    name: "article",
    displayName: "Статья",
    description: "Обычная статья",
    collection: "content_articles",
    mixins: [
      {
        name: "base_content",
        path: "$"
      },
      {
        name: "content_fields",
        path: "$"
      },
      {
        name: "seo_metadata",
        path: "$.seo"
      },
      {
        name: "author_info",
        path: "$.author"
      }
    ],
    schema: {
      type: "object",
      properties: {
        author: {
          type: "object",
          title: "Автор",
          "qwe:reference": {
            schema: "content_users",
            displayField: "name"
          }
        }
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    name: "page",
    displayName: "Страница",
    description: "Статическая страница",
    collection: "content_pages",
    mixins: [
      {
        name: "base_content",
        path: "$"
      },
      {
        name: "content_fields",
        path: "$"
      },
      {
        name: "seo_metadata",
        path: "$.seo"
      }
    ],
    schema: {
      type: "object",
      properties: {
        template: {
          type: "string",
          title: "Шаблон страницы"
        }
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    name: "news",
    displayName: "Новость",
    description: "Новостная статья",
    collection: "content_news",
    mixins: [
      {
        name: "base_content",
        path: "$"
      },
      {
        name: "content_fields",
        path: "$"
      },
      {
        name: "seo_metadata",
        path: "$.seo"
      },
      {
        name: "author_info",
        path: "$.author"
      }
    ],
    schema: {
      type: "object",
      properties: {
        priority: {
          type: "integer",
          minimum: 1,
          maximum: 10,
          title: "Приоритет"
        },
        expiresAt: {
          type: "string",
          format: "date-time",
          title: "Дата истечения"
        }
      }
    },
    createdAt: new Date(),
    updatedAt: new Date()
  }
]);

// Создаем коллекции для контента с индексами
db.createCollection('content_articles');
db.content_articles.createIndex({ "slug": 1 }, { unique: true });
db.content_articles.createIndex({ "status": 1 });
db.content_articles.createIndex({ "createdAt": -1 });
db.content_articles.createIndex({ "author.$id": 1 });

db.createCollection('content_pages');
db.content_pages.createIndex({ "slug": 1 }, { unique: true });
db.content_pages.createIndex({ "status": 1 });
db.content_pages.createIndex({ "createdAt": -1 });

db.createCollection('content_news');
db.content_news.createIndex({ "slug": 1 }, { unique: true });
db.content_news.createIndex({ "status": 1 });
db.content_news.createIndex({ "createdAt": -1 });
db.content_news.createIndex({ "priority": -1 });
db.content_news.createIndex({ "expiresAt": 1 });

print("База данных qweCMS успешно инициализирована");
print("Созданы коллекции: schemas, mixins, content_articles, content_pages, content_news");
print("Добавлены базовые миксины и схемы");