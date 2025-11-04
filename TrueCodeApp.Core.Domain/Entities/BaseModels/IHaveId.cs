namespace TrueCodeApp.Core.Domain.Entities.BaseModels;

/// <summary>
/// Identifier common interface
/// </summary>
public interface IHaveId<T>
{
    /// <summary>
    /// Identifier
    /// </summary>
    T Id { get; set; }
}