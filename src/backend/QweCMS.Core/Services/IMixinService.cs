using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Интерфейс репозитория для работы с миксинами в MongoDB
/// </summary>
public interface IMixinRepository
{
    /// <summary>
    /// Получить все миксины
    /// </summary>
    /// <returns>Коллекция всех миксинов</returns>
    Task<IEnumerable<MixinEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>Миксин или null если не найден</returns>
    Task<MixinEntity?> GetByIdAsync(string id);
    
    /// <summary>
    /// Создать новый миксин
    /// </summary>
    /// <param name="entity">Сущность миксина</param>
    /// <returns>Созданный миксин</returns>
    Task<MixinEntity> CreateAsync(MixinEntity entity);
    
    /// <summary>
    /// Обновить существующий миксин
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <param name="entity">Обновленная сущность</param>
    /// <returns>Обновленный миксин</returns>
    Task<MixinEntity> UpdateAsync(string id, MixinEntity entity);
    
    /// <summary>
    /// Удалить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>True если удален, иначе false</returns>
    Task<bool> DeleteAsync(string id);
    
    /// <summary>
    /// Получить миксины с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters);
}

/// <summary>
/// Интерфейс сервиса для бизнес-логики работы с миксинами
/// </summary>
public interface IMixinService
{
    /// <summary>
    /// Получить все миксины
    /// </summary>
    /// <returns>Коллекция всех миксинов</returns>
    Task<IEnumerable<MixinEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>Миксин или null если не найден</returns>
    Task<MixinEntity?> GetByIdAsync(string id);
    
    /// <summary>
    /// Создать новый миксин с валидацией
    /// </summary>
    /// <param name="entity">Сущность миксина</param>
    /// <returns>Созданный миксин</returns>
    Task<MixinEntity> CreateAsync(MixinEntity entity);
    
    /// <summary>
    /// Обновить существующий миксин с валидацией
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <param name="entity">Обновленная сущность</param>
    /// <returns>Обновленный миксин</returns>
    Task<MixinEntity> UpdateAsync(string id, MixinEntity entity);
    
    /// <summary>
    /// Удалить миксин по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор миксина</param>
    /// <returns>True если удален, иначе false</returns>
    Task<bool> DeleteAsync(string id);
    
    /// <summary>
    /// Валидировать JSON Schema
    /// </summary>
    /// <param name="schema">JSON Schema для валидации</param>
    /// <returns>True если валидна, иначе false</returns>
    Task<bool> ValidateSchemaAsync(object schema);
    
    /// <summary>
    /// Получить миксины с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    Task<PagedResult<MixinEntity>> GetPagedAsync(SearchParameters parameters);
}