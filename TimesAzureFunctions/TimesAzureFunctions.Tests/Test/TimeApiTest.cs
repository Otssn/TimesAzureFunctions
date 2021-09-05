using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TimesAzureFunctions.common.Model;
using TimesAzureFunctions.Function.Entities;
using TimesAzureFunctions.Function.Functions;
using TimesAzureFunctions.Tests.Helpers;
using Xunit;

namespace TimesAzureFunctions.Tests.Test
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        public Task TimeApi { get; private set; }

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            //Arregne

            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            Times timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttoRequest(timeRequest);

            //Act

            IActionResult response = await TimesApi.RegisterEmployed(request, mockTime, logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            //Arregne

            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            Times timeRequest = TestFactory.GetTimeRequest();
            Guid TimeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttoRequest(TimeId,timeRequest);

            //Act

            IActionResult response = await TimesApi.updateEmployed(request, mockTime, TimeId.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void GetAllTime_Should_Return_200()
        {
            //Arregne

            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            Times timeRequest = TestFactory.GetTimeRequest();
            Guid TimeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttoRequest(TimeId, timeRequest);

            //Act

            IActionResult response = await TimesApi.GetAllRegister(request, mockTime, logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public void GetById_Should_Return_200()
        {
            //Arregne

            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            Times timeRequest = TestFactory.GetTimeRequest();
            Guid TimeId = Guid.NewGuid();
            TimeEntity entity = TestFactory.GetTimeEntity();
            DefaultHttpRequest request = TestFactory.CreateHttoRequest(TimeId, timeRequest);

            //Act

            IActionResult response = TimesApi.GetregisterById(request, entity, TimeId.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void DeleteTime_Should_Return_200()
        {
            //Arregne

            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            Times timeRequest = TestFactory.GetTimeRequest();
            Guid TimeId = Guid.NewGuid();
            TimeEntity entity = TestFactory.GetTimeEntity();
            DefaultHttpRequest request = TestFactory.CreateHttoRequest(TimeId, timeRequest);

            //Act

            IActionResult response = await TimesApi.DeleteRegister(request, entity, mockTime , TimeId.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
