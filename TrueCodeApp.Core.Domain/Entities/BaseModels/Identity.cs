using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrueCodeApp.Core.Domain.Entities.BaseModels;

/// <summary>
/// Identifier
/// </summary>
public abstract class Identity<T> : IHaveId<T>
{
    /// <summary>
    /// Identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; } = default!;
}