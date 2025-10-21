# 📱 Telegram Bot Notification

Веб-приложение для отправки сообщений через Telegram с удобным интерфейсом и поддержкой Docker.

## ✨ Возможности

- 📤 Отправка одиночных сообщений
- 🔄 Отправка множественных сообщений
- ⏰ Планирование отправки сообщений
- 📊 Планирование множественных сообщений с интервалом
- 🎨 Современный веб-интерфейс
- 🐳 Docker поддержка

## 🚀 Быстрый старт с Docker

### Предварительные требования

- Docker Desktop (для Windows)
- Telegram API credentials (api_id и api_hash)

### Настройка Telegram API

1. Перейдите на https://my.telegram.org/
2. Войдите со своим номером телефона
3. Перейдите в "API development tools"
4. Создайте новое приложение и получите `api_id` и `api_hash`
5. Создайте файл `.env` в корне проекта (скопируйте из `.env.example`):

```env
TELEGRAM_API_ID=ваш_api_id
TELEGRAM_API_HASH=ваш_api_hash
TELEGRAM_PHONE=+79991234567
```

### Запуск приложения

```bash
# Сборка и запуск контейнера
docker-compose up -d

# Просмотр логов
docker-compose logs -f

# Остановка
docker-compose down
```

Приложение будет доступно по адресу: http://localhost:5000

## 🖥️ Использование

1. Откройте браузер и перейдите на http://localhost:5000
2. Нажмите кнопку "🔐 Войти в Telegram"
3. Следуйте инструкциям для авторизации (номер телефона и код)
4. Используйте вкладки для выбора нужной функции:
   - **Отправить сообщение** - одиночное сообщение
   - **Несколько сообщений** - множественная отправка
   - **Запланировать** - отложенная отправка
   - **Запланировать несколько** - множественная отложенная отправка

## 🛠️ Локальная разработка (без Docker)

### Требования

- .NET 6.0 SDK
- Visual Studio / VS Code / Rider

### Запуск

```bash
# Восстановление зависимостей
dotnet restore

# Запуск приложения
dotnet run

# Или с указанием URL
dotnet run --urls "http://localhost:5000"
```

## 📁 Структура проекта

```
├── Services/
│   └── TelegramService.cs      # Сервис для работы с Telegram API
├── wwwroot/
│   ├── index.html              # Главная страница
│   ├── styles.css              # Стили
│   └── script.js               # JavaScript логика
├── Program.cs                  # Точка входа и API endpoints
├── Dockerfile                  # Docker конфигурация
├── docker-compose.yml          # Docker Compose конфигурация
└── README.md                   # Документация
```

## 🔧 Конфигурация

Приложение использует WTelegramClient для работы с Telegram. Конфигурация задаётся через переменные окружения в файле `.env`:

```env
TELEGRAM_API_ID=12345678
TELEGRAM_API_HASH=0123456789abcdef0123456789abcdef
TELEGRAM_PHONE=+79991234567
```

Для локальной разработки можно установить переменные в PowerShell:

```powershell
$env:TELEGRAM_API_ID="12345678"
$env:TELEGRAM_API_HASH="0123456789abcdef0123456789abcdef"
$env:TELEGRAM_PHONE="+79991234567"
```

## 🐛 Решение проблем

### Ошибка авторизации
- Убедитесь, что файл `.env` создан и содержит правильные `TELEGRAM_API_ID`, `TELEGRAM_API_HASH` и `TELEGRAM_PHONE`
- Проверьте логи: `docker-compose logs -f`
- При первом запуске потребуется ввод кода подтверждения из Telegram

### Порт занят
Измените порт в `docker-compose.yml`:
```yaml
ports:
  - "8080:80"  # Вместо 5000
```

### Сессия не сохраняется
Убедитесь, что директория `./data` существует и доступна для записи

## 📝 Лицензия

MIT

## 🤝 Контрибьютинг

Pull requests приветствуются!

## 📧 Контакты

При возникновении проблем создайте Issue в репозитории.
