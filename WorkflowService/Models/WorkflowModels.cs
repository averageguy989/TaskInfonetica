namespace WorkflowService.Models;

public class WorkflowState
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool IsInitial { get; set; }
    public required bool IsFinal { get; set; }
    public required bool Enabled { get; set; }
    public string? Description { get; set; }
}

public class WorkflowAction
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool Enabled { get; set; }
    public required List<string> FromStates { get; set; }
    public required string ToState { get; set; }
    public string? Description { get; set; }
}

public class WorkflowDefinition
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required List<WorkflowState> States { get; set; }
    public required List<WorkflowAction> Actions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
}

public class WorkflowInstance
{
    public required string Id { get; set; }
    public required string DefinitionId { get; set; }
    public required string CurrentState { get; set; }
    public required List<WorkflowHistoryEntry> History { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class WorkflowHistoryEntry
{
    public required string ActionId { get; set; }
    public required string FromState { get; set; }
    public required string ToState { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
