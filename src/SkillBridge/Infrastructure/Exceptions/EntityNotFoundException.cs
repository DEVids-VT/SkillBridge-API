using System;

namespace SkillBridge.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} with ID '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public EntityNotFoundException(string entityName, object entityId, string message)
        : base(message)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
