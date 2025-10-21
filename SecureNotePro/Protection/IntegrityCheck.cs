using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Code integrity verification
    /// - Checks if the executable has been modified
    /// - Verifies critical functions haven't been patched
    /// - Uses checksums and hashing
    /// </summary>
    public static class IntegrityCheck
    {
        private static bool integrityVerified = false;
        private static readonly byte[] expectedHashPrefix = { 0x5E, 0xC0, 0xDE }; // Example

        /// <summary>
        /// PROTECTION: Verify code integrity on startup
        /// </summary>
        public static void VerifyCodeIntegrity()
        {
            try
            {
                // Check 1: Verify assembly hasn't been modified
                if (!VerifyAssemblyIntegrity())
                {
                    HandleIntegrityViolation("Assembly Modification");
                    return;
                }

                // Check 2: Verify critical type exists and hasn't been gutted
                if (!VerifyCriticalTypes())
                {
                    HandleIntegrityViolation("Type Tampering");
                    return;
                }

                // Check 3: Check for common patching signatures
                if (DetectCommonPatches())
                {
                    HandleIntegrityViolation("Patch Detection");
                    return;
                }

                integrityVerified = true;
            }
            catch (Exception ex)
            {
                HandleIntegrityViolation($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// PROTECTION: Verify the assembly file hasn't been significantly modified
        /// </summary>
        private static bool VerifyAssemblyIntegrity()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyPath = assembly.Location;

                if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
                {
                    return false; // Can't verify if we can't find the file
                }

                // PROTECTION: Check file size is reasonable
                var fileInfo = new FileInfo(assemblyPath);

                // If file is suspiciously small, it might be a cracked stub
                if (fileInfo.Length < 10000) // 10KB minimum
                {
                    return false;
                }

                // PROTECTION: Calculate hash of the assembly
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(assemblyPath))
                {
                    byte[] hash = sha256.ComputeHash(stream);

                    // PROTECTION: We can't store the exact hash (it changes with each build)
                    // Instead, check that the hash has certain properties
                    // In a real scenario, this might be a digital signature check

                    // Simple check: hash should be 32 bytes
                    if (hash.Length != 32)
                        return false;

                    // Check hash isn't all zeros (obviously tampered)
                    bool allZeros = true;
                    foreach (byte b in hash)
                    {
                        if (b != 0)
                        {
                            allZeros = false;
                            break;
                        }
                    }

                    if (allZeros)
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Verify critical types exist and have expected methods
        /// </summary>
        private static bool VerifyCriticalTypes()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                // Check that LicenseValidator type exists
                var licenseValidatorType = assembly.GetType("SecureNotePro.Protection.LicenseValidator");
                if (licenseValidatorType == null)
                    return false;

                // Check that ValidateLicense method exists
                var validateMethod = licenseValidatorType.GetMethod("ValidateLicense",
                    BindingFlags.Public | BindingFlags.Static);
                if (validateMethod == null)
                    return false;

                // PROTECTION: Check method hasn't been replaced with a simple return true
                // We can check the IL size (a patched method returning true would be very small)
                var methodBody = validateMethod.GetMethodBody();
                if (methodBody == null || methodBody.GetILAsByteArray() == null)
                    return false;

                byte[] ilBytes = methodBody.GetILAsByteArray();

                // PROTECTION: Real validation method should have substantial IL code
                // A patched "return true" would be just a few bytes
                if (ilBytes.Length < 50) // Arbitrary threshold
                {
                    return false; // Method too small, likely patched
                }

                // Check for suspicious IL patterns (simple return true/false)
                // IL for "return true" is typically: ldc.i4.1, ret (0x17, 0x2A)
                if (ilBytes.Length < 10 &&
                    ilBytes[0] == 0x17 && // ldc.i4.1
                    ilBytes[ilBytes.Length - 1] == 0x2A) // ret
                {
                    return false; // Definitely patched to return true
                }

                if (ilBytes.Length < 10 &&
                    ilBytes[0] == 0x16 && // ldc.i4.0
                    ilBytes[ilBytes.Length - 1] == 0x2A) // ret
                {
                    return false; // Patched to return false
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PROTECTION: Detect common patching tools signatures
        /// </summary>
        private static bool DetectCommonPatches()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                // PROTECTION: Check for dnSpy debugger attributes
                var attributes = assembly.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    string attrName = attr.GetType().Name;

                    // Some patching tools leave signatures
                    if (attrName.Contains("Debuggable") ||
                        attrName.Contains("DebuggableAttribute"))
                    {
                        var debuggableAttr = attr as System.Diagnostics.DebuggableAttribute;
                        if (debuggableAttr != null)
                        {
                            // Check if JIT optimization is disabled (common in modified assemblies)
                            if (debuggableAttr.IsJITOptimizerDisabled)
                            {
                                // This might be legitimate in Debug builds
                                // but suspicious in Release builds
                                #if !DEBUG
                                return true; // Patch detected
                                #endif
                            }
                        }
                    }
                }

                return false; // No patches detected
            }
            catch
            {
                return true; // If we can't check, assume tampered
            }
        }

        /// <summary>
        /// PROTECTION: Handle integrity violation detection
        /// </summary>
        private static void HandleIntegrityViolation(string reason)
        {
            try
            {
                MessageBox.Show(
                    $"⚠️ INTEGRITY VIOLATION DETECTED!\n\n" +
                    $"Reason: {reason}\n\n" +
                    $"This is an educational integrity check.\n" +
                    $"The application detected that its code may have been modified.\n\n" +
                    $"Common modifications detected by this check:\n" +
                    $"• Assembly patching (dnSpy, ILSpy editing)\n" +
                    $"• Method replacement (return true patches)\n" +
                    $"• Binary modification\n" +
                    $"• Debug attribute injection\n\n" +
                    $"In real software protection, this might trigger:\n" +
                    $"• Immediate termination\n" +
                    $"• Silent feature corruption\n" +
                    $"• License blacklisting\n\n" +
                    $"For learning purposes, the app will continue running.",
                    "Integrity Check Failed - Educational",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
            catch
            {
                // Silently fail if we can't show UI
            }

            integrityVerified = false;
        }

        /// <summary>
        /// PROTECTION: Check if integrity was verified
        /// </summary>
        public static bool IsIntegrityVerified()
        {
            return integrityVerified;
        }

        /// <summary>
        /// PROTECTION: Runtime integrity check for specific method
        /// </summary>
        public static bool VerifyMethodIntegrity(Type type, string methodName, int minimumILSize)
        {
            try
            {
                var method = type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                    return false;

                var methodBody = method.GetMethodBody();
                if (methodBody == null)
                    return false;

                byte[] ilBytes = methodBody.GetILAsByteArray();
                if (ilBytes == null || ilBytes.Length < minimumILSize)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
