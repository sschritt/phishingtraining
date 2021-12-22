namespace PhishingTraining.Web.Helpers.Security
{
    public class FeatureClaims
    {
        public const string CampaignManagement = nameof(CampaignManagement);
        public const string TemplateManagement = nameof(TemplateManagement);

        public static string[] ParticipantFeatureClaims => new string[] { };
        public static string[] ManagerFeatureClaims => new[] { CampaignManagement, TemplateManagement };

        public static string[] AdminFeatureClaims => new[] { CampaignManagement, TemplateManagement };
    }
}