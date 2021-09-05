using System;

namespace TimesAzureFunctions.Tests.Helpers
{
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; set; } = new NullScope();
        public void Dispose() { }
        private NullScope() { }
    }
}
