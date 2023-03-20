// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureSQL.ToDo
{
    public static class GetToDos
    {
        // return items from ToDo table
        // id querystring in the query text to filter
        // uses input binding to run the query and return the results
        [FunctionName("GetToDosById")]
        public static async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ToDo/{id}")] HttpRequest req, string id,
            ILogger log,
            [Sql("select Id, [order], title, url, completed from dbo.ToDo where @Id = Id", "SqlConnectionString", commandType: System.Data.CommandType.Text, parameters: "@Id={id}")] IAsyncEnumerable<ToDoItem> toDos)
        {
            Guid GId = Guid.Empty;
            if (!Guid.TryParse(id, out GId) && !string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult($"Invalid id: {id}");
            }
            return new OkObjectResult(toDos);
        }

        // return items from ToDo table
        // uses input binding to run the query and return the results
        [FunctionName("GetAllToDos")]
        public static async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ToDo")] HttpRequest req,
            ILogger log,
            [Sql("select Id, [order], title, url, completed from dbo.ToDo", "SqlConnectionString", commandType: System.Data.CommandType.Text)] IAsyncEnumerable<ToDoItem> toDos)
        {
            return new OkObjectResult(toDos);
        }
    }
}
