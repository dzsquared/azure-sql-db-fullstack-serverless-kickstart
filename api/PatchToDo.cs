// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureSQL.ToDo
{
    public static class PatchToDo
    {
        // update an item from new data in body object
        // receives a list in the body with the existing data in at first position, and updates in at second position
        // uses output binding to update the row in ToDo table
        [FunctionName("PatchToDo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "ToDo/{id}")] HttpRequest req, string id,
            ILogger log,
            [Sql("dbo.ToDo", "SqlConnectionString")] IAsyncCollector<ToDoItem> toDoItems)
        {

            // read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);

            // new content of the item
            ToDoItem newToDoItem = JsonConvert.DeserializeObject<ToDoItem>(requestBody);
            log.LogInformation("deserialized request");

            ToDoItem toDoItem;
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ToDoUri"));
                log.LogInformation(client.BaseAddress.ToString()+"/"+id);

                // existing content of the item
                var getExistingResponse = await client.GetAsync("/api/ToDo/"+id);
                string existingBody = await getExistingResponse.Content.ReadAsStringAsync();
                log.LogInformation(existingBody);
                toDoItem = JsonConvert.DeserializeObject<List<ToDoItem>>(existingBody)[0];
            }

            if (toDoItem == null)
            {
                return new NotFoundObjectResult($"Item not found: {id}");
            }

            // compare the two items attributes
            if (newToDoItem.title != null)
            {
                toDoItem.title = newToDoItem.title;
            }
            if (newToDoItem.order != null)
            {
                toDoItem.order = newToDoItem.order;
            }
            if (newToDoItem.completed != null)
            {
                toDoItem.completed = newToDoItem.completed;
            }

            await toDoItems.AddAsync(toDoItem);
            await toDoItems.FlushAsync();

            return new OkObjectResult(toDoItem);
        }
    }
}
