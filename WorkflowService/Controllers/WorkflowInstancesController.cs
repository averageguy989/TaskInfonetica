using Microsoft.AspNetCore.Mvc;
using WorkflowService.DTOs;
using WorkflowService.Services;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkflowInstancesController : ControllerBase
{
    private readonly Services.WorkflowService _workflowService;
    private readonly ILogger<WorkflowInstancesController> _logger;

    public WorkflowInstancesController(Services.WorkflowService workflowService, ILogger<WorkflowInstancesController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 400)]
    public async Task<ActionResult<ApiResponse<WorkflowInstanceResponse>>> StartInstance([FromBody] StartWorkflowInstanceRequest request)
    {
        var result = await _workflowService.StartInstanceAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/actions")]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 404)]
    public async Task<ActionResult<ApiResponse<WorkflowInstanceResponse>>> ExecuteAction(string id, [FromBody] ExecuteActionRequest request)
    {
        var result = await _workflowService.ExecuteActionAsync(id, request);
        
        if (!result.Success)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 404)]
    public async Task<ActionResult<ApiResponse<WorkflowInstanceResponse>>> GetInstance(string id)
    {
        var result = await _workflowService.GetInstanceAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}/state")]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WorkflowInstanceResponse>), 404)]
    public async Task<ActionResult<ApiResponse<WorkflowInstanceResponse>>> GetInstanceState(string id)
    {
        var result = await _workflowService.GetInstanceAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<WorkflowInstanceResponse>>), 200)]
    public async Task<ActionResult<ApiResponse<List<WorkflowInstanceResponse>>>> GetAllInstances()
    {
        var result = await _workflowService.GetAllInstancesAsync();
        return Ok(result);
    }
}
