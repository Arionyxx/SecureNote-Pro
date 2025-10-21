using System;

namespace SecureNotePro.Models
{
    /// <summary>
    /// Stores license and trial information
    /// </summary>
    public class LicenseInfo
    {
        public bool IsLicensed { get; set; }
        public string? LicenseKey { get; set; }
        public DateTime? ActivationDate { get; set; }
        public bool IsTrialActive { get; set; }
        public DateTime TrialStartDate { get; set; }
        public int TrialDaysRemaining { get; set; }
        public string LicenseType { get; set; } = "Free";
    }
}
