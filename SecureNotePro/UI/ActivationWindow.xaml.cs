using System.Windows;
using System.Windows.Controls;
using SecureNotePro.Models;
using SecureNotePro.Protection;

namespace SecureNotePro.UI
{
    public partial class ActivationWindow : Window
    {
        private LicenseInfo licenseInfo;

        public ActivationWindow(LicenseInfo currentLicenseInfo)
        {
            InitializeComponent();
            licenseInfo = currentLicenseInfo;

            // Show current status
            if (licenseInfo.IsLicensed)
            {
                ValidationMessageText.Text = "✓ Currently licensed";
                ValidationMessageText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else if (licenseInfo.IsTrialActive)
            {
                ValidationMessageText.Text = $"ℹ️ Trial active: {licenseInfo.TrialDaysRemaining} days remaining";
                ValidationMessageText.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                ValidationMessageText.Text = "No active license or trial";
                ValidationMessageText.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void LicenseKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string key = LicenseKeyTextBox.Text.Trim();

            // Auto-format with dashes
            if (key.Length > 0 && !key.Contains("-"))
            {
                key = FormatLicenseKey(key);
                if (key != LicenseKeyTextBox.Text)
                {
                    int cursorPos = LicenseKeyTextBox.SelectionStart;
                    LicenseKeyTextBox.Text = key;
                    LicenseKeyTextBox.SelectionStart = System.Math.Min(cursorPos, key.Length);
                }
            }

            // Validate as user types
            if (key.Length == 19) // Full length with dashes
            {
                bool isValid = LicenseValidator.ValidateLicense(key);
                if (isValid)
                {
                    ValidationMessageText.Text = "✓ Valid license key";
                    ValidationMessageText.Foreground = System.Windows.Media.Brushes.Green;
                    BtnActivate.IsEnabled = true;
                }
                else
                {
                    ValidationMessageText.Text = "✗ Invalid license key";
                    ValidationMessageText.Foreground = System.Windows.Media.Brushes.Red;
                    BtnActivate.IsEnabled = false;
                }
            }
            else
            {
                ValidationMessageText.Text = "Enter license key (format: XXXX-XXXX-XXXX-XXXX)";
                ValidationMessageText.Foreground = System.Windows.Media.Brushes.Gray;
                BtnActivate.IsEnabled = false;
            }
        }

        private string FormatLicenseKey(string input)
        {
            // Remove any existing dashes
            string clean = input.Replace("-", "").ToUpper();

            if (clean.Length <= 4)
                return clean;
            else if (clean.Length <= 8)
                return clean.Substring(0, 4) + "-" + clean.Substring(4);
            else if (clean.Length <= 12)
                return clean.Substring(0, 4) + "-" + clean.Substring(4, 4) + "-" + clean.Substring(8);
            else if (clean.Length <= 16)
                return clean.Substring(0, 4) + "-" + clean.Substring(4, 4) + "-" + clean.Substring(8, 4) + "-" + clean.Substring(12);
            else
                return clean.Substring(0, 4) + "-" + clean.Substring(4, 4) + "-" + clean.Substring(8, 4) + "-" + clean.Substring(12, 4);
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            string key = LicenseKeyTextBox.Text.Trim();

            // PROTECTION: Validate license key
            bool isValid = LicenseValidator.ValidateLicense(key);

            if (isValid)
            {
                // Save license key
                bool saved = TrialManager.SaveLicenseKey(key);

                if (saved)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to save license key. Please try again.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    ObfuscatedStrings.GetInvalidLicenseMessage(),
                    "Invalid License",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
