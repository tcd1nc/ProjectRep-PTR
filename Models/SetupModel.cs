
namespace PTR.Models
{
    public class SetupModel
    {
        public string Emailformat { get; set; }
        public string Domain { get; set; }
        public string RequiringCompletionEMTitle { get; set; }
        public string RequiringCompletionEMMessage { get; set; }
        public string OverdueEMTitle { get; set; }
        public string OverdueEMMessage { get; set; }
        public string IncompleteEPsEMTitle { get; set; }
        public string IncompleteEPsEMMessage { get; set; }
        public string MissingEPsEMTitle { get; set; }
        public string MissingEPsEMMessage { get; set; }
        public string MilestoneDueEMTitle { get; set; }
        public string MilestoneDueEMMessage { get; set; }
        public string SMTP { get; set; }
        public string TargetName { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public bool UseExtEMCredentials { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string EMUser { get; set; }
        public string EMPWD { get; set; }
        public bool IsBodyHtml { get; set; }
        public bool UseEmail { get; set; }
        public string Productformat { get; set; }
    }
}
