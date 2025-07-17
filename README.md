# Workflow Service API

A minimal backend service that implements configurable workflow state machines with full validation and state management.

## Overview

This service allows clients to:
1. **Define** workflow state machines with configurable states and actions
2. **Start** workflow instances from chosen definitions
3. **Execute** actions to move instances between states with full validation
4. **Inspect/List** states, actions, definitions, and running instances

## Architecture

### Core Components

- **WorkflowDefinition**: Contains states and actions that define a workflow template
- **WorkflowInstance**: A running instance of a workflow definition with current state and history
- **WorkflowState**: Individual states with properties (id, name, isInitial, isFinal, enabled)
- **WorkflowAction**: Transitions between states with validation rules (fromStates, toState, enabled)

### Key Services

- **WorkflowService**: Main business logic for workflow operations
- **WorkflowValidationService**: Validates definitions and action executions
- **WorkflowStorageService**: In-memory persistence with JSON file backup

## API Endpoints

### Workflow Definitions

- `POST /api/workflowdefinitions` - Create a new workflow definition
- `GET /api/workflowdefinitions` - Get all workflow definitions
- `GET /api/workflowdefinitions/{id}` - Get a specific workflow definition

### Workflow Instances

- `POST /api/workflowinstances` - Start a new workflow instance
- `GET /api/workflowinstances` - Get all workflow instances
- `GET /api/workflowinstances/{id}` - Get a specific workflow instance
- `GET /api/workflowinstances/{id}/state` - Get instance state and history
- `POST /api/workflowinstances/{id}/actions` - Execute an action on an instance

### Health Check

- `GET /health` - Simple health check endpoint

## Validation Rules

### Workflow Definition Validation
- Must have exactly one initial state (isInitial = true)
- No duplicate state or action IDs
- All action fromStates and toState must reference valid states
- Actions must have at least one fromState

### Action Execution Validation
- Action must exist in the workflow definition
- Action must be enabled
- Current state must be in the action's fromStates
- Cannot execute actions from final states
- Target state must exist and be enabled

## Data Persistence

The service uses in-memory storage with automatic JSON file persistence to the `/Data` directory:
- Workflow definitions: `definition_{id}.json`
- Workflow instances: `instance_{id}.json`

Files are automatically loaded on startup and saved when data changes.

## Example Usage

### 1. Create a Simple Approval Workflow

```json
POST /api/workflowdefinitions
{
  "name": "Simple Approval",
  "description": "A basic approval workflow",
  "states": [
    {
      "id": "draft",
      "name": "Draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "pending",
      "name": "Pending Approval",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "approved",
      "name": "Approved",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    },
    {
      "id": "rejected",
      "name": "Rejected",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    }
  ],
  "actions": [
    {
      "id": "submit",
      "name": "Submit for Approval",
      "enabled": true,
      "fromStates": ["draft"],
      "toState": "pending"
    },
    {
      "id": "approve",
      "name": "Approve",
      "enabled": true,
      "fromStates": ["pending"],
      "toState": "approved"
    },
    {
      "id": "reject",
      "name": "Reject",
      "enabled": true,
      "fromStates": ["pending"],
      "toState": "rejected"
    }
  ]
}
```

### 2. Start a Workflow Instance

```json
POST /api/workflowinstances
{
  "definitionId": "{definition-id-from-step-1}"
}
```

### 3. Execute Actions

```json
POST /api/workflowinstances/{instance-id}/actions
{
  "actionId": "submit"
}
```

## Running the Application

### Prerequisites
- .NET 8 SDK

### Development
```bash
cd WorkflowService
dotnet run
```

The API will be available at `https://localhost:7xxx` with Swagger UI at the root URL.

### Building
```bash
dotnet build
```

## Design Decisions

1. **In-Memory with File Persistence**: Simple but reliable for small-scale usage, easily replaceable with database
2. **Minimal Dependencies**: Only essential packages (ASP.NET Core, Swagger, Newtonsoft.Json)
3. **Strong Validation**: Comprehensive validation at both definition and runtime levels
4. **Immutable History**: Complete audit trail of all state transitions
5. **RESTful API**: Standard HTTP methods and status codes for easy integration
6. **Swagger Documentation**: Auto-generated API documentation for easy testing

## Future Enhancements (TODO)

- Database persistence option
- Workflow definition versioning
- Conditional transitions based on data
- Parallel workflow execution
- Workflow templates and inheritance
- Advanced query capabilities
- Authentication and authorization
- Metrics and monitoring
"# Infonetica-Assignment" 
"# Infonetica-Task" 
"# Infonetica-Task" 
