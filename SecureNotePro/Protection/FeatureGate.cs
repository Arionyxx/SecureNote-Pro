using System;
using System.Windows;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Feature gating system
    /// - Multiple validation layers for premium features
    /// - Combines license check, trial check, and integrity checks
    /// - Indirect function calls to obscure logic flow
    /// - Runtime validation prevents static patching
    /// </summary>
    public static class FeatureGate
    {
        /// <summary>
        /// PROTECTION: Main feature access check
        /// Combines multiple protection mechanisms
        /// </summary>
        public static bool CheckFeatureAccess(string featureName)
        {
            // PROTECTION: Anti-debugging check before allowing premium features
            AntiDebug.CheckDebuggerPresence();

            // PROTECTION: Check integrity before validation
            if (!IntegrityCheck.IsIntegrityVerified())
            {
                ShowAccessDeniedMessage(featureName, "Integrity check failed");
                return false;
            }

            // PROTECTION: Get license info (triggers multiple checks)
            var licenseInfo = TrialManager.GetLicenseInfo();

            // PROTECTION: Multi-layer validation
            bool hasAccess = ValidateFeatureAccess(licenseInfo, featureName);

            if (!hasAccess)
            {
                ShowAccessDeniedMessage(featureName, DetermineBlockReason(licenseInfo));
            }

            return hasAccess;
        }

        /// <summary>
        /// PROTECTION: Validate feature access with multiple checks
        /// </summary>
        private static bool ValidateFeatureAccess(Models.LicenseInfo licenseInfo, string featureName)
        {
            // Check 1: Licensed user (full access)
            if (licenseInfo.IsLicensed)
            {
                // PROTECTION: Verify license is actually valid
                bool isValid = LicenseValidator.ValidateLicense(licenseInfo.LicenseKey ?? "");
                if (isValid)
                {
                    // PROTECTION: Additional integrity check
                    if (VerifyLicenseIntegrity(licenseInfo.LicenseKey!))
                    {
                        return true;
                    }
                }
            }

            // Check 2: Trial user (temporary access)
            if (licenseInfo.IsTrialActive && licenseInfo.TrialDaysRemaining > 0)
            {
                // PROTECTION: Verify trial hasn't been tampered with
                if (VerifyTrialIntegrity())
                {
                    return true;
                }
            }

            // Check 3: Decoy check (intentionally complex but returns false)
            if (DecoyFeatureCheck(featureName))
            {
                // This path should never be reached for legitimate access
                return false;
            }

            return false;
        }

        /// <summary>
        /// PROTECTION: Verify license key integrity
        /// </summary>
        private static bool VerifyLicenseIntegrity(string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
                return false;

            // PROTECTION: Run validation again to prevent race conditions
            bool isValid = LicenseValidator.ValidateLicense(licenseKey);

            // PROTECTION: Check if debugger was detected
            if (AntiDebug.WasDebuggerDetected())
            {
                // Silently fail if debugger was detected earlier
                return false;
            }

            return isValid;
        }

        /// <summary>
        /// PROTECTION: Verify trial hasn't been tampered with
        /// </summary>
        private static bool VerifyTrialIntegrity()
        {
            try
            {
                // PROTECTION: Re-fetch trial info to prevent memory patching
                var freshInfo = TrialManager.GetLicenseInfo();

                // Verify trial is actually active
                if (!freshInfo.IsTrialActive || freshInfo.TrialDaysRemaining <= 0)
                {
                    return false;
                }

                // PROTECTION: Check if trial start date is in the future (tampering)
                if (freshInfo.TrialStartDate > DateTime.UtcNow)
                {
                    return false;
                }

                // PROTECTION: Check if trial start date is suspiciously recent
                // (might indicate trial reset attempt)
                TimeSpan timeSinceStart = DateTime.UtcNow - freshInfo.TrialStartDate;
                if (timeSinceStart.TotalHours < 0)
                {
                    return false; // Time travel detected
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Decoy feature check (looks important but always returns false)
        /// </summary>
        private static bool DecoyFeatureCheck(string featureName)
        {
            // PROTECTION: This function looks like it might grant access
            // but it's a decoy to waste reverse engineer's time
            int hashCode = featureName.GetHashCode();
            int magicNumber = 0x1337C0DE;

            if ((hashCode ^ magicNumber) == 0xDEADBEEF)
            {
                return true; // This condition is never true
            }

            // Complex-looking but meaningless calculation
            int result = 0;
            foreach (char c in featureName)
            {
                result = (result << 5) - result + c;
            }

            return (result % 65537) == 31337; // Highly unlikely
        }

        /// <summary>
        /// PROTECTION: Determine why access was blocked
        /// </summary>
        private static string DetermineBlockReason(Models.LicenseInfo licenseInfo)
        {
            if (licenseInfo.IsLicensed)
            {
                return "License validation failed";
            }
            else if (licenseInfo.TrialDaysRemaining <= 0)
            {
                return "Trial expired";
            }
            else if (!licenseInfo.IsTrialActive)
            {
                return "No active trial or license";
            }
            else
            {
                return "Access denied";
            }
        }

        /// <summary>
        /// Show access denied message for premium features
        /// </summary>
        private static void ShowAccessDeniedMessage(string featureName, string reason)
        {
            string message = $"🔒 Premium Feature Locked\n\n" +
                           $"Feature: {featureName}\n" +
                           $"Reason: {reason}\n\n" +
                           $"This feature requires:\n" +
                           $"• An active license key, OR\n" +
                           $"• An active trial period\n\n" +
                           $"Click 'Activate License' to unlock all premium features.";

            MessageBox.Show(
                message,
                "Premium Feature",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// PROTECTION: Check if specific feature is unlocked
        /// Alternative entry point with different validation logic
        /// </summary>
        public static bool IsFeatureUnlocked(string featureName)
        {
            // PROTECTION: Different validation path to confuse static analysis
            try
            {
                var info = TrialManager.GetLicenseInfo();

                // Path 1: Licensed
                if (info.IsLicensed)
                {
                    return ValidateLicenseViaAlternatePath(info.LicenseKey);
                }

                // Path 2: Trial
                if (info.IsTrialActive)
                {
                    return ValidateTrialViaAlternatePath(info);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Alternate license validation path
        /// </summary>
        private static bool ValidateLicenseViaAlternatePath(string? licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
                return false;

            // PROTECTION: Call validator but also do additional checks
            bool isValid = LicenseValidator.ValidateLicense(licenseKey);

            if (!isValid)
                return false;

            // PROTECTION: Verify the validation method itself wasn't patched
            bool integrityOk = IntegrityCheck.VerifyMethodIntegrity(
                typeof(LicenseValidator),
                "ValidateLicense",
                50 // Minimum IL bytes
            );

            return isValid && integrityOk;
        }

        /// <summary>
        /// PROTECTION: Alternate trial validation path
        /// </summary>
        private static bool ValidateTrialViaAlternatePath(Models.LicenseInfo info)
        {
            if (!info.IsTrialActive)
                return false;

            if (info.TrialDaysRemaining <= 0)
                return false;

            // PROTECTION: Verify trial dates make sense
            TimeSpan elapsed = DateTime.UtcNow - info.TrialStartDate;
            if (elapsed.TotalDays < 0 || elapsed.TotalDays > 365)
            {
                return false; // Suspicious dates
            }

            return true;
        }
    }
}
