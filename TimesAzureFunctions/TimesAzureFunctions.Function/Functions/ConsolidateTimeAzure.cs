using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TimesAzureFunctions.common.Responses;
using TimesAzureFunctions.Function.Entities;

namespace TimesAzureFunctions.Function.Functions
{
    public static class ConsolidateTimeAzure
    {
        [FunctionName(nameof(consolidateGet))]
        public static async Task<IActionResult> consolidateGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "consolidateProcess")] HttpRequest req,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("consolidateProcess", Connection = "AzureWebJobsStorage")] CloudTable consolidateprocess,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();


            TableQuery<TimeEntity> query2 = new TableQuery<TimeEntity>();
            TableQuerySegment<TimeEntity> timis = await timeTable.ExecuteQuerySegmentedAsync(query2, null);

            int creadRegister = 0;
            int updateRegister = 0;

            TimeEntity[] tiea = timis.Results.ToArray();
            if (tiea.Length == 0)
            {
                return new OkObjectResult(new Response
                {
                    IsSuccess = true,
                    Message = $"Records add: {creadRegister}, records updated: {updateRegister}"
                });
            }

            string filter = TableQuery.GenerateFilterConditionForBool("consolidate", QueryComparisons.Equal, false);
            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> registers = await timeTable.ExecuteQuerySegmentedAsync(query, null);

            TableQuery<consolidateEntity> query3 = new TableQuery<consolidateEntity>();
            TableQuerySegment<consolidateEntity> registers2 = await consolidateprocess.ExecuteQuerySegmentedAsync(query3, null);

            if (registers.Results == null)
            {
                return new OkObjectResult(new Response
                {
                    IsSuccess = true,
                    Message = $"Records add: {creadRegister}, records updated: {updateRegister}"
                });
            }


            TimeEntity[] reg = registers.Results.ToArray();
            consolidateEntity[] reg2 = registers2.Results.ToArray();

            IEnumerable<TimeEntity> registe = reg.OrderBy(a => a.dateCreate).OrderBy(a => a.Id);

            TimeEntity[] auxreg = registe.ToArray();
            TimeEntity timeEntity;
            TimeEntity timeEntity2;
            consolidateEntity consolidate;

            string[] aux;
            string[] auxD;
            string[] aux2;
            int sw = 0;

            int auxI = 0;
            int sw3 = 0;

            do
            {
                if (reg.Length != 1)
                {
                    if (auxreg[auxI].type == 0 && auxreg[auxI + 1].type == 1)
                    {
                        sw3 = 1;
                    }
                    else
                    {
                        auxI++;
                        if ((auxI + 1) == auxreg.Length)
                        {
                            return new BadRequestObjectResult(new Response
                            {
                                IsSuccess = false,
                                Message = "There is no record to consolidate"
                            });
                        }
                    }
                }
                else
                {
                    return new BadRequestObjectResult(new Response
                    {
                        IsSuccess = false,
                        Message = "There is no record to consolidate"
                    });
                }

            } while (sw3 == 0);

            int auxId = auxreg[auxI].Id;
            TimeSpan ts = auxreg[auxI + 1].dateCreate - auxreg[auxI].dateCreate;

            int minut = (int)ts.TotalMinutes;
            string[] auxDate = auxreg[0].dateCreate.ToString().Split(" ");
            consolidateEntity centity = null;
            for (int i = auxI + 2; i < auxreg.Length; i += 2)
            {
                if ((i + 1) != auxreg.Length)
                {
                    if (auxreg[i].type == 0 && auxreg[i + 1].type == 1)
                    {
                        if (auxreg[i].Id == auxId)
                        {
                            aux = auxreg[i].dateCreate.ToString().Split(" ");
                            if (auxDate[0] == aux[0])
                            {
                                ts = auxreg[i + 1].dateCreate - auxreg[i].dateCreate;
                                minut += (int)ts.TotalMinutes;

                                timeEntity = auxreg[i];
                                timeEntity.consolidate = true;
                                timeEntity2 = auxreg[i + 1];
                                timeEntity2.consolidate = true;

                                TableOperation addOperation = TableOperation.Replace(timeEntity);
                                await timeTable.ExecuteAsync(addOperation);

                                TableOperation addOperation2 = TableOperation.Replace(timeEntity2);
                                await timeTable.ExecuteAsync(addOperation2);
                            }
                            else
                            {
                                auxDate = auxreg[i].dateCreate.ToString().Split(" ");
                                auxD = auxreg[i - 1].dateCreate.Date.ToString().Split("");

                                for (int j = 0; j < reg2.Length; j++)
                                {
                                    aux2 = auxreg[i - 1].dateCreate.ToString().Split(" ");
                                    string[] aux3 = reg2[j].dateCreate.ToString().Split(" ");
                                    if (auxreg[i - 1].Id == reg2[j].Id && aux2[0] == aux3[0])
                                    {
                                        sw = 1;
                                        reg2[j].minutes += minut;
                                        centity = reg2[j];
                                    }
                                }
                                if (sw == 1)
                                {
                                    TableOperation addOperation9 = TableOperation.Replace(centity);
                                    await consolidateprocess.ExecuteAsync(addOperation9);
                                    updateRegister++;
                                }
                                else
                                {
                                    consolidate = new consolidateEntity
                                    {
                                        Id = auxreg[i - 1].Id,
                                        dateCreate = Convert.ToDateTime(auxD[0]),
                                        minutes = minut,
                                        ETag = "*",
                                        PartitionKey = "CONSOLIDATE",
                                        RowKey = Guid.NewGuid().ToString()
                                    };
                                    TableOperation addOperation = TableOperation.Insert(consolidate);
                                    await consolidateprocess.ExecuteAsync(addOperation);
                                    creadRegister++;
                                }

                                ts = auxreg[i + 1].dateCreate - auxreg[i].dateCreate;

                                minut = (int)ts.TotalMinutes;

                            }

                        }
                        else
                        {
                            auxDate = auxreg[i].dateCreate.ToString().Split(" ");
                            auxId = auxreg[i].Id;
                            auxD = auxreg[i - 1].dateCreate.Date.ToString().Split("");

                            for (int j = 0; j < reg2.Length; j++)
                            {
                                aux2 = auxreg[i - 1].dateCreate.ToString().Split(" ");
                                string[] aux3 = reg2[j].dateCreate.ToString().Split(" ");
                                if (auxreg[i - 1].Id == reg2[j].Id && aux2[0] == aux3[0])
                                {
                                    sw = 1;
                                    reg2[j].minutes += minut;
                                    centity = reg2[j];
                                }
                            }
                            if (sw == 1)
                            {
                                TableOperation addOperation9 = TableOperation.Replace(centity);
                                await consolidateprocess.ExecuteAsync(addOperation9);
                                updateRegister++;
                            }
                            else
                            {
                                consolidate = new consolidateEntity
                                {
                                    Id = auxreg[i - 1].Id,
                                    dateCreate = Convert.ToDateTime(auxD[0]),
                                    minutes = minut,
                                    ETag = "*",
                                    PartitionKey = "CONSOLIDATE",
                                    RowKey = Guid.NewGuid().ToString()
                                };
                                TableOperation addOperation = TableOperation.Insert(consolidate);
                                await consolidateprocess.ExecuteAsync(addOperation);
                                creadRegister++;
                            }

                            ts = auxreg[i + 1].dateCreate - auxreg[i].dateCreate;

                            minut = (int)ts.TotalMinutes;

                            timeEntity = auxreg[i];
                            timeEntity.consolidate = true;
                            timeEntity2 = auxreg[i + 1];
                            timeEntity2.consolidate = true;

                            TableOperation addOperation3 = TableOperation.Replace(timeEntity);
                            await timeTable.ExecuteAsync(addOperation3);

                            TableOperation addOperation4 = TableOperation.Replace(timeEntity2);
                            await timeTable.ExecuteAsync(addOperation4);
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
                else
                {
                    if (auxreg[i - 1].Id == auxreg[i - 2].Id && auxreg[i - 1].type == 1 && auxreg[i - 2].type == 0)
                    {
                        auxDate = auxreg[i - 1].dateCreate.ToString().Split(" ");
                        auxId = auxreg[i - 1].Id;
                        auxD = auxreg[i - 2].dateCreate.Date.ToString().Split("");

                        for (int j = 0; j < reg2.Length; j++)
                        {
                            aux2 = auxreg[i - 2].dateCreate.ToString().Split(" ");
                            string[] aux3 = reg2[j].dateCreate.ToString().Split(" ");
                            if (auxreg[i - 2].Id == reg2[j].Id && aux2[0] == aux3[0])
                            {
                                sw = 1;
                                reg2[j].minutes += minut;
                                centity = reg2[j];
                            }
                        }
                        if (sw == 1)
                        {
                            TableOperation addOperation9 = TableOperation.Replace(centity);
                            await consolidateprocess.ExecuteAsync(addOperation9);
                            updateRegister++;
                        }
                        else
                        {
                            consolidate = new consolidateEntity
                            {
                                Id = auxreg[i - 2].Id,
                                dateCreate = Convert.ToDateTime(auxD[0]),
                                minutes = minut,
                                ETag = "*",
                                PartitionKey = "CONSOLIDATE",
                                RowKey = Guid.NewGuid().ToString()
                            };
                            TableOperation addOperation = TableOperation.Insert(consolidate);
                            await consolidateprocess.ExecuteAsync(addOperation);
                            creadRegister++;
                        }
                    }
                }
            }
            auxD = auxreg[auxreg.Length - 1].dateCreate.Date.ToString().Split("");
            sw = 0;
            if (auxreg[auxreg.Length - 1].type == 1 && auxreg[auxreg.Length - 2].type == 0)
            {
                for (int j = 0; j < reg2.Length; j++)
                {
                    aux2 = auxreg[auxreg.Length - 1].dateCreate.ToString().Split(" ");
                    string[] aux3 = reg2[j].dateCreate.ToString().Split(" ");
                    if (auxreg[auxreg.Length - 1].Id == reg2[j].Id && aux2[0] == aux3[0])
                    {
                        sw = 1;
                        reg2[j].minutes += minut;
                        centity = reg2[j];
                    }
                }
                if (sw == 1)
                {
                    TableOperation addOperation9 = TableOperation.Replace(centity);
                    await consolidateprocess.ExecuteAsync(addOperation9);
                    updateRegister++;
                }
                else
                {
                    consolidate = new consolidateEntity
                    {
                        Id = auxreg[auxreg.Length - 1].Id,
                        dateCreate = Convert.ToDateTime(auxD[0]),
                        minutes = minut,
                        ETag = "*",
                        PartitionKey = "CONSOLIDATE",
                        RowKey = Guid.NewGuid().ToString()
                    };
                    TableOperation addOperation = TableOperation.Insert(consolidate);
                    await consolidateprocess.ExecuteAsync(addOperation);
                    creadRegister++;
                }
                timeEntity = auxreg[auxreg.Length - 1];
                timeEntity.consolidate = true;
                timeEntity2 = auxreg[auxreg.Length - 2];
                timeEntity2.consolidate = true;

                TableOperation addOperation3 = TableOperation.Replace(timeEntity);
                await timeTable.ExecuteAsync(addOperation3);

                TableOperation addOperation4 = TableOperation.Replace(timeEntity2);
                await timeTable.ExecuteAsync(addOperation4);
            }

            if (auxreg[auxI + 1].type == 1 && auxreg[auxI].type == 0)
            {
                timeEntity = auxreg[auxI];
                timeEntity.consolidate = true;
                timeEntity2 = auxreg[auxI + 1];
                timeEntity2.consolidate = true;

                TableOperation addOperation6 = TableOperation.Replace(timeEntity);
                await timeTable.ExecuteAsync(addOperation6);

                TableOperation addOperation7 = TableOperation.Replace(timeEntity2);
                await timeTable.ExecuteAsync(addOperation7);
            }


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = $"Records add: {creadRegister}, records updated: {updateRegister}"
            });
        }

        [FunctionName(nameof(GetConsolidateByDate))]
        public static async Task<IActionResult> GetConsolidateByDate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidateProcess/{date}")] HttpRequest req,
            [Table("consolidateProcess", Connection = "AzureWebJobsStorage")] CloudTable consolidateprocess,
            DateTime date,
            ILogger log)
        {
            log.LogInformation($"Get register by date: {date} received.");

            string filter = TableQuery.GenerateFilterConditionForDate("dateCreate", QueryComparisons.Equal, date);
            TableQuery<consolidateEntity> query = new TableQuery<consolidateEntity>().Where(filter);
            TableQuerySegment<consolidateEntity> registers = await consolidateprocess.ExecuteQuerySegmentedAsync(query, null);

            string message = $"Consolidate by Date";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = registers
            });
        }
    }
}
