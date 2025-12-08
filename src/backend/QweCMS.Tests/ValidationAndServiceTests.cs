using Xunit;
using Moq;
using QweCMS.Core.Entities;
using QweCMS.Core.Models;
using QweCMS.Core.Services;

namespace QweCMS.Tests;

/// <summary>
/// Тесты для JsonSchemaValidationService
/// </summary>
public class JsonSchemaValidationServiceTests
{
    private readonly JsonSchemaValidationService _validationService;

    public JsonSchemaValidationServiceTests()
    {
        _validationService = new JsonSchemaValidationService();
    }

    [Fact]
    public void ValidateSchema_WithValidSchema_ReturnsSuccess()
    {
        // Arrange
        var validSchema = new
        {
            schema = "https://json-schema.org/draft/2020-12/schema",
            type = "object",
            title = "Test Schema",
            description = "A test schema",
            properties = new
            {
                name = new
                {
                    type = "string",
                    description = "Name field"
                }
            }
        };

        // Act
        var result = _validationService.ValidateSchema(validSchema);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateSchema_WithNullSchema_ReturnsError()
    {
        // Arrange
        object? nullSchema = null;

        // Act
        var result = _validationService.ValidateSchema(nullSchema!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("SCHEMA_NULL", result.Errors[0].Code);
    }

    [Fact]
    public void ValidateSchema_WithMissingType_ReturnsError()
    {
        // Arrange
        var schemaWithoutType = new
        {
            schema = "https://json-schema.org/draft/2020-12/schema",
            title = "Test Schema"
        };

        // Act
        var result = _validationService.ValidateSchema(schemaWithoutType);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "MISSING_TYPE");
    }

    [Fact]
    public void ValidateSchema_WithMissingTitle_ReturnsWarning()
    {
        // Arrange
        var schemaWithoutTitle = new
        {
            schema = "https://json-schema.org/draft/2020-12/schema",
            type = "object"
        };

        // Act
        var result = _validationService.ValidateSchema(schemaWithoutTitle);

        // Assert
        Assert.Contains(result.Warnings, w => w.Code == "MISSING_TITLE");
    }
}

/// <summary>
/// Тесты для MixinService
/// </summary>
public class MixinServiceTests
{
    private readonly Mock<IMixinRepository> _mockRepository;
    private readonly Mock<IJsonSchemaValidationService> _mockValidationService;
    private readonly MixinService _mixinService;

    public MixinServiceTests()
    {
        _mockRepository = new Mock<IMixinRepository>();
        _mockValidationService = new Mock<IJsonSchemaValidationService>();
        _mixinService = new MixinService(_mockRepository.Object, _mockValidationService.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidEntity_ReturnsSuccess()
    {
        // Arrange
        var entity = new MixinEntity
        {
            Name = "test-mixin",
            DisplayName = "Test Mixin",
            Description = "Test Description",
            Schema = new
            {
                type = "object",
                title = "Test Schema"
            }
        };

        var validationResult = new ValidationResult
        {
            IsValid = true,
            Errors = new List<ValidationError>(),
            Warnings = new List<ValidationWarning>()
        };

        _mockValidationService.Setup(x => x.ValidateSchema(entity.Schema))
            .Returns(validationResult);

        _mockRepository.Setup(x => x.CreateAsync(entity))
            .ReturnsAsync(entity);

        // Act
        var result = await _mixinService.CreateAsync(entity);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(entity.Name, result.Data.Name);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidSchema_ReturnsFailure()
    {
        // Arrange
        var entity = new MixinEntity
        {
            Name = "test-mixin",
            DisplayName = "Test Mixin",
            Schema = new { type = "invalid" }
        };

        var validationResult = new ValidationResult
        {
            IsValid = false,
            Errors = new List<ValidationError>
            {
                new ValidationError
                {
                    Code = "INVALID_TYPE",
                    Message = "Invalid type"
                }
            }
        };

        _mockValidationService.Setup(x => x.ValidateSchema(entity.Schema))
            .Returns(validationResult);

        // Act
        var result = await _mixinService.CreateAsync(entity);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("Invalid JSON Schema", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var id = "test-id";

        _mockRepository.Setup(x => x.DeleteAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _mixinService.DeleteAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        Assert.Equal("Mixin deleted successfully", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ReturnsFailure()
    {
        // Arrange
        var id = "non-existing-id";

        _mockRepository.Setup(x => x.DeleteAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _mixinService.DeleteAsync(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "NOT_FOUND");
    }
}

/// <summary>
/// Тесты для OperationResult
/// </summary>
public class OperationResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = OperationResult<string>.Success(data, "Success message");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(data, result.Data);
        Assert.Equal("Success message", result.Message);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Failure_CreatesFailureResult()
    {
        // Arrange
        var errors = new List<ValidationError>
        {
            new ValidationError { Code = "ERROR1", Message = "Error 1" }
        };

        // Act
        var result = OperationResult<string>.Failure(errors, "Failure message");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Failure message", result.Message);
        Assert.Single(result.Errors);
        Assert.Equal("ERROR1", result.Errors[0].Code);
    }
}