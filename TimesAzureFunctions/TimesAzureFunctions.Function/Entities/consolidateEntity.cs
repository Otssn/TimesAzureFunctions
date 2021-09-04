using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimesAzureFunctions.Function.Entities
{
    public class consolidateEntity : TableEntity
    {
        public int Id { get; set; }
        public DateTime dateCreate { get; set; }
        public int minutes { get; set; }
    }
}
