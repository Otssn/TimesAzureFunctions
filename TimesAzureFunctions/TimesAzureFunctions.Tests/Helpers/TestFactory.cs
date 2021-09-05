using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TimesAzureFunctions.common.Model;
using TimesAzureFunctions.Function.Entities;

namespace TimesAzureFunctions.Tests.Helpers
{
    public class TestFactory
    {

        public static TimeEntity GetTimeEntity()
        {
            return new TimeEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Id = 2,
                dateCreate = DateTime.UtcNow,
                type = 0,
                consolidate = false
            };
        }
        public static consolidateEntity GetconsolidateEntity()
        {
            return new consolidateEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Id = 2,
                dateCreate = DateTime.UtcNow,
                minutes = 0
            };
        }
        public static List<TimeEntity> GetsTimeEntitys()
        {
            List<TimeEntity> lis = new List<TimeEntity>();
            return lis;
        }
        public static List<consolidateEntity> GetsConsolidateEntities()
        {
            List<consolidateEntity> lis = new List<consolidateEntity>();
            return lis;
        }

        public static DefaultHttpRequest CreateHttoRequest(Guid timeId, Times timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request),
                Path = $"/{timeId}"
            };
        }

        public static DefaultHttpRequest CreateHttoRequestConsolidate(Guid timeId, Consolidate consolidateRequest)
        {
            string request = JsonConvert.SerializeObject(consolidateRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request),
                Path = $"/{timeId}"
            };
        }

        public static DefaultHttpRequest CreateHttoRequest(Guid timeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{timeId}"
            };
        }

        public static DefaultHttpRequest CreateHttoRequestConsolidate(Guid timeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{timeId}"
            };
        }

        public static DefaultHttpRequest CreateHttoRequest(Times timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttoRequestConsolidate(Consolidate consolidateRequest)
        {
            string request = JsonConvert.SerializeObject(consolidateRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttoRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static DefaultHttpRequest CreateHttoRequestConsolidate()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Times GetTimeRequest()
        {
            return new Times
            {
                Id = 1,
                dateCreate = DateTime.UtcNow,
                type = 0,
                consolidate = false
            };
        }
        public static Consolidate GetConsolidateRequest()
        {
            return new Consolidate
            {
                Id = 1,
                dateCreate = DateTime.UtcNow,
                minutes = 0
            };
        }


        public static Stream GenereteStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes types = LoggerTypes.Null)
        {
            ILogger logger;
            if (types == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }
    }
}
