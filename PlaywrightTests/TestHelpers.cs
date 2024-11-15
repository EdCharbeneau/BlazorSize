using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    internal static class TestHelpers
    {
        public static string SUTUrlTemplate { get; } = "https://localhost:7249{0}";
        public static string GetUrl(string route) => string.Format(SUTUrlTemplate, route);
    }
}
