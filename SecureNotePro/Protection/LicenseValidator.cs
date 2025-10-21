using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Multi-layered license validation
    /// - Custom checksum algorithm
    /// - Character encoding verification
    /// - Split validation logic across multiple methods
    /// - Decoy/dummy validation functions to confuse reverse engineers
    /// </summary>
    public static class LicenseValidator
    {
        // PROTECTION: Obfuscated constants used in validation
        private const int MagicNumber1 = 0x5EC0DE;
        private const int MagicNumber2 = 0xC0FFEE;
        private const string ValidationSalt = "S3cur3N0t3Pr0";

        /// <summary>
        /// Main entry point for license validation
        /// PROTECTION: Calls multiple validation functions to make patching harder
        /// </summary>
        public static bool ValidateLicense(string licenseKey)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
                return false;

            // PROTECTION: Anti-debugging check before validation
            AntiDebug.CheckDebuggerPresence();

            // PROTECTION: Multiple validation checks that all must pass
            bool check1 = ValidateFormat(licenseKey);
            bool check2 = ValidateChecksum(licenseKey);
            bool check3 = ValidateAdvancedAlgorithm(licenseKey);
            bool check4 = ValidateDecoy1(licenseKey); // Decoy function
            bool check5 = ValidateSignature(licenseKey);

            // PROTECTION: Complex boolean logic to obscure the actual validation
            return check1 && check2 && check3 && !check4 && check5;
        }

        /// <summary>
        /// PROTECTION: Format validation - license must be in correct format
        /// Format: XXXX-XXXX-XXXX-XXXX (16 characters + 3 dashes)
        /// </summary>
        private static bool ValidateFormat(string key)
        {
            if (key.Length != 19)
                return false;

            var parts = key.Split('-');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (part.Length != 4)
                    return false;

                // Must contain alphanumeric characters
                if (!part.All(c => char.IsLetterOrDigit(c)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// PROTECTION: Checksum validation using custom algorithm
        /// The last 4 characters encode a checksum of the first 12 characters
        /// </summary>
        private static bool ValidateChecksum(string key)
        {
            string keyWithoutDashes = key.Replace("-", "");

            if (keyWithoutDashes.Length != 16)
                return false;

            string dataSection = keyWithoutDashes.Substring(0, 12);
            string checksumSection = keyWithoutDashes.Substring(12, 4);

            // PROTECTION: Custom checksum calculation
            int calculatedChecksum = CalculateChecksum(dataSection);
            string expectedChecksum = ConvertChecksumToString(calculatedChecksum);

            return checksumSection.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// PROTECTION: Calculate checksum using XOR and magic numbers
        /// </summary>
        private static int CalculateChecksum(string data)
        {
            int checksum = MagicNumber1;

            for (int i = 0; i < data.Length; i++)
            {
                checksum ^= (data[i] << (i % 8));
                checksum = (checksum << 1) | (checksum >> 31); // Rotate left
                checksum ^= (i * MagicNumber2);
            }

            return checksum & 0xFFFF; // Keep only 16 bits
        }

        /// <summary>
        /// PROTECTION: Convert checksum number to base-36 string (4 characters)
        /// </summary>
        private static string ConvertChecksumToString(int checksum)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] result = new char[4];

            for (int i = 3; i >= 0; i--)
            {
                result[i] = chars[checksum % 36];
                checksum /= 36;
            }

            return new string(result);
        }

        /// <summary>
        /// PROTECTION: Advanced validation using salted hash
        /// Verifies that the license key's first 8 characters match expected pattern
        /// </summary>
        private static bool ValidateAdvancedAlgorithm(string key)
        {
            string keyWithoutDashes = key.Replace("-", "");
            string prefix = keyWithoutDashes.Substring(0, 8);

            // PROTECTION: The prefix must encode certain information
            int prefixValue = 0;
            foreach (char c in prefix)
            {
                if (char.IsDigit(c))
                    prefixValue += (c - '0');
                else
                    prefixValue += (char.ToUpper(c) - 'A' + 10);
            }

            // PROTECTION: Prefix sum must be divisible by 7 (magic validation rule)
            return (prefixValue % 7) == 0;
        }

        /// <summary>
        /// PROTECTION: Signature validation using HMAC
        /// </summary>
        private static bool ValidateSignature(string key)
        {
            try
            {
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ValidationSalt)))
                {
                    string keyData = key.Replace("-", "").Substring(0, 12);
                    byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(keyData));

                    // PROTECTION: Check if hash matches expected pattern
                    int hashSum = hash.Take(4).Sum(b => b);
                    return (hashSum % 17) == 0; // Another magic validation rule
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: DECOY FUNCTION #1
        /// This function looks important but actually should return FALSE for valid keys
        /// It's designed to confuse reverse engineers
        /// </summary>
        private static bool ValidateDecoy1(string key)
        {
            // This looks like a real validation but it's inverted
            // Notice in ValidateLicense we use !check4
            if (key.Contains("INVALID") || key.Contains("FAKE"))
                return true; // Intentionally wrong

            return false; // This is actually the "valid" return
        }

        /// <summary>
        /// PROTECTION: DECOY FUNCTION #2 - Never called but looks important
        /// This exists to waste reverse engineer's time
        /// </summary>
        private static bool ValidateDecoy2(string key)
        {
            // Complex-looking but meaningless calculation
            int sum = key.Where(c => char.IsDigit(c)).Sum(c => c - '0');
            int product = key.Where(c => char.IsLetter(c)).Aggregate(1, (acc, c) => acc * (c % 7));

            return (sum * product) % 1337 == 42; // Looks suspicious on purpose
        }

        /// <summary>
        /// HELPER: Generate a valid license key (for testing purposes)
        /// In a real application, this would be server-side only
        /// </summary>
        public static string GenerateLicenseKey(string seed)
        {
            // Use seed to generate deterministic but pseudo-random data
            using (var sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(seed + ValidationSalt));

                // Convert hash to alphanumeric characters
                const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                StringBuilder dataSection = new StringBuilder(12);

                for (int i = 0; i < 12; i++)
                {
                    dataSection.Append(chars[hashBytes[i] % 36]);
                }

                // Ensure prefix validation passes (sum divisible by 7)
                string prefix = dataSection.ToString().Substring(0, 8);
                int prefixSum = 0;
                foreach (char c in prefix)
                {
                    if (char.IsDigit(c))
                        prefixSum += (c - '0');
                    else
                        prefixSum += (char.ToUpper(c) - 'A' + 10);
                }

                // Adjust last character of prefix to make sum divisible by 7
                int remainder = prefixSum % 7;
                if (remainder != 0)
                {
                    int adjustment = 7 - remainder;
                    char lastChar = dataSection[7];
                    int currentValue = char.IsDigit(lastChar) ? (lastChar - '0') : (lastChar - 'A' + 10);
                    int newValue = (currentValue + adjustment) % 36;
                    dataSection[7] = chars[newValue];
                }

                // Calculate and append checksum
                string data = dataSection.ToString();
                int checksum = CalculateChecksum(data);
                string checksumStr = ConvertChecksumToString(checksum);

                string fullKey = data + checksumStr;

                // Format with dashes
                return $"{fullKey.Substring(0, 4)}-{fullKey.Substring(4, 4)}-{fullKey.Substring(8, 4)}-{fullKey.Substring(12, 4)}";
            }
        }
    }
}
