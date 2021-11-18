using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdk.AspNetCore.Serilog
{
    public class CorrelationIdOptions
    {
        private const string DefaultHeader = "X-Correlation-ID";

        /// <summary>
        /// Generates a Correlation-ID header with a GUID.
        /// </summary>
        /// <example>6811bcb3-dead-43bb-a738-2c5faf98ce1d</example>
        /// <returns>A new GUID</returns>
        public static string GuidCorrelationIdBuilder() => Guid.NewGuid().ToString();

        /// <summary>
        /// Generates a Correlation-ID header with a GUID, without the dashes.
        /// </summary>
        /// <example>6811bcb3dead43bba7382c5faf98ce1d</example>
        /// <returns>A new GUID</returns>
        public static string ShortGuidCorrelationIdBuilder() => Guid.NewGuid().ToString("N");

        /// <summary>
        /// The header field name where the correlation ID will be stored
        /// </summary>
        public string Header { get; set; } = DefaultHeader;

        /// <summary>
        /// Controls whether the correlation ID is returned in the response headers
        /// </summary>
        public bool IncludeInResponse { get; set; } = true;

        /// <summary>
        /// The function that will construct a new Correlation-ID header value, if none is found.
        /// To skip generation of the header, set this property to NULL.
        /// Default: <see cref="GuidCorrelationIdBuilder"/>
        /// </summary>
        public Func<string> CorrelationIdBuilder { get; set; } = GuidCorrelationIdBuilder;
    }
}
