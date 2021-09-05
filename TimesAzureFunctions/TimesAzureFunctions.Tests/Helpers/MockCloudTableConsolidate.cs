using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TimesAzureFunctions.Tests.Helpers
{
    class MockCloudTableConsolidate : CloudTable
    {
        public MockCloudTableConsolidate(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableConsolidate(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableConsolidate(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }
        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetconsolidateEntity()
            });
        }
        public override async Task<TableQuerySegment<consolidateEntity>> ExecuteQuerySegmentedAsync<consolidateEntity>(TableQuery<consolidateEntity> query, TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<consolidateEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                   .FirstOrDefault(c => c.GetParameters().Count() == 1);

            return await Task.FromResult(constructor.Invoke(new object[] { TestFactory.GetsConsolidateEntities() }) as TableQuerySegment<consolidateEntity>);
        }
    }
}
