using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repository.Neo4j.Configuration;

/// <summary>
/// Opciones de configuración tipada para Neo4j
/// </summary>
public class Neo4jOptions
{
    public const string SectionName = "Neo4j";

    [Required(ErrorMessage = "Neo4j URI is required")]
    [Url(ErrorMessage = "Neo4j URI must be a valid URL")]
    public string Uri { get; set; } = string.Empty;

    [Required(ErrorMessage = "Neo4j user is required")]
    public string User { get; set; } = string.Empty;

    [Required(ErrorMessage = "Neo4j password is required")]
    public string Password { get; set; } = string.Empty;

    public string? Database { get; set; }

    [Range(1, 1000, ErrorMessage = "MaxConnectionPoolSize must be between 1 and 1000")]
    public int MaxConnectionPoolSize { get; set; } = 100;

    [Range(1, 3600, ErrorMessage = "ConnectionIdleTimeoutSeconds must be between 1 and 3600")]
    public int ConnectionIdleTimeoutSeconds { get; set; } = 60;

    [Range(1, 300, ErrorMessage = "ConnectionAcquisitionTimeoutSeconds must be between 1 and 300")]
    public int ConnectionAcquisitionTimeoutSeconds { get; set; } = 60;

    [Range(1, 3600, ErrorMessage = "MaxConnectionLifetimeSeconds must be between 1 and 3600")]
    public int MaxConnectionLifetimeSeconds { get; set; } = 3600;

    [Range(0, 10, ErrorMessage = "MaxRetryCount must be between 0 and 10")]
    public int MaxRetryCount { get; set; } = 3;

    [Range(100, 30000, ErrorMessage = "RetryDelayMilliseconds must be between 100 and 30000")]
    public int RetryDelayMilliseconds { get; set; } = 1000;

    public bool EnableDriverLogging { get; set; } = false;
}