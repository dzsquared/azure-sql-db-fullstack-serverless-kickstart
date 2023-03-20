// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureSQL.ToDo
{
    public static class DeleteToDo
    {
        // delete a specific item from querystring
        // returns remaining items
        // uses input binding with a stored procedure DeleteToDo to delete items and return remaining items
        [FunctionName("DeleteToDoById")]
        public static IActionResult DeleteById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "ToDo/{id}")] HttpRequest req, string id,
            ILogger log,
            [Sql("web.DeleteToDoById", "SqlConnectionString", commandType: System.Data.CommandType.StoredProcedure, 
                parameters: "@Id={id}")] 
                IEnumerable<ToDoItem> toDoItems)
        {
            Guid GId = Guid.Empty;
            if (!Guid.TryParse(id, out GId) && !string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult($"Invalid id: {id}");
            }
            return new OkObjectResult(toDoItems);
        }

        // delete all items
        // returns remaining items
        // uses input binding with a stored procedure DeleteToDo to delete items and return remaining items
        [FunctionName("DeleteToDo")]
        public static IActionResult DeleteAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "ToDo")] HttpRequest req,
            ILogger log,
            [Sql("web.DeleteToDo", "SqlConnectionString", commandType: System.Data.CommandType.StoredProcedure)] IEnumerable<ToDoItem> toDoItems)
        {
            return new OkObjectResult(toDoItems);
        }
    }
}
