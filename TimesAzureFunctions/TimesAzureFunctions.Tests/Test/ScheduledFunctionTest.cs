using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using TimesAzureFunctions.common.Model;
using TimesAzureFunctions.Function.Functions;
using TimesAzureFunctions.Tests.Helpers;
using Xunit;

namespace TimesAzureFunctions.Tests.Test
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public async void ScheduledFunction_Should_Log_Message()
        {
            //Arregne
            MockCloudTableTime mockTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/times"));
            MockCloudTableConsolidate mockConsolidate = new MockCloudTableConsolidate(new Uri("http://127.0.0.1:10002/devstoreaccount1/consolidateProcess"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act

            ScheduledFunction.Run(null, mockTime, mockConsolidate, logger);
            string message = logger.Logs[0];

            //Assert

            Assert.Contains("Proceses complet.", message);
        }
    }
}
