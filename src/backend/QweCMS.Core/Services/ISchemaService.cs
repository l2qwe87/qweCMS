using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Services;

/// <summary>
/// Интерфейс репозитория для работы со схемами в MongoDB
/// </summary>
public interface ISchemaRepository
{
    /// <summary>
    /// Получить все схемы
    /// </summary>
    /// <returns>Коллекция всех схем</returns>
    Task<IEnumerable<SchemaEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Схема или null если не найдена</returns>
    Task<SchemaEntity?> GetByIdAsync(string id);
    
    /// <summary>
    /// Создать новую схему
    /// </summary>
    /// <param name="entity">Сущность схемы</param>
    /// <returns>Созданная схема</returns>
    Task<SchemaEntity> CreateAsync(SchemaEntity entity);
    
    /// <summary>
    /// Обновить существующую схему
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <param name="entity">Обновленная сущность</param>
    /// <returns>Обновленная схема</returns>
    Task<SchemaEntity> UpdateAsync(string id, SchemaEntity entity);
    
    /// <summary>
    /// Удалить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>True если удалена, иначе false</returns>
    Task<bool> DeleteAsync(string id);
    
    /// <summary>
    /// Получить схемы с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    Task<PagedResult<SchemaEntity>> GetPagedAsync(SearchParameters parameters);
}

/// <summary>
/// Интерфейс сервиса для бизнес-логики работы со схемами
/// </summary>
public interface ISchemaService
{
    /// <summary>
    /// Получить все схемы
    /// </summary>
    /// <returns>Коллекция всех схем</returns>
    Task<IEnumerable<SchemaEntity>> GetAllAsync();
    
    /// <summary>
    /// Получить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Схема или null если не найдена</returns>
    Task<SchemaEntity?> GetByIdAsync(string id);
    
    /// <summary>
    /// Создать новую схему с валидацией
    /// </summary>
    /// <param name="entity">Сущность схемы</param>
    /// <returns>Результат операции с созданной схемой</returns>
    Task<OperationResult<SchemaEntity>> CreateAsync(SchemaEntity entity);
    
    /// <summary>
    /// Обновить существующую схему с валидацией
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <param name="entity">Обновленная сущность</param>
    /// <returns>Результат операции с обновленной схемой</returns>
    Task<OperationResult<SchemaEntity>> UpdateAsync(string id, SchemaEntity entity);
    
    /// <summary>
    /// Удалить схему по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Результат операции удаления</returns>
    Task<OperationResult<bool>> DeleteAsync(string id);
    
    /// <summary>
    /// Валидировать связи с миксинами
    /// </summary>
    /// <param name="entity">Сущность схемы</param>
    /// <returns>Результат валидации</returns>
    Task<ValidationResult> ValidateMixinsAsync(SchemaEntity entity);
    
    /// <summary>
    /// Получить скомпонованную схему с миксинами
    /// </summary>
    /// <param name="id">Идентификатор схемы</param>
    /// <returns>Скомпонованная схема</returns>
    Task<OperationResult<object>> GetComposedSchemaAsync(string id);
    
    /// <summary>
    /// Получить схемы с пагинацией и поиском
    /// </summary>
    /// <param name="parameters">Параметры поиска и пагинации</param>
    /// <returns>Результат с пагинацией</returns>
    Task<PagedResult<SchemaEntity>> GetPagedAsync(SearchParameters parameters);
}