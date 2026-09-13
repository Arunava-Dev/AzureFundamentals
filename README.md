Absolutely. I analyzed the repository structure and the actual Azure Function/Blob/Queue/Timer/Logic App code that is currently in `AzureFundamentals`. I’ve kept this README **accurate to the implementation**, rather than making it sound bigger than it is.

One important thing: **I have deliberately not put your Logic App URL/signature in the README**, because that credential is currently exposed in `HomeController.cs` and the repository is public. Rotate that signature before sharing the repository widely.

Here is the README content you can copy-paste directly into GitHub:

# Azure Fundamentals – .NET & Azure Hands-on Projects

A collection of hands-on **.NET and Microsoft Azure projects** built to understand and practice Azure cloud services, serverless computing, event-driven architecture, Azure Storage, database integration, and workflow automation.

This repository contains multiple independent examples covering **Azure Functions, Azure Blob Storage, Azure Queue Storage, Azure SQL/SQL Server, Entity Framework Core, Azure Logic Apps, and image processing**.

---

## 🛠️ Technologies Used

* **C#**
* **.NET 9 / .NET 10**
* **ASP.NET Core MVC**
* **Azure Functions v4 – Isolated Worker Model**
* **Azure Blob Storage**
* **Azure Queue Storage**
* **Azure Logic Apps**
* **Azure SQL / SQL Server**
* **Entity Framework Core**
* **Azure Storage SDK for .NET**
* **Newtonsoft.Json**
* **SixLabors ImageSharp**
* **OpenTelemetry / Azure Monitor**
* **Visual Studio**
* **Git & GitHub**

---

# 📁 Project Structure

```text
AzureFundamentals
│
├── AzureBlobProject
│   └── ASP.NET Core MVC application for working with
│       Azure Blob Storage and containers
│
├── AzureFunctionTangyWeb
│   └── Supporting ASP.NET Core web project
│
├── TangyAzureFunc
│   └── Azure Functions demonstrating HTTP, Queue,
│       Blob and Timer triggers
│
├── AzureSpookyLogic
│   └── ASP.NET Core MVC application integrated with
│       Azure Logic Apps and Blob Storage
│
└── AzureFundamentals.slnx
```

---

# 1. Azure Blob Storage Project

### Project: `AzureBlobProject`

This project demonstrates how an ASP.NET Core MVC application can interact with **Azure Blob Storage using the Azure Storage SDK for .NET**.

The application uses dependency injection to register and consume Azure Blob Storage services.

### Implemented Operations

* Upload a blob
* Delete a blob
* Retrieve blobs from a container
* Retrieve blob names
* Retrieve blob URIs
* Read blob properties
* Store and retrieve custom blob metadata
* Work with Blob Containers
* Explore SAS URI generation

### Blob Metadata

The project demonstrates storing custom metadata such as:

```text
title
comment
```

The metadata can later be retrieved through the blob properties.

### Architecture

```text
ASP.NET Core MVC
       │
       ▼
Blob Service
       │
       ▼
BlobContainerClient
       │
       ▼
Azure Blob Storage
       │
       ├── Upload Blob
       ├── Delete Blob
       ├── List Blobs
       ├── Get Blob Properties
       └── Read Metadata
```

### Key Azure SDK Classes

```csharp
BlobServiceClient
BlobContainerClient
BlobClient
BlobHttpHeaders
BlobProperties
```

---

# 2. Azure Functions – HTTP → Queue → Database

### Project: `TangyAzureFunc`

This example demonstrates an **event-driven and decoupled architecture** using Azure Functions and Azure Queue Storage.

## Flow

```text
Client / ASP.NET Core Application
             │
             ▼
      HTTP Trigger Function
             │
             ▼
       Azure Queue Storage
     "SalesRequestInBound"
             │
             ▼
      Queue Trigger Function
             │
             ▼
       Deserialize Message
             │
             ▼
       Entity Framework Core
             │
             ▼
        SQL Database
```

### HTTP Trigger

The `OnSalesUploadWriteToQueue` function receives an HTTP request containing a `SalesRequest`.

The function:

1. Reads the HTTP request body.
2. Deserializes the JSON payload.
3. Converts it into a C# model.
4. Returns the model through a `QueueOutput` binding.
5. Azure Functions automatically writes the returned object to the configured Azure Storage Queue.

### Queue Trigger

The `OnQueueTriggerUpdateDatabase` function listens to:

```text
SalesRequestInBound
```

When a message arrives:

1. The queue message is read.
2. The JSON body is deserialized.
3. A `SalesRequest` object is created.
4. The entity is added to the Entity Framework Core `DbContext`.
5. The record is saved to the SQL database.

### Why use a Queue?

The queue creates separation between the initial request and database processing.

Instead of:

```text
HTTP Request → Database
```

the application uses:

```text
HTTP Request → Queue → Database
```

This allows the producer and consumer to operate independently and provides a foundation for asynchronous/event-driven processing.

---

# 3. Azure Functions – Blob Image Processing

The repository also demonstrates processing files uploaded to Azure Blob Storage using a **Blob Trigger**.

## Flow

```text
Blob Upload
    │
    ▼
Azure Blob Storage
    │
    ▼
Blob Trigger Function
    │
    ▼
ImageSharp
    │
    ▼
Resize Image to 100 × 100
    │
    ▼
Blob Output
"functionsalesrep-final"
```

### Function

```text
ResizeImageOnBlobUpload
```

The function is triggered when a blob is uploaded to:

```text
functionsalesrep/{name}
```

The uploaded bytes are loaded using **SixLabors ImageSharp**.

The image is resized to:

```text
100 × 100
```

The processed image is then returned through a `BlobOutput` binding and written to:

```text
functionsalesrep-final/{name}
```

### Technologies Demonstrated

* Azure Blob Trigger
* Azure Blob Output Binding
* MemoryStream
* ImageSharp
* Image resizing
* Serverless image processing

---

# 4. Azure Functions – Update Database After Image Processing

After the image has been resized and written to the processed container, another Blob Trigger function reacts to the processed blob.

### Function

```text
BlobResizeUpdateDbStatus
```

## Flow

```text
Processed Blob
      │
      ▼
Blob Trigger
      │
      ▼
Extract Request ID from File Name
      │
      ▼
Query SQL Database using EF Core
      │
      ▼
Update Status
"Image Processed"
```

The function extracts the file name without the extension and uses it to find the corresponding `SalesRequest`.

The database status is then updated to:

```text
Image Processed
```

This demonstrates how Azure Blob events can be used to drive further application processing.

---

# 5. Azure Functions – Timer Trigger

The project also contains a **Timer Trigger** function:

```text
UpdateStatusToComplete
```

The configured schedule is:

```text
0 */5 * * * *
```

which executes every five minutes.

The function finds records whose status is:

```text
Image Processed
```

and changes them to:

```text
Completed
```

## Overall Processing Flow

The complete example can therefore be viewed as:

```text
Request
   │
   ▼
HTTP Trigger
   │
   ▼
Azure Queue
   │
   ▼
Queue Trigger
   │
   ▼
SQL Database
   │
   │
   └───────────────┐
                   │
              Image Upload
                   │
                   ▼
             Blob Storage
                   │
                   ▼
             Blob Trigger
                   │
                   ▼
           ImageSharp Resize
                   │
                   ▼
          Processed Blob
                   │
                   ▼
             Blob Trigger
                   │
                   ▼
        Update DB Status
        "Image Processed"
                   │
                   ▼
             Timer Trigger
                   │
                   ▼
             "Completed"
```

---

# 6. Azure Functions – HTTP APIs

The repository also contains HTTP-triggered Azure Functions in `GroceryAPI.cs`.

Examples include:

```text
GET /GroceryList
GET /GroceryList/{id}
```

These functions demonstrate creating HTTP endpoints using:

```csharp
[HttpTrigger(
    AuthorizationLevel.Function,
    "get"
)]
```

This provides hands-on experience with:

* HTTP-triggered Azure Functions
* Function authorization levels
* Routing
* HTTP requests
* Returning API responses

---

# 7. Azure Logic Apps Integration

### Project: `AzureSpookyLogic`

This ASP.NET Core MVC application demonstrates integrating an application with **Azure Logic Apps**.

The MVC application collects request information and optionally accepts a file upload.

## Request Flow

```text
ASP.NET Core MVC
       │
       ├──────────────► HTTP Request
       │                    │
       │                    ▼
       │              Azure Logic App
       │
       └──────────────► Azure Blob Storage
```

The MVC controller:

1. Generates a unique `Guid` for the request.
2. Serializes the request object to JSON.
3. Creates an `HttpClient` using `IHttpClientFactory`.
4. Sends the JSON payload to an Azure Logic App HTTP trigger.
5. Uploads the selected file to Azure Blob Storage.
6. Uses the generated request ID as part of the blob file name.

### Azure Services Demonstrated

* Azure Logic Apps
* Logic App HTTP trigger
* ASP.NET Core `IHttpClientFactory`
* Azure Blob Storage
* Blob Container
* Blob upload
* JSON serialization

---

# 8. Azure Storage Concepts Covered

This repository provides hands-on practice with several Azure Storage concepts.

### Blob Storage

* Storage accounts
* Containers
* Blobs
* Blob upload
* Blob deletion
* Blob listing
* Blob properties
* Blob metadata
* Blob URLs
* Blob triggers
* Blob output bindings
* SAS URI exploration

### Queue Storage

* Queue messages
* Queue output binding
* Queue trigger
* Producer/consumer pattern
* Asynchronous processing

---

# 9. Azure Functions Triggers Covered

The repository demonstrates multiple Azure Functions trigger types:

| Trigger       | Example                        | Purpose                  |
| ------------- | ------------------------------ | ------------------------ |
| HTTP Trigger  | `OnSalesUploadWriteToQueue`    | Receive HTTP requests    |
| HTTP Trigger  | `GroceryAPI`                   | Expose HTTP APIs         |
| Queue Trigger | `OnQueueTriggerUpdateDatabase` | Process queue messages   |
| Blob Trigger  | `ResizeImageOnBlobUpload`      | React to blob uploads    |
| Blob Trigger  | `BlobResizeUpdateDbStatus`     | React to processed blobs |
| Timer Trigger | `UpdateStatusToComplete`       | Run scheduled processing |

---

# 10. Database Integration

The Azure Functions project uses:

```text
Entity Framework Core
```

with SQL Server.

The Functions project includes:

```text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
```

Database operations demonstrated include:

* Adding entities
* Querying entities
* Updating entity status
* Saving changes
* Using `DbContext` inside Azure Functions

---

# 11. Serverless & Event-Driven Architecture

One of the main objectives of this repository is understanding how Azure Functions can be used to build **serverless, event-driven applications**.

Instead of keeping all processing inside one synchronous application, responsibilities are separated across functions.

For example:

```text
HTTP Function
     │
     ▼
Queue
     │
     ▼
Queue Function
     │
     ▼
Database
```

and:

```text
Blob Upload
     │
     ▼
Blob Trigger
     │
     ▼
Image Processing
     │
     ▼
Processed Blob
     │
     ▼
Database Update
     │
     ▼
Timer-based Completion
```

This provides practical exposure to:

* Decoupling
* Asynchronous processing
* Event-driven architecture
* Serverless computing
* Trigger-based execution
* Azure Storage bindings
* Cloud-based workflows

---

# 12. What I Practiced

Through these projects, I gained hands-on experience with:

* Creating and working with Azure Storage resources
* Uploading and managing blobs
* Working with Blob Containers
* Reading Blob metadata and properties
* Using Azure Storage SDKs from .NET
* Creating HTTP-triggered Azure Functions
* Creating Queue-triggered Azure Functions
* Creating Blob-triggered Azure Functions
* Creating Timer-triggered Azure Functions
* Using Queue Output bindings
* Using Blob Output bindings
* Connecting Azure Functions with SQL Server
* Using Entity Framework Core inside Azure Functions
* Processing images using ImageSharp
* Integrating ASP.NET Core MVC with Azure Logic Apps
* Uploading files from ASP.NET Core to Azure Blob Storage
* Building asynchronous and event-driven processing flows

---

# 13. Key Architecture Concepts

### Decoupling

Azure Queue Storage is used to separate the request-producing component from the database-processing component.

### Event-Driven Processing

Blob and Queue triggers allow Azure Functions to execute in response to events rather than requiring continuous application execution.

### Serverless Computing

Azure Functions allow individual pieces of application logic to run independently without managing a continuously running server.

### Workflow Automation

Azure Logic Apps can be used to orchestrate application and Azure service workflows.

### Cloud Storage

Azure Blob Storage provides object storage for uploaded and processed files.

---

# 14. Repository Purpose

This repository is primarily a **hands-on learning and experimentation project** for understanding how .NET applications integrate with Microsoft Azure.

It focuses on practical implementation rather than being a single production application.

The projects are intentionally separated so that individual Azure services and architectural patterns can be explored independently.

---

## 🔗 Repository

GitHub:

[https://github.com/Arunava-Dev/AzureFundamentals](https://github.com/Arunava-Dev/AzureFundamentals)

---

## 📌 Note

This repository contains learning projects and Azure service experiments. Azure resources used during development may be created, modified, or deleted as part of the learning process.

Credentials, connection strings, function keys, and other secrets should never be committed to the repository.

This version is deliberately **interview-friendly** too. It gives you a clean story: **Blob → Queue → Functions → SQL → Logic Apps**, and every major claim corresponds to something actually present in the repository.
