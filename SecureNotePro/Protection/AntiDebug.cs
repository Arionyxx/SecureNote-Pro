using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace SecureNotePro.Protection
{
    /// <summary>
    /// PROTECTION TECHNIQUE: Anti-debugging mechanisms
    /// - IsDebuggerPresent API check
    /// - Timing-based detection
    /// - CheckRemoteDebuggerPresent check
    /// - Debug breakpoint detection
    /// </summary>
    public static class AntiDebug
    {
        // PROTECTION: Windows API imports for anti-debugging
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        private static bool debugDetected = false;

        /// <summary>
        /// PROTECTION: Perform anti-debug checks on application startup
        /// </summary>
        public static void PerformStartupChecks()
        {
            // Check 1: IsDebuggerPresent
            if (IsDebuggerPresent())
            {
                debugDetected = true;
                HandleDebuggerDetection("IsDebuggerPresent");
            }

            // Check 2: Managed debugger check
            if (Debugger.IsAttached)
            {
                debugDetected = true;
                HandleDebuggerDetection("Managed Debugger");
            }

            // Check 3: Remote debugger check
            bool isRemoteDebuggerPresent = false;
            CheckRemoteDebuggerPresent(GetCurrentProcess(), ref isRemoteDebuggerPresent);
            if (isRemoteDebuggerPresent)
            {
                debugDetected = true;
                HandleDebuggerDetection("Remote Debugger");
            }

            // Check 4: Timing check
            if (DetectDebuggerByTiming())
            {
                debugDetected = true;
                HandleDebuggerDetection("Timing Analysis");
            }

            // PROTECTION: Start background monitoring thread
            StartBackgroundMonitoring();
        }

        /// <summary>
        /// PROTECTION: Runtime debugger presence check (called periodically)
        /// </summary>
        public static void CheckDebuggerPresence()
        {
            if (debugDetected)
                return; // Already detected

            // Quick check
            if (IsDebuggerPresent() || Debugger.IsAttached)
            {
                debugDetected = true;
                HandleDebuggerDetection("Runtime Check");
            }
        }

        /// <summary>
        /// PROTECTION: Timing-based debugger detection
        /// Debuggers slow down execution, so we measure timing
        /// </summary>
        private static bool DetectDebuggerByTiming()
        {
            var sw = Stopwatch.StartNew();

            // Simple operation that should be very fast
            int dummy = 0;
            for (int i = 0; i < 100; i++)
            {
                dummy += i;
            }

            sw.Stop();

            // PROTECTION: If execution took too long, debugger might be present
            // Normal execution should be well under 1ms
            return sw.ElapsedMilliseconds > 50;
        }

        /// <summary>
        /// PROTECTION: Detect hardware breakpoints by checking debug registers
        /// </summary>
        private static bool DetectHardwareBreakpoints()
        {
            // PROTECTION: Check if debug registers are in use
            // This is a simplified check - real implementation would use CONTEXT structure
            try
            {
                // If we can't perform the check due to debugger interference, assume debugger present
                var sw = Stopwatch.StartNew();
                Thread.Sleep(1);
                sw.Stop();

                // Check if sleep actually took ~1ms or if it was stepped over
                return sw.ElapsedMilliseconds > 10 || sw.ElapsedMilliseconds < 1;
            }
            catch
            {
                return true; // Exception during check indicates debugging
            }
        }

        /// <summary>
        /// PROTECTION: Background thread that continuously monitors for debuggers
        /// </summary>
        private static void StartBackgroundMonitoring()
        {
            Thread monitorThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000); // Check every 5 seconds

                    if (!debugDetected)
                    {
                        // Perform checks
                        if (IsDebuggerPresent() || Debugger.IsAttached)
                        {
                            debugDetected = true;
                            HandleDebuggerDetection("Background Monitor");
                        }

                        // Timing check
                        if (DetectDebuggerByTiming())
                        {
                            debugDetected = true;
                            HandleDebuggerDetection("Background Timing");
                        }
                    }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            monitorThread.Start();
        }

        /// <summary>
        /// PROTECTION: Handle debugger detection
        /// Educational note: In real malware this might exit or corrupt data,
        /// but for education we just show a message
        /// </summary>
        private static void HandleDebuggerDetection(string method)
        {
            // PROTECTION: For educational purposes, we show a message
            // In real protection, this might:
            // - Silently exit
            // - Corrupt license validation
            // - Display fake success messages
            // - Introduce subtle bugs

            try
            {
                MessageBox.Show(
                    $"🔍 DEBUGGER DETECTED!\n\n" +
                    $"Detection Method: {method}\n\n" +
                    $"This is an educational anti-debugging check.\n" +
                    $"The application detected that it's being debugged or analyzed.\n\n" +
                    $"In real software protection, this might trigger:\n" +
                    $"• Silent termination\n" +
                    $"• License invalidation\n" +
                    $"• Feature corruption\n" +
                    $"• Misleading behavior\n\n" +
                    $"For learning purposes, the app will continue running.",
                    "Anti-Debug Detection - Educational",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
            catch
            {
                // Silently fail if we can't show UI
            }
        }

        /// <summary>
        /// PROTECTION: Check if debugger was detected at any point
        /// </summary>
        public static bool WasDebuggerDetected()
        {
            return debugDetected;
        }

        /// <summary>
        /// PROTECTION: Anti-tamper check - verify this function itself wasn't patched
        /// This is a simple integrity check
        /// </summary>
        public static bool VerifyAntiDebugIntegrity()
        {
            // PROTECTION: In a real scenario, this would hash the function code
            // and compare against a known good hash
            // For education, we just check if the debugDetected flag seems valid

            try
            {
                bool test1 = IsDebuggerPresent();
                bool test2 = Debugger.IsAttached;

                // If both checks return false but debugDetected is true, something is wrong
                if (debugDetected && !test1 && !test2)
                {
                    // Possible tampering - the flag was set but checks show no debugger
                    // This could mean checks were patched
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
