# Sample Workflow Definitions

## Simple Approval Workflow

This is a basic approval workflow that can be used for testing.

```json
{
  "name": "Simple Approval Workflow",
  "description": "A basic document approval process",
  "states": [
    {
      "id": "draft",
      "name": "Draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true,
      "description": "Document is being prepared"
    },
    {
      "id": "pending",
      "name": "Pending Approval",
      "isInitial": false,
      "isFinal": false,
      "enabled": true,
      "description": "Document is waiting for approval"
    },
    {
      "id": "approved",
      "name": "Approved",
      "isInitial": false,
      "isFinal": true,
      "enabled": true,
      "description": "Document has been approved"
    },
    {
      "id": "rejected",
      "name": "Rejected",
      "isInitial": false,
      "isFinal": true,
      "enabled": true,
      "description": "Document has been rejected"
    }
  ],
  "actions": [
    {
      "id": "submit",
      "name": "Submit for Approval",
      "enabled": true,
      "fromStates": ["draft"],
      "toState": "pending",
      "description": "Submit document for approval"
    },
    {
      "id": "approve",
      "name": "Approve",
      "enabled": true,
      "fromStates": ["pending"],
      "toState": "approved",
      "description": "Approve the document"
    },
    {
      "id": "reject",
      "name": "Reject",
      "enabled": true,
      "fromStates": ["pending"],
      "toState": "rejected",
      "description": "Reject the document"
    },
    {
      "id": "revise",
      "name": "Send Back for Revision",
      "enabled": true,
      "fromStates": ["pending"],
      "toState": "draft",
      "description": "Send back to draft for revision"
    }
  ]
}
```

## Order Processing Workflow

A more complex workflow for order processing:

```json
{
  "name": "Order Processing Workflow",
  "description": "Complete order processing from placement to delivery",
  "states": [
    {
      "id": "placed",
      "name": "Order Placed",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "validated",
      "name": "Order Validated",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "processing",
      "name": "Processing",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "shipped",
      "name": "Shipped",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "delivered",
      "name": "Delivered",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    },
    {
      "id": "cancelled",
      "name": "Cancelled",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    }
  ],
  "actions": [
    {
      "id": "validate",
      "name": "Validate Order",
      "enabled": true,
      "fromStates": ["placed"],
      "toState": "validated"
    },
    {
      "id": "start_processing",
      "name": "Start Processing",
      "enabled": true,
      "fromStates": ["validated"],
      "toState": "processing"
    },
    {
      "id": "ship",
      "name": "Ship Order",
      "enabled": true,
      "fromStates": ["processing"],
      "toState": "shipped"
    },
    {
      "id": "deliver",
      "name": "Mark as Delivered",
      "enabled": true,
      "fromStates": ["shipped"],
      "toState": "delivered"
    },
    {
      "id": "cancel_placed",
      "name": "Cancel Order",
      "enabled": true,
      "fromStates": ["placed", "validated"],
      "toState": "cancelled"
    }
  ]
}
```
