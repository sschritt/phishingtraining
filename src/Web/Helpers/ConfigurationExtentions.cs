using System;
using Microsoft.Extensions.Configuration;
using PhishingTraining.Web.Entities;

namespace PhishingTraining.Web.Helpers
{
    public static class ConfigurationExtentions
    {
        public static string SmsProviderApiKey(this IConfiguration config)
        {
            return config[ConfigurationConstants.SmsProvider.ApiKey];
        }
        public static string MailServerHost(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServer.Host];
        }

        public static int MailServerPort(this IConfiguration config)
        {
            return Convert.ToInt32(config[ConfigurationConstants.MailServer.Port]);
        }

        public static bool MailServerUseSsl(this IConfiguration config)
        {
            return Convert.ToBoolean(config[ConfigurationConstants.MailServer.UseSsl]);
        }

        public static string MailServerUser(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServer.User];
        }

        public static string MailServerPassword(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServer.Password];
        }

        public static string IdentitySenderAddress(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServer.IdentitySenderAddress];
        }

        public static bool MailServerNeedsAuthentication(this IConfiguration config)
        {
            return !(string.IsNullOrWhiteSpace(config.MailServerUser()) ||
                string.IsNullOrWhiteSpace(config.MailServerPassword()));
        }

        public static string MailServerFakeHost(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServerFake.Host];
        }

        public static int MailServerFakePort(this IConfiguration config)
        {
            return Convert.ToInt32(config[ConfigurationConstants.MailServerFake.Port]);
        }

        public static bool MailServerFakeUseSsl(this IConfiguration config)
        {
            return Convert.ToBoolean(config[ConfigurationConstants.MailServerFake.UseSsl]);
        }

        public static string MailServerFakePassword(this IConfiguration config)
        {
            return config[ConfigurationConstants.MailServerFake.Password];
        }

        public static bool MailServerFakeNeedsAuthentication(this IConfiguration config)
        {
            return !(string.IsNullOrWhiteSpace(config.MailServerFakePassword()));
        }

        public static HostedServiceActivationState DatabaseMigrationJob(this IConfiguration config)
        {
            return Enum.Parse<HostedServiceActivationState>(config[ConfigurationConstants.HostedServices.DatabaseMigration]);
        }

        public static HostedServiceActivationState DatabaseSeedingJob(this IConfiguration config)
        {
            return Enum.Parse<HostedServiceActivationState>(config[ConfigurationConstants.HostedServices.DatabaseSeeding]);
        }

        public static HostedServiceActivationState TestMailSendingJob(this IConfiguration config)
        {
            return Enum.Parse<HostedServiceActivationState>(config[ConfigurationConstants.HostedServices.TestMailSending]);
        }

        public static string ConnectionStringDefaultConnectionPassword(this IConfiguration config)
        {
            return config[ConfigurationConstants.Secrets.ConnectionStringDefaultConnectionPassword];
        }
        public static string TemplateDataStoragePath(this IConfiguration config)
        {
            return config[ConfigurationConstants.TemplateDataStorage.Path];
        }

        public static string TemplateZipPath(this IConfiguration config, Guid templateId)
        {
            var path = config.TemplateDataStoragePath();
            var templateFilename = $"{templateId}.zip";
            var filepath = path + "/" + templateFilename;
            return filepath;
        }

        public static string TemplateRootPath(this IConfiguration config, Guid templateId)
        {
            var path = config.TemplateDataStoragePath();
            var folderPath = $"{path}/{templateId}";
            return folderPath;
        }
    }

    public static class ConfigurationConstants
    {
        public static class Secrets
        {
            public const string ConnectionStringDefaultConnectionPassword =
                nameof(Secrets) + ":" + nameof(ConnectionStringDefaultConnectionPassword);
        }

        public static class SmsProvider
        {
            public const string ApiKey = nameof(SmsProvider) + ":" + nameof(ApiKey);
        }

        public static class MailServer
        {
            public const string Host = nameof(MailServer) + ":" + nameof(Host);
            public const string Port = nameof(MailServer) + ":" + nameof(Port);
            public const string UseSsl = nameof(MailServer) + ":" + nameof(UseSsl);
            public const string User = nameof(MailServer) + ":" + nameof(User);
            public const string Password = nameof(MailServer) + ":" + nameof(Password);
            public const string IdentitySenderAddress = nameof(MailServer) + ":" + nameof(IdentitySenderAddress);
        }

        public static class MailServerFake
        {
            public const string Host = nameof(MailServerFake) + ":" + nameof(Host);
            public const string Port = nameof(MailServerFake) + ":" + nameof(Port);
            public const string UseSsl = nameof(MailServerFake) + ":" + nameof(UseSsl);
            public const string User = nameof(MailServerFake) + ":" + nameof(User);
            public const string Password = nameof(MailServerFake) + ":" + nameof(Password);
        }

        public static class HostedServices
        {
            public const string DatabaseMigration = nameof(HostedServices) + ":" + nameof(DatabaseMigration);
            public const string DatabaseSeeding = nameof(HostedServices) + ":" + nameof(DatabaseSeeding);
            public const string TestMailSending = nameof(HostedServices) + ":" + nameof(TestMailSending);
        }
        public static class TemplateDataStorage
        {
            public const string Path = nameof(TemplateDataStorage) + ":" + nameof(Path);
        }        
    }
    public enum HostedServiceActivationState
    {
        enabled,
        disabled
    }
}
