namespace WorkflowService.DTOs;

public class CreateWorkflowDefinitionRequest
{
    public required string Name { get; set; }
    public required List<StateDto> States { get; set; }
    public required List<ActionDto> Actions { get; set; }
    public string? Description { get; set; }
}

public class StartWorkflowInstanceRequest
{
    public required string DefinitionId { get; set; }
}

public class ExecuteActionRequest
{
    public required string ActionId { get; set; }
}

public class StateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool IsInitial { get; set; }
    public required bool IsFinal { get; set; }
    public required bool Enabled { get; set; }
    public string? Description { get; set; }
}

public class ActionDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool Enabled { get; set; }
    public required List<string> FromStates { get; set; }
    public required string ToState { get; set; }
    public string? Description { get; set; }
}

public class WorkflowDefinitionResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required List<StateDto> States { get; set; }
    public required List<ActionDto> Actions { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
}

public class WorkflowInstanceResponse
{
    public required string Id { get; set; }
    public required string DefinitionId { get; set; }
    public required string CurrentState { get; set; }
    public required List<HistoryEntryDto> History { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class HistoryEntryDto
{
    public required string ActionId { get; set; }
    public required string FromState { get; set; }
    public required string ToState { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public List<string>? ValidationErrors { get; set; }

    public static ApiResponse<T> SuccessResult(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> ErrorResult(string error) => new() { Success = false, Error = error };
    public static ApiResponse<T> ValidationErrorResult(List<string> errors) => new() { Success = false, ValidationErrors = errors };
}
