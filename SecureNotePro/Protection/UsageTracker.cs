using System;
using System.IO;
using Microsoft.Win32;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Usage tracking for free version limits
    /// - Tracks save count across application runs
    /// - Stores data in multiple locations (redundancy)
    /// - Obfuscates counter to prevent easy manipulation
    /// - Validates data integrity
    /// </summary>
    public static class UsageTracker
    {
        // PROTECTION: Multiple storage locations for redundancy
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\SecureNotePro";
        private const string RegistryKeySaveCount = "UsageMetrics";
        private const string RegistryKeyVerification = "UsageCheck";

        private static readonly string UsageFile1 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".snp_usage"
        );

        private static readonly string UsageFile2 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "Edge", ".snp_usage"
        );

        /// <summary>
        /// PROTECTION: Get total save count from multiple sources
        /// Uses the highest value from all sources to prevent reset attempts
        /// </summary>
        public static int GetTotalSaveCount()
        {
            try
            {
                // PROTECTION: Get counts from all sources
                int count1 = GetSaveCountFromRegistry();
                int count2 = GetSaveCountFromFile1();
                int count3 = GetSaveCountFromFile2();

                // PROTECTION: Use the maximum value (prevents simple reset)
                int maxCount = Math.Max(count1, Math.Max(count2, count3));

                // PROTECTION: Verify the count is reasonable (not negative, not impossibly high)
                if (maxCount < 0 || maxCount > 100000)
                {
                    return 0; // Reset if tampered
                }

                return maxCount;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// PROTECTION: Increment save counter in all locations
        /// </summary>
        public static void IncrementSaveCount()
        {
            try
            {
                int currentCount = GetTotalSaveCount();
                int newCount = currentCount + 1;

                // Save to all locations
                SaveCountToRegistry(newCount);
                SaveCountToFile1(newCount);
                SaveCountToFile2(newCount);
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// PROTECTION: Reset save counter (only for licensed users or testing)
        /// </summary>
        public static void ResetSaveCount()
        {
            try
            {
                SaveCountToRegistry(0);
                SaveCountToFile1(0);
                SaveCountToFile2(0);
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// PROTECTION: Get save count from registry
        /// </summary>
        private static int GetSaveCountFromRegistry()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(RegistryKeySaveCount);
                        if (value != null && int.TryParse(value.ToString(), out int obfuscated))
                        {
                            // PROTECTION: Deobfuscate the stored value
                            int count = DeobfuscateCount(obfuscated);

                            // PROTECTION: Verify with checksum
                            object? checkValue = key.GetValue(RegistryKeyVerification);
                            if (checkValue != null && int.TryParse(checkValue.ToString(), out int checksum))
                            {
                                if (CalculateChecksum(count) == checksum)
                                {
                                    return count;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// PROTECTION: Save count to registry
        /// </summary>
        private static void SaveCountToRegistry(int count)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    // PROTECTION: Obfuscate the value before storing
                    int obfuscated = ObfuscateCount(count);
                    key.SetValue(RegistryKeySaveCount, obfuscated.ToString());

                    // PROTECTION: Store verification checksum
                    int checksum = CalculateChecksum(count);
                    key.SetValue(RegistryKeyVerification, checksum.ToString());
                }
            }
            catch { }
        }

        /// <summary>
        /// PROTECTION: Get save count from hidden file 1
        /// </summary>
        private static int GetSaveCountFromFile1()
        {
            try
            {
                if (File.Exists(UsageFile1))
                {
                    string content = File.ReadAllText(UsageFile1);
                    if (int.TryParse(content, out int obfuscated))
                    {
                        return DeobfuscateCount(obfuscated);
                    }
                }
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// PROTECTION: Save count to hidden file 1
        /// </summary>
        private static void SaveCountToFile1(int count)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UsageFile1)!);
                int obfuscated = ObfuscateCount(count);
                File.WriteAllText(UsageFile1, obfuscated.ToString());
                File.SetAttributes(UsageFile1, FileAttributes.Hidden | FileAttributes.System);
            }
            catch { }
        }

        /// <summary>
        /// PROTECTION: Get save count from hidden file 2
        /// </summary>
        private static int GetSaveCountFromFile2()
        {
            try
            {
                if (File.Exists(UsageFile2))
                {
                    string content = File.ReadAllText(UsageFile2);
                    if (int.TryParse(content, out int obfuscated))
                    {
                        return DeobfuscateCount(obfuscated);
                    }
                }
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// PROTECTION: Save count to hidden file 2
        /// </summary>
        private static void SaveCountToFile2(int count)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UsageFile2)!);
                int obfuscated = ObfuscateCount(count);
                File.WriteAllText(UsageFile2, obfuscated.ToString());
                File.SetAttributes(UsageFile2, FileAttributes.Hidden | FileAttributes.System);
            }
            catch { }
        }

        /// <summary>
        /// PROTECTION: Obfuscate counter value
        /// Makes it harder to find and modify the counter
        /// </summary>
        private static int ObfuscateCount(int count)
        {
            // PROTECTION: XOR with magic number and add offset
            long obfuscated = count ^ 0xDEADBEEF;
            obfuscated = (obfuscated << 3) | (obfuscated >> 29); // Rotate left by 3
            obfuscated += 0x1337C0DE;
            return (int)obfuscated;
        }

        /// <summary>
        /// PROTECTION: Deobfuscate counter value
        /// </summary>
        private static int DeobfuscateCount(int obfuscated)
        {
            // PROTECTION: Reverse the obfuscation
            long temp = obfuscated - 0x1337C0DE;
            temp = (temp >> 3) | (temp << 29); // Rotate right by 3
            int count = (int)(temp ^ 0xDEADBEEF);
            return count;
        }

        /// <summary>
        /// PROTECTION: Calculate checksum for verification
        /// </summary>
        private static int CalculateChecksum(int count)
        {
            // PROTECTION: Simple checksum using prime multiplication
            int checksum = count * 31;
            checksum ^= 0x5EC0DE;
            return checksum;
        }

        /// <summary>
        /// PROTECTION: Verify usage data integrity
        /// Checks if all sources agree (within tolerance)
        /// </summary>
        public static bool VerifyIntegrity()
        {
            try
            {
                int count1 = GetSaveCountFromRegistry();
                int count2 = GetSaveCountFromFile1();
                int count3 = GetSaveCountFromFile2();

                // PROTECTION: All counts should be close (allow small variance for race conditions)
                int maxDiff = Math.Max(Math.Abs(count1 - count2), Math.Abs(count2 - count3));
                maxDiff = Math.Max(maxDiff, Math.Abs(count1 - count3));

                // If difference is more than 5, something might be tampered
                return maxDiff <= 5;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Check if save limit reached
        /// </summary>
        public static bool IsSaveLimitReached(int maxSaves)
        {
            return GetTotalSaveCount() >= maxSaves;
        }

        /// <summary>
        /// PROTECTION: Get remaining saves
        /// </summary>
        public static int GetRemainingSaves(int maxSaves)
        {
            int used = GetTotalSaveCount();
            return Math.Max(0, maxSaves - used);
        }
    }
}
