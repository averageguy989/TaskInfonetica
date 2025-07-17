using WorkflowService.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace WorkflowService.Services;

public class WorkflowStorageService
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();
    private readonly string _dataDirectory;
    private readonly ILogger<WorkflowStorageService> _logger;

    public WorkflowStorageService(ILogger<WorkflowStorageService> logger)
    {
        _logger = logger;
        _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(_dataDirectory);
        LoadFromFiles();
    }

    public async Task<string> SaveDefinitionAsync(WorkflowDefinition definition)
    {
        if (string.IsNullOrEmpty(definition.Id))
        {
            definition.Id = Guid.NewGuid().ToString();
        }

        _definitions[definition.Id] = definition;
        await SaveDefinitionToFileAsync(definition);
        _logger.LogInformation("Saved workflow definition: {DefinitionId}", definition.Id);
        return definition.Id;
    }

    public Task<WorkflowDefinition?> GetDefinitionAsync(string id)
    {
        _definitions.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }

    public Task<List<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        return Task.FromResult(_definitions.Values.ToList());
    }

    public async Task<bool> DeleteDefinitionAsync(string id)
    {
        var removed = _definitions.TryRemove(id, out _);
        if (removed)
        {
            var filePath = Path.Combine(_dataDirectory, $"definition_{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _logger.LogInformation("Deleted workflow definition: {DefinitionId}", id);
        }
        return removed;
    }

    public async Task<string> SaveInstanceAsync(WorkflowInstance instance)
    {
        if (string.IsNullOrEmpty(instance.Id))
        {
            instance.Id = Guid.NewGuid().ToString();
        }

        instance.LastUpdated = DateTime.UtcNow;
        _instances[instance.Id] = instance;
        await SaveInstanceToFileAsync(instance);
        _logger.LogInformation("Saved workflow instance: {InstanceId}", instance.Id);
        return instance.Id;
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string id)
    {
        _instances.TryGetValue(id, out var instance);
        return Task.FromResult(instance);
    }

    public Task<List<WorkflowInstance>> GetAllInstancesAsync()
    {
        return Task.FromResult(_instances.Values.ToList());
    }

    public Task<List<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId)
    {
        var instances = _instances.Values.Where(i => i.DefinitionId == definitionId).ToList();
        return Task.FromResult(instances);
    }

    public async Task<bool> DeleteInstanceAsync(string id)
    {
        var removed = _instances.TryRemove(id, out _);
        if (removed)
        {
            var filePath = Path.Combine(_dataDirectory, $"instance_{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _logger.LogInformation("Deleted workflow instance: {InstanceId}", id);
        }
        return removed;
    }

    private void LoadFromFiles()
    {
        try
        {
            var definitionFiles = Directory.GetFiles(_dataDirectory, "definition_*.json");
            foreach (var file in definitionFiles)
            {
                var json = File.ReadAllText(file);
                var definition = JsonConvert.DeserializeObject<WorkflowDefinition>(json);
                if (definition != null)
                {
                    _definitions[definition.Id] = definition;
                }
            }

            var instanceFiles = Directory.GetFiles(_dataDirectory, "instance_*.json");
            foreach (var file in instanceFiles)
            {
                var json = File.ReadAllText(file);
                var instance = JsonConvert.DeserializeObject<WorkflowInstance>(json);
                if (instance != null)
                {
                    _instances[instance.Id] = instance;
                }
            }

            _logger.LogInformation("Loaded {DefinitionCount} definitions and {InstanceCount} instances from files", 
                _definitions.Count, _instances.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from files");
        }
    }

    private async Task SaveDefinitionToFileAsync(WorkflowDefinition definition)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"definition_{definition.Id}.json");
            var json = JsonConvert.SerializeObject(definition, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving definition to file: {DefinitionId}", definition.Id);
        }
    }

    private async Task SaveInstanceToFileAsync(WorkflowInstance instance)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"instance_{instance.Id}.json");
            var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving instance to file: {InstanceId}", instance.Id);
        }
    }
}
