using WorkflowService.Models;

namespace WorkflowService.Services;

public class WorkflowValidationService
{
    public ValidationResult ValidateWorkflowDefinition(WorkflowDefinition definition)
    {
        var errors = new List<string>();

        var stateIds = definition.States.Select(s => s.Id).ToList();
        var duplicateStates = stateIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var duplicate in duplicateStates)
        {
            errors.Add($"Duplicate state ID: {duplicate}");
        }

        var actionIds = definition.Actions.Select(a => a.Id).ToList();
        var duplicateActions = actionIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var duplicate in duplicateActions)
        {
            errors.Add($"Duplicate action ID: {duplicate}");
        }

        var initialStates = definition.States.Where(s => s.IsInitial).ToList();
        if (initialStates.Count == 0)
        {
            errors.Add("Workflow must have exactly one initial state");
        }
        else if (initialStates.Count > 1)
        {
            errors.Add($"Workflow must have exactly one initial state, found {initialStates.Count}");
        }

        foreach (var action in definition.Actions)
        {
            foreach (var fromState in action.FromStates)
            {
                if (!stateIds.Contains(fromState))
                {
                    errors.Add($"Action '{action.Id}' references unknown fromState: {fromState}");
                }
            }

            if (!stateIds.Contains(action.ToState))
            {
                errors.Add($"Action '{action.Id}' references unknown toState: {action.ToState}");
            }

            if (!action.FromStates.Any())
            {
                errors.Add($"Action '{action.Id}' must have at least one fromState");
            }
        }

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }

    public ValidationResult ValidateActionExecution(WorkflowDefinition definition, WorkflowInstance instance, string actionId)
    {
        var errors = new List<string>();

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
        {
            errors.Add($"Action '{actionId}' not found in workflow definition");
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        if (!action.Enabled)
        {
            errors.Add($"Action '{actionId}' is disabled");
        }

        if (!action.FromStates.Contains(instance.CurrentState))
        {
            errors.Add($"Action '{actionId}' cannot be executed from current state '{instance.CurrentState}'");
        }

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState?.IsFinal == true)
        {
            errors.Add($"Cannot execute actions from final state '{instance.CurrentState}'");
        }

        var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (targetState == null)
        {
            errors.Add($"Target state '{action.ToState}' not found");
        }
        else if (!targetState.Enabled)
        {
            errors.Add($"Target state '{action.ToState}' is disabled");
        }

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
{
    /// <summary>
    /// Validates a workflow definition
    /// </summary>
    public ValidationResult ValidateWorkflowDefinition(WorkflowDefinition definition)
    {
        var errors = new List<string>();

        // Check for duplicate state IDs
        var stateIds = definition.States.Select(s => s.Id).ToList();
        var duplicateStates = stateIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var duplicate in duplicateStates)
        {
            errors.Add($"Duplicate state ID: {duplicate}");
        }

        // Check for duplicate action IDs
        var actionIds = definition.Actions.Select(a => a.Id).ToList();
        var duplicateActions = actionIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var duplicate in duplicateActions)
        {
            errors.Add($"Duplicate action ID: {duplicate}");
        }

        // Must have exactly one initial state
        var initialStates = definition.States.Where(s => s.IsInitial).ToList();
        if (initialStates.Count == 0)
        {
            errors.Add("Workflow must have exactly one initial state");
        }
        else if (initialStates.Count > 1)
        {
            errors.Add($"Workflow must have exactly one initial state, found {initialStates.Count}");
        }

        // Validate actions reference valid states
        foreach (var action in definition.Actions)
        {
            // Check fromStates exist
            foreach (var fromState in action.FromStates)
            {
                if (!stateIds.Contains(fromState))
                {
                    errors.Add($"Action '{action.Id}' references unknown fromState: {fromState}");
                }
            }

            // Check toState exists
            if (!stateIds.Contains(action.ToState))
            {
                errors.Add($"Action '{action.Id}' references unknown toState: {action.ToState}");
            }

            // Actions cannot have empty fromStates
            if (!action.FromStates.Any())
            {
                errors.Add($"Action '{action.Id}' must have at least one fromState");
            }
        }

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }

    /// <summary>
    /// Validates if an action can be executed on a workflow instance
    /// </summary>
    public ValidationResult ValidateActionExecution(WorkflowDefinition definition, WorkflowInstance instance, string actionId)
    {
        var errors = new List<string>();

        // Find the action
        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
        {
            errors.Add($"Action '{actionId}' not found in workflow definition");
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        // Check if action is enabled
        if (!action.Enabled)
        {
            errors.Add($"Action '{actionId}' is disabled");
        }

        // Check if current state is in fromStates
        if (!action.FromStates.Contains(instance.CurrentState))
        {
            errors.Add($"Action '{actionId}' cannot be executed from current state '{instance.CurrentState}'");
        }

        // Check if current state is final (cannot transition from final states)
        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState?.IsFinal == true)
        {
            errors.Add($"Cannot execute actions from final state '{instance.CurrentState}'");
        }

        // Check if target state exists and is enabled
        var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (targetState == null)
        {
            errors.Add($"Target state '{action.ToState}' not found");
        }
        else if (!targetState.Enabled)
        {
            errors.Add($"Target state '{action.ToState}' is disabled");
        }

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

/// <summary>
/// Result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
