using System;
using System.Reflection;

namespace PhishingTraining.Web.Helpers
{
    public static class AssemblyHelper
    {
        private static string AssemblyInformationalVersion;

        public static string GetAssemblyInformationalVersion()
        {
            if (AssemblyInformationalVersion == null)
            {
                var assembly = Assembly.GetAssembly(typeof(AssemblyHelper));
                var attr = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                AssemblyInformationalVersion = attr?.InformationalVersion
                                               ?? throw new ArgumentNullException(nameof(AssemblyInformationalVersionAttribute));
            }

            return AssemblyInformationalVersion;
        }
    }
}