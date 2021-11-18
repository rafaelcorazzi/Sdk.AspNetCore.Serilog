using System.Diagnostics.CodeAnalysis;

namespace Sdk.AspNetCore.Serilog.LogModels
{
    [ExcludeFromCodeCoverage]
    public class NumberLog
    {
        private NumberLog(decimal value) => NumberValue = value;

        public static NumberLog GetLog(decimal value) => new NumberLog(value);
        public decimal NumberValue { get; set; }
    }
}
