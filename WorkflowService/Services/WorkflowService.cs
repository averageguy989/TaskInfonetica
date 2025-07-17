using WorkflowService.Models;
using WorkflowService.DTOs;

namespace WorkflowService.Services;

public class WorkflowService
{
    private readonly WorkflowStorageService _storage;
    private readonly WorkflowValidationService _validation;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(
        WorkflowStorageService storage,
        WorkflowValidationService validation,
        ILogger<WorkflowService> logger)
    {
        _storage = storage;
        _validation = validation;
        _logger = logger;
    }

    public async Task<ApiResponse<WorkflowDefinitionResponse>> CreateDefinitionAsync(CreateWorkflowDefinitionRequest request)
    {
        try
        {
            var definition = new WorkflowDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                States = request.States.Select(MapToState).ToList(),
                Actions = request.Actions.Select(MapToAction).ToList()
            };

            var validationResult = _validation.ValidateWorkflowDefinition(definition);
            if (!validationResult.IsValid)
            {
                return ApiResponse<WorkflowDefinitionResponse>.ValidationErrorResult(validationResult.Errors);
            }

            var definitionId = await _storage.SaveDefinitionAsync(definition);
            definition.Id = definitionId;

            var response = MapToDefinitionResponse(definition);
            _logger.LogInformation("Created workflow definition: {DefinitionId}", definitionId);

            return ApiResponse<WorkflowDefinitionResponse>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow definition");
            return ApiResponse<WorkflowDefinitionResponse>.ErrorResult("Failed to create workflow definition");
        }
    }

    public async Task<ApiResponse<WorkflowDefinitionResponse>> GetDefinitionAsync(string id)
    {
        try
        {
            var definition = await _storage.GetDefinitionAsync(id);
            if (definition == null)
            {
                return ApiResponse<WorkflowDefinitionResponse>.ErrorResult($"Workflow definition '{id}' not found");
            }

            var response = MapToDefinitionResponse(definition);
            return ApiResponse<WorkflowDefinitionResponse>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definition: {DefinitionId}", id);
            return ApiResponse<WorkflowDefinitionResponse>.ErrorResult("Failed to retrieve workflow definition");
        }
    }

    public async Task<ApiResponse<List<WorkflowDefinitionResponse>>> GetAllDefinitionsAsync()
    {
        try
        {
            var definitions = await _storage.GetAllDefinitionsAsync();
            var responses = definitions.Select(MapToDefinitionResponse).ToList();
            return ApiResponse<List<WorkflowDefinitionResponse>>.SuccessResult(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all workflow definitions");
            return ApiResponse<List<WorkflowDefinitionResponse>>.ErrorResult("Failed to retrieve workflow definitions");
        }
    }

    public async Task<ApiResponse<WorkflowInstanceResponse>> StartInstanceAsync(StartWorkflowInstanceRequest request)
    {
        try
        {
            var definition = await _storage.GetDefinitionAsync(request.DefinitionId);
            if (definition == null)
            {
                return ApiResponse<WorkflowInstanceResponse>.ErrorResult($"Workflow definition '{request.DefinitionId}' not found");
            }

            var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
            if (initialState == null)
            {
                return ApiResponse<WorkflowInstanceResponse>.ErrorResult("Workflow definition has no initial state");
            }

            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = request.DefinitionId,
                CurrentState = initialState.Id,
                History = new List<WorkflowHistoryEntry>()
            };

            var instanceId = await _storage.SaveInstanceAsync(instance);
            instance.Id = instanceId;

            var response = MapToInstanceResponse(instance);
            _logger.LogInformation("Started workflow instance: {InstanceId} from definition: {DefinitionId}", instanceId, request.DefinitionId);

            return ApiResponse<WorkflowInstanceResponse>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting workflow instance for definition: {DefinitionId}", request.DefinitionId);
            return ApiResponse<WorkflowInstanceResponse>.ErrorResult("Failed to start workflow instance");
        }
    }

    public async Task<ApiResponse<WorkflowInstanceResponse>> ExecuteActionAsync(string instanceId, ExecuteActionRequest request)
    {
        try
        {
            var instance = await _storage.GetInstanceAsync(instanceId);
            if (instance == null)
            {
                return ApiResponse<WorkflowInstanceResponse>.ErrorResult($"Workflow instance '{instanceId}' not found");
            }

            var definition = await _storage.GetDefinitionAsync(instance.DefinitionId);
            if (definition == null)
            {
                return ApiResponse<WorkflowInstanceResponse>.ErrorResult($"Workflow definition '{instance.DefinitionId}' not found");
            }

            var validationResult = _validation.ValidateActionExecution(definition, instance, request.ActionId);
            if (!validationResult.IsValid)
            {
                return ApiResponse<WorkflowInstanceResponse>.ValidationErrorResult(validationResult.Errors);
            }

            var action = definition.Actions.First(a => a.Id == request.ActionId);
            var previousState = instance.CurrentState;

            instance.CurrentState = action.ToState;
            instance.History.Add(new WorkflowHistoryEntry
            {
                ActionId = request.ActionId,
                FromState = previousState,
                ToState = action.ToState
            });

            await _storage.SaveInstanceAsync(instance);

            var response = MapToInstanceResponse(instance);
            _logger.LogInformation("Executed action '{ActionId}' on instance '{InstanceId}': {FromState} -> {ToState}", 
                request.ActionId, instanceId, previousState, action.ToState);

            return ApiResponse<WorkflowInstanceResponse>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action '{ActionId}' on instance '{InstanceId}'", request.ActionId, instanceId);
            return ApiResponse<WorkflowInstanceResponse>.ErrorResult("Failed to execute action");
        }
    }

    public async Task<ApiResponse<WorkflowInstanceResponse>> GetInstanceAsync(string id)
    {
        try
        {
            var instance = await _storage.GetInstanceAsync(id);
            if (instance == null)
            {
                return ApiResponse<WorkflowInstanceResponse>.ErrorResult($"Workflow instance '{id}' not found");
            }

            var response = MapToInstanceResponse(instance);
            return ApiResponse<WorkflowInstanceResponse>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instance: {InstanceId}", id);
            return ApiResponse<WorkflowInstanceResponse>.ErrorResult("Failed to retrieve workflow instance");
        }
    }

    public async Task<ApiResponse<List<WorkflowInstanceResponse>>> GetAllInstancesAsync()
    {
        try
        {
            var instances = await _storage.GetAllInstancesAsync();
            var responses = instances.Select(MapToInstanceResponse).ToList();
            return ApiResponse<List<WorkflowInstanceResponse>>.SuccessResult(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all workflow instances");
            return ApiResponse<List<WorkflowInstanceResponse>>.ErrorResult("Failed to retrieve workflow instances");
        }
    }

    private static WorkflowState MapToState(StateDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        IsInitial = dto.IsInitial,
        IsFinal = dto.IsFinal,
        Enabled = dto.Enabled,
        Description = dto.Description
    };

    private static WorkflowAction MapToAction(ActionDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Enabled = dto.Enabled,
        FromStates = dto.FromStates,
        ToState = dto.ToState,
        Description = dto.Description
    };

    private static WorkflowDefinitionResponse MapToDefinitionResponse(WorkflowDefinition definition) => new()
    {
        Id = definition.Id,
        Name = definition.Name,
        Description = definition.Description,
        CreatedAt = definition.CreatedAt,
        States = definition.States.Select(s => new StateDto
        {
            Id = s.Id,
            Name = s.Name,
            IsInitial = s.IsInitial,
            IsFinal = s.IsFinal,
            Enabled = s.Enabled,
            Description = s.Description
        }).ToList(),
        Actions = definition.Actions.Select(a => new ActionDto
        {
            Id = a.Id,
            Name = a.Name,
            Enabled = a.Enabled,
            FromStates = a.FromStates,
            ToState = a.ToState,
            Description = a.Description
        }).ToList()
    };

    private static WorkflowInstanceResponse MapToInstanceResponse(WorkflowInstance instance) => new()
    {
        Id = instance.Id,
        DefinitionId = instance.DefinitionId,
        CurrentState = instance.CurrentState,
        CreatedAt = instance.CreatedAt,
        LastUpdated = instance.LastUpdated,
        History = instance.History.Select(h => new HistoryEntryDto
        {
            ActionId = h.ActionId,
            FromState = h.FromState,
            ToState = h.ToState,
            Timestamp = h.Timestamp
        }).ToList()
    };
}
