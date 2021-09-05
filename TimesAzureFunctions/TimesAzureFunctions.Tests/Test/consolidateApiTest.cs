using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimesAzureFunctions.common.Model;
using TimesAzureFunctions.Function.Functions;
using TimesAzureFunctions.Tests.Helpers;
using Xunit;

namespace TimesAzureFunctions.Tests.Test
{
    public class consolidateApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        public Task ConsolidateApi { get; private set; }

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            //Arregne
            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            MockCloudTableConsolidate mockConsolidate= new MockCloudTableConsolidate(new Uri("http://127.0.0.1:10002/devstoreaccount1/consolidateProcess"));
            Consolidate consolidateRequest = TestFactory.GetConsolidateRequest();
            DefaultHttpRequest request = TestFactory.CreateHttoRequestConsolidate(consolidateRequest);

            //Act

            IActionResult response = await ConsolidateTimeAzure.consolidateGet(request, mockTime, mockConsolidate , logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void ConsolidateByDate_Should_Return_200()
        {
            //Arregne
            MockCloudTableConsolidate mockConsolidate = new MockCloudTableConsolidate(new Uri("http://127.0.0.1:10002/devstoreaccount1/consolidateProcess"));
            Consolidate consolidateRequest = TestFactory.GetConsolidateRequest();
            DefaultHttpRequest request = TestFactory.CreateHttoRequestConsolidate(consolidateRequest);
            DateTime datet = DateTime.UtcNow;

            //Act

            IActionResult response = await ConsolidateTimeAzure.GetConsolidateByDate(request, mockConsolidate, datet, logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
