using System.Text.Json;

namespace QweCMS.Core.Services;

/// <summary>
/// Сервис для работы с JSON Pointer (RFC 6901)
/// </summary>
public class JsonPointerService
{
    /// <summary>
    /// Разобрать JSON Pointer на токены
    /// </summary>
    /// <param name="pointer">JSON Pointer строка</param>
    /// <returns>Массив токенов пути</returns>
    public string[] ParsePointer(string pointer)
    {
        // Конвертируем наши пути в стандартные JSON Pointer
        if (pointer.StartsWith("$"))
        {
            pointer = pointer.Replace("$", "/").Replace("$.", "/");
        }

        if (string.IsNullOrEmpty(pointer) || pointer == "/")
            return Array.Empty<string>();

        if (!pointer.StartsWith("/"))
            throw new ArgumentException("JSON Pointer must start with '/'", nameof(pointer));

        // Удаляем начальный слэш и разбиваем по слэшам
        var tokens = pointer.Substring(1).Split('/');

        // Декодируем токены согласно RFC 6901
        for (int i = 0; i < tokens.Length; i++)
        {
            tokens[i] = tokens[i]
                .Replace("~1", "/")  // ~1 -> /
                .Replace("~0", "~"); // ~0 -> ~
        }

        return tokens;
    }

    /// <summary>
    /// Создать JSON Pointer из массива токенов
    /// </summary>
    /// <param name="tokens">Массив токенов</param>
    /// <returns>JSON Pointer строка</returns>
    public string CreatePointer(string[] tokens)
    {
        if (tokens == null || tokens.Length == 0)
            return "/";

        var encodedTokens = tokens.Select(token =>
            token.Replace("~", "~0").Replace("/", "~1")
        );

        return "/" + string.Join("/", encodedTokens);
    }

    /// <summary>
    /// Получить значение по JSON Pointer из объекта
    /// </summary>
    /// <param name="root">Корневой JSON объект</param>
    /// <param name="pointer">JSON Pointer</param>
    /// <returns>Значение по пути или null если не найдено</returns>
    public JsonElement? GetValue(JsonElement root, string pointer)
    {
        var tokens = ParsePointer(pointer);
        var current = root;

        foreach (var token in tokens)
        {
            if (current.ValueKind == JsonValueKind.Object)
            {
                if (!current.TryGetProperty(token, out current))
                    return null;
            }
            else if (current.ValueKind == JsonValueKind.Array)
            {
                if (!int.TryParse(token, out int index) || index < 0 || index >= current.GetArrayLength())
                    return null;
                current = current[index];
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    /// <summary>
    /// Установить значение по JSON Pointer в объекте
    /// </summary>
    /// <param name="root">Корневой JSON объект</param>
    /// <param name="pointer">JSON Pointer</param>
    /// <param name="value">Значение для установки</param>
    /// <returns>Обновленный JSON объект</returns>
    public JsonElement SetValue(JsonElement root, string pointer, JsonElement value)
    {
        var tokens = ParsePointer(pointer);
        return SetValueRecursive(root, tokens, 0, value);
    }

    private JsonElement SetValueRecursive(JsonElement current, string[] tokens, int index, JsonElement value)
    {
        if (index == tokens.Length)
            return value;

        var token = tokens[index];

        if (current.ValueKind == JsonValueKind.Object)
        {
            var obj = new Dictionary<string, JsonElement>();
            bool found = false;

            foreach (var property in current.EnumerateObject())
            {
                if (property.Name == token)
                {
                    obj[property.Name] = SetValueRecursive(property.Value, tokens, index + 1, value);
                    found = true;
                }
                else
                {
                    obj[property.Name] = property.Value;
                }
            }

            if (!found)
            {
                // Создаем промежуточные объекты если нужно
                obj[token] = CreateIntermediateValue(tokens, index + 1, value);
            }

            return JsonSerializer.SerializeToElement(obj);
        }
        else if (current.ValueKind == JsonValueKind.Array)
        {
            if (!int.TryParse(token, out int arrayIndex))
                throw new ArgumentException($"Invalid array index: {token}");

            var array = current.EnumerateArray().ToList();

            while (array.Count <= arrayIndex)
                array.Add(JsonDocument.Parse("null").RootElement);

            array[arrayIndex] = SetValueRecursive(array[arrayIndex], tokens, index + 1, value);

            return JsonSerializer.SerializeToElement(array);
        }

        throw new InvalidOperationException($"Cannot set value at path token '{token}' in {current.ValueKind}");
    }

    private JsonElement CreateIntermediateValue(string[] tokens, int index, JsonElement value)
    {
        if (index == tokens.Length)
            return value;

        var token = tokens[index];

        // Если следующий токен - число, создаем массив
        if (int.TryParse(token, out _))
        {
            var array = new List<JsonElement> { CreateIntermediateValue(tokens, index + 1, value) };
            return JsonSerializer.SerializeToElement(array);
        }
        else
        {
            // Иначе создаем объект
            var obj = new Dictionary<string, JsonElement>
            {
                [token] = CreateIntermediateValue(tokens, index + 1, value)
            };
            return JsonSerializer.SerializeToElement(obj);
        }
    }

    /// <summary>
    /// Проверить валидность JSON Pointer
    /// </summary>
    /// <param name="pointer">JSON Pointer для проверки</param>
    /// <returns>True если валиден</returns>
    public bool IsValidPointer(string pointer)
    {
        // Для нашей системы допускаем пути, начинающиеся с "$"
        if (string.IsNullOrEmpty(pointer) || !pointer.StartsWith("$"))
            return false;

        // Конвертируем в стандартный JSON Pointer для проверки
        var jsonPointer = pointer.Replace("$", "/").Replace("$.", "/");

        try
        {
            ParsePointer(jsonPointer);
            return true;
        }
        catch
        {
            return false;
        }
    }
}