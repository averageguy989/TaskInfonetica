using Microsoft.AspNetCore.Mvc;
using WorkflowService.DTOs;
using WorkflowService.Services;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkflowDefinitionsController : ControllerBase
{
    private readonly Services.WorkflowService _workflowService;
    private readonly ILogger<WorkflowDefinitionsController> _logger;

    public WorkflowDefinitionsController(Services.WorkflowService workflowService, ILogger<WorkflowDefinitionsController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorkflowDefinitionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowDefinitionResponse>), 400)]
    public async Task<ActionResult<ApiResponse<WorkflowDefinitionResponse>>> CreateDefinition([FromBody] CreateWorkflowDefinitionRequest request)
    {
        var result = await _workflowService.CreateDefinitionAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorkflowDefinitionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowDefinitionResponse>), 404)]
    public async Task<ActionResult<ApiResponse<WorkflowDefinitionResponse>>> GetDefinition(string id)
    {
        var result = await _workflowService.GetDefinitionAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<WorkflowDefinitionResponse>>), 200)]
    public async Task<ActionResult<ApiResponse<List<WorkflowDefinitionResponse>>>> GetAllDefinitions()
    {
        var result = await _workflowService.GetAllDefinitionsAsync();
        return Ok(result);
    }
}
