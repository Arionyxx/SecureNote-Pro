using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using SecureNotePro.Models;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Trial period management
    /// - Stores trial data in multiple locations (redundancy)
    /// - Registry storage with obfuscated keys
    /// - Hidden file storage with encryption
    /// - Encoded timestamps to prevent easy manipulation
    /// - Multiple validation checks
    /// </summary>
    public static class TrialManager
    {
        // PROTECTION: Obfuscated registry paths
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\SecureNotePro";
        private const string RegistryKeyInstall = "InstallDate";
        private const string RegistryKeyTrial = "TrialStatus";

        // PROTECTION: Hidden file location (multiple locations for redundancy)
        private static readonly string TrialFile1 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".snp_config"
        );

        private static readonly string TrialFile2 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "Edge", ".snp_cache"
        );

        // License file
        private static readonly string LicenseFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecureNotePro", "license.dat"
        );

        // PROTECTION: Very short trial period - forces activation quickly!
        // Only 2 minutes to test all features before requiring activation
        private const int TrialMinutes = 2;

        /// <summary>
        /// PROTECTION: Get license and trial information with multiple checks
        /// </summary>
        public static LicenseInfo GetLicenseInfo()
        {
            var info = new LicenseInfo();

            // Check 1: Look for license file first
            string? licenseKey = LoadLicenseKey();
            if (!string.IsNullOrEmpty(licenseKey))
            {
                info.LicenseKey = licenseKey;
                info.IsLicensed = LicenseValidator.ValidateLicense(licenseKey);

                if (info.IsLicensed)
                {
                    info.LicenseType = "Professional";
                    info.ActivationDate = GetLicenseActivationDate();
                    return info;
                }
            }

            // Check 2: Trial information from multiple sources
            DateTime? installDate = GetInstallDate();

            if (!installDate.HasValue)
            {
                // First run - initialize trial
                installDate = DateTime.UtcNow;
                SaveInstallDate(installDate.Value);
            }

            // PROTECTION: Calculate trial days from multiple sources and cross-validate
            DateTime trialDate1 = GetTrialDateFromRegistry() ?? installDate.Value;
            DateTime trialDate2 = GetTrialDateFromFile1() ?? installDate.Value;
            DateTime trialDate3 = GetTrialDateFromFile2() ?? installDate.Value;

            // PROTECTION: Use the earliest date (prevents trial reset)
            DateTime effectiveTrialStart = trialDate1;
            if (trialDate2 < effectiveTrialStart) effectiveTrialStart = trialDate2;
            if (trialDate3 < effectiveTrialStart) effectiveTrialStart = trialDate3;

            info.TrialStartDate = effectiveTrialStart;
            TimeSpan elapsed = DateTime.UtcNow - effectiveTrialStart;

            // PROTECTION: Calculate remaining minutes (not days!)
            int minutesElapsed = (int)elapsed.TotalMinutes;
            int minutesRemaining = Math.Max(0, TrialMinutes - minutesElapsed);

            // Store in TrialDaysRemaining for compatibility (represents minutes now)
            info.TrialDaysRemaining = minutesRemaining;
            info.IsTrialActive = minutesRemaining > 0;

            return info;
        }

        /// <summary>
        /// PROTECTION: Save license key to encrypted file
        /// </summary>
        public static bool SaveLicenseKey(string licenseKey)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LicenseFile)!);

                // PROTECTION: Encrypt license key before saving
                string encrypted = EncryptData(licenseKey);
                File.WriteAllText(LicenseFile, encrypted);

                // Also save activation date
                SaveLicenseActivationDate(DateTime.UtcNow);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Load license key from encrypted file
        /// </summary>
        private static string? LoadLicenseKey()
        {
            try
            {
                if (!File.Exists(LicenseFile))
                    return null;

                string encrypted = File.ReadAllText(LicenseFile);
                return DecryptData(encrypted);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// PROTECTION: Get install date from registry
        /// </summary>
        private static DateTime? GetInstallDate()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(RegistryKeyInstall);
                        if (value != null && long.TryParse(value.ToString(), out long ticks))
                        {
                            return new DateTime(ticks, DateTimeKind.Utc);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// PROTECTION: Save install date to multiple locations
        /// </summary>
        private static void SaveInstallDate(DateTime date)
        {
            // Save to registry
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    // PROTECTION: Encode as ticks (harder to manipulate than readable date)
                    key.SetValue(RegistryKeyInstall, date.Ticks.ToString());

                    // PROTECTION: Also save obfuscated trial marker
                    long obfuscated = ObfuscateTimestamp(date.Ticks);
                    key.SetValue(RegistryKeyTrial, obfuscated.ToString());
                }
            }
            catch { }

            // Save to hidden file 1
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TrialFile1)!);
                string encrypted = EncryptData(date.Ticks.ToString());
                File.WriteAllText(TrialFile1, encrypted);
                File.SetAttributes(TrialFile1, FileAttributes.Hidden | FileAttributes.System);
            }
            catch { }

            // Save to hidden file 2
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TrialFile2)!);
                string encrypted = EncryptData(date.Ticks.ToString());
                File.WriteAllText(TrialFile2, encrypted);
                File.SetAttributes(TrialFile2, FileAttributes.Hidden | FileAttributes.System);
            }
            catch { }
        }

        /// <summary>
        /// PROTECTION: Get trial date from registry
        /// </summary>
        private static DateTime? GetTrialDateFromRegistry()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(RegistryKeyTrial);
                        if (value != null && long.TryParse(value.ToString(), out long obfuscated))
                        {
                            long ticks = DeobfuscateTimestamp(obfuscated);
                            return new DateTime(ticks, DateTimeKind.Utc);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// PROTECTION: Get trial date from hidden file 1
        /// </summary>
        private static DateTime? GetTrialDateFromFile1()
        {
            try
            {
                if (File.Exists(TrialFile1))
                {
                    string encrypted = File.ReadAllText(TrialFile1);
                    string decrypted = DecryptData(encrypted);
                    if (long.TryParse(decrypted, out long ticks))
                    {
                        return new DateTime(ticks, DateTimeKind.Utc);
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// PROTECTION: Get trial date from hidden file 2
        /// </summary>
        private static DateTime? GetTrialDateFromFile2()
        {
            try
            {
                if (File.Exists(TrialFile2))
                {
                    string encrypted = File.ReadAllText(TrialFile2);
                    string decrypted = DecryptData(encrypted);
                    if (long.TryParse(decrypted, out long ticks))
                    {
                        return new DateTime(ticks, DateTimeKind.Utc);
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// PROTECTION: Obfuscate timestamp to prevent easy manipulation
        /// </summary>
        private static long ObfuscateTimestamp(long ticks)
        {
            // PROTECTION: XOR with magic number and rotate bits
            long obfuscated = ticks ^ 0x5EC0DEC0FFEE;
            obfuscated = (obfuscated << 13) | (obfuscated >> (64 - 13));
            return obfuscated;
        }

        /// <summary>
        /// PROTECTION: Deobfuscate timestamp
        /// </summary>
        private static long DeobfuscateTimestamp(long obfuscated)
        {
            // PROTECTION: Reverse the obfuscation
            long rotated = (obfuscated >> 13) | (obfuscated << (64 - 13));
            return rotated ^ 0x5EC0DEC0FFEE;
        }

        /// <summary>
        /// PROTECTION: Encrypt data using AES
        /// </summary>
        private static string EncryptData(string plainText)
        {
            try
            {
                byte[] key = DeriveKey("SecureNotePro_2024");
                byte[] iv = new byte[16]; // Simple IV for demo

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var writer = new StreamWriter(cs))
                        {
                            writer.Write(plainText);
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                return plainText; // Fallback
            }
        }

        /// <summary>
        /// PROTECTION: Decrypt data using AES
        /// </summary>
        private static string DecryptData(string cipherText)
        {
            try
            {
                byte[] key = DeriveKey("SecureNotePro_2024");
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(buffer))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// PROTECTION: Derive encryption key from password
        /// </summary>
        private static byte[] DeriveKey(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Save license activation date
        /// </summary>
        private static void SaveLicenseActivationDate(DateTime date)
        {
            try
            {
                string activationFile = Path.Combine(
                    Path.GetDirectoryName(LicenseFile)!,
                    "activation.dat"
                );
                File.WriteAllText(activationFile, EncryptData(date.Ticks.ToString()));
            }
            catch { }
        }

        /// <summary>
        /// Get license activation date
        /// </summary>
        private static DateTime? GetLicenseActivationDate()
        {
            try
            {
                string activationFile = Path.Combine(
                    Path.GetDirectoryName(LicenseFile)!,
                    "activation.dat"
                );

                if (File.Exists(activationFile))
                {
                    string encrypted = File.ReadAllText(activationFile);
                    string decrypted = DecryptData(encrypted);
                    if (long.TryParse(decrypted, out long ticks))
                    {
                        return new DateTime(ticks, DateTimeKind.Utc);
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
