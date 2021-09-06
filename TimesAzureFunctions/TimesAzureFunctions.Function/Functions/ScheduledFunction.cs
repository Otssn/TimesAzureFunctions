using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimesAzureFunctions.Function.Entities;

namespace TimesAzureFunctions.Function.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("consolidateProcess", Connection = "AzureWebJobsStorage")] CloudTable consolidateprocess,
            ILogger log)
        {

            string filter = TableQuery.GenerateFilterConditionForBool("consolidate", QueryComparisons.Equal, false);
            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> registers = await timeTable.ExecuteQuerySegmentedAsync(query, null);

            TableQuery<consolidateEntity> query3 = new TableQuery<consolidateEntity>();
            TableQuerySegment<consolidateEntity> registers2 = await consolidateprocess.ExecuteQuerySegmentedAsync(query3, null);

            TimeEntity[] tien = registers.Results.ToArray();
            int creadRegister = 0;
            int updateRegister = 0;
            if (tien.Length != 0)
            {
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
                int swExit = 0;
                do
                {
                    if (tien.Length != 1)
                    {
                        if (auxreg[auxI].type == 0 && auxreg[auxI + 1].type == 1)
                            sw3 = 1;
                        else
                        {
                            auxI++;
                            if ((auxI + 1) == auxreg.Length)
                            {
                                swExit = 1;
                                sw3 = 1;
                            }
                        }
                    }
                    else
                    {
                        swExit = 1;
                        sw3 = 1;
                    }
                } while (sw3 == 0);
                if (swExit == 0)
                {
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
                                            TableOperation addOperation3 = TableOperation.Insert(consolidate);
                                            await consolidateprocess.ExecuteAsync(addOperation3);
                                            creadRegister++;
                                        }

                                        timeEntity = auxreg[i];
                                        timeEntity.consolidate = true;
                                        timeEntity2 = auxreg[i + 1];
                                        timeEntity2.consolidate = true;

                                        TableOperation addOperation = TableOperation.Replace(timeEntity);
                                        await timeTable.ExecuteAsync(addOperation);

                                        TableOperation addOperation2 = TableOperation.Replace(timeEntity2);
                                        await timeTable.ExecuteAsync(addOperation2);

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
                    }
                    sw = 0;
                    auxD = auxreg[auxreg.Length - 1].dateCreate.Date.ToString().Split("");
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
                }
            }
            else
            {

            }
            log.LogInformation("Proceses complet.");
            log.LogInformation($"Records add: {creadRegister}, records updated: {updateRegister}");
        }
    }
}
