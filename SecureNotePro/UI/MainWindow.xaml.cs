using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SecureNotePro.Models;
using SecureNotePro.Protection;

namespace SecureNotePro.UI
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Note> notes = new ObservableCollection<Note>();
        private Note? currentNote;
        private LicenseInfo licenseInfo;
        private DispatcherTimer trialTimer;

        // PROTECTION: Very restrictive free version limits
        private const int MaxNotesForFreeUsers = 3;  // Only 3 notes!
        private const int MaxSavesForFreeUsers = 10; // Only 10 saves!

        public MainWindow()
        {
            InitializeComponent();

            // PROTECTION: Runtime anti-debugging checks
            AntiDebug.CheckDebuggerPresence();

            // PROTECTION: Code integrity verification
            IntegrityCheck.VerifyCodeIntegrity();

            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Load trial and license information
            licenseInfo = TrialManager.GetLicenseInfo();

            // Update UI based on license status
            UpdateLicenseUI();

            // Initialize with a sample note
            notes.Add(new Note
            {
                Title = "Welcome to SecureNote Pro!",
                Content = "🚨 TRIAL VERSION: You have 2 MINUTES to test all features!\n\n" +
                         "After 2 minutes, you must activate a license key.\n\n" +
                         "Free version limits:\n" +
                         "- Only 3 notes maximum\n" +
                         "- Only 10 saves total\n" +
                         "- No premium features\n\n" +
                         "Activate now to get unlimited access!"
            });

            NotesListBox.ItemsSource = notes;

            // PROTECTION: Start trial countdown timer
            StartTrialCountdownTimer();
        }

        /// <summary>
        /// PROTECTION: Timer that updates trial countdown every second
        /// </summary>
        private void StartTrialCountdownTimer()
        {
            trialTimer = new DispatcherTimer();
            trialTimer.Interval = TimeSpan.FromSeconds(1);
            trialTimer.Tick += (s, e) =>
            {
                // Re-fetch license info
                var freshInfo = TrialManager.GetLicenseInfo();

                // Update UI
                if (freshInfo.IsLicensed)
                {
                    TrialInfoText.Text = "Licensed Version - All Features Unlocked!";
                    trialTimer.Stop();
                    return;
                }

                if (freshInfo.IsTrialActive && freshInfo.TrialDaysRemaining > 0)
                {
                    // Calculate time remaining
                    TimeSpan elapsed = DateTime.UtcNow - freshInfo.TrialStartDate;
                    int totalSecondsRemaining = (2 * 60) - (int)elapsed.TotalSeconds;

                    if (totalSecondsRemaining > 0)
                    {
                        int minutesRemaining = totalSecondsRemaining / 60;
                        int secondsRemaining = totalSecondsRemaining % 60;
                        TrialInfoText.Text = $"⏱️ Trial expires in {minutesRemaining}:{secondsRemaining:D2}";

                        // PROTECTION: Warning when time is running out
                        if (totalSecondsRemaining == 60)
                        {
                            MessageBox.Show(
                                "⚠️ TRIAL EXPIRING SOON!\n\n" +
                                "Only 1 MINUTE remaining in your trial!\n\n" +
                                "Activate a license key now to keep using premium features.",
                                "Trial Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                        }
                        else if (totalSecondsRemaining == 30)
                        {
                            MessageBox.Show(
                                "🚨 TRIAL EXPIRING!\n\n" +
                                "Only 30 SECONDS remaining!\n\n" +
                                "Click 'Activate License' now!",
                                "Trial Warning",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                        }
                    }
                    else
                    {
                        // Trial just expired!
                        TrialInfoText.Text = "⛔ Trial EXPIRED - Activate now!";
                        DisablePremiumFeatures();

                        MessageBox.Show(
                            "⛔ TRIAL EXPIRED!\n\n" +
                            "Your 2-minute trial has ended.\n\n" +
                            "Premium features are now locked.\n" +
                            "Free version limits apply:\n" +
                            "- Maximum 3 notes\n" +
                            "- Maximum 10 saves\n\n" +
                            "Activate a license key to unlock all features!",
                            "Trial Expired",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop
                        );

                        trialTimer.Stop();
                    }
                }
                else
                {
                    TrialInfoText.Text = "⛔ Trial EXPIRED - Activate now!";
                    trialTimer.Stop();
                }
            };

            trialTimer.Start();
        }

        private void UpdateLicenseUI()
        {
            // PROTECTION: Verify license through multiple checks
            bool isLicensed = LicenseValidator.ValidateLicense(licenseInfo.LicenseKey ?? "");

            if (isLicensed)
            {
                LicenseStatusText.Text = ObfuscatedStrings.DecryptString(ObfuscatedStrings.LicensedStatus);
                TrialInfoText.Text = "Licensed Version - All Features Unlocked!";
                EnablePremiumFeatures();
            }
            else if (licenseInfo.IsTrialActive && licenseInfo.TrialDaysRemaining > 0)
            {
                LicenseStatusText.Text = "[TRIAL VERSION]";
                int minutesRemaining = licenseInfo.TrialDaysRemaining;
                int seconds = (int)((DateTime.UtcNow - licenseInfo.TrialStartDate).TotalSeconds % 60);
                TrialInfoText.Text = $"⏱️ Trial expires in {minutesRemaining} minute(s) {60 - seconds} second(s)";
                EnablePremiumFeatures(); // Trial users get premium features
            }
            else
            {
                LicenseStatusText.Text = "[FREE VERSION]";
                TrialInfoText.Text = licenseInfo.TrialDaysRemaining <= 0 ? "⛔ Trial EXPIRED - Activate now!" : "No active trial";
                DisablePremiumFeatures();
            }
        }

        private void EnablePremiumFeatures()
        {
            BtnTags.IsEnabled = true;
            BtnFavorite.IsEnabled = true;
            BtnCategory.IsEnabled = true;
            BtnSearch.IsEnabled = true;
            BtnExport.IsEnabled = true;

            BtnTags.Content = "🏷️ Tags";
            BtnFavorite.Content = "⭐ Favorite";
            BtnCategory.Content = "📁 Categories";
            BtnSearch.Content = "🔍 Search";
            BtnExport.Content = "📊 Export";
        }

        private void DisablePremiumFeatures()
        {
            BtnTags.IsEnabled = false;
            BtnFavorite.IsEnabled = false;
            BtnCategory.IsEnabled = false;
            BtnSearch.IsEnabled = false;
            BtnExport.IsEnabled = false;

            BtnTags.Content = "🏷️ Tags (PRO)";
            BtnFavorite.Content = "⭐ Favorite (PRO)";
            BtnCategory.Content = "📁 Categories (PRO)";
            BtnSearch.Content = "🔍 Search (PRO)";
            BtnExport.Content = "📊 Export (PRO)";
        }

        private void BtnNewNote_Click(object sender, RoutedEventArgs e)
        {
            // PROTECTION: Check document limit for free users
            if (!IsUserLicensed() && notes.Count >= MaxNotesForFreeUsers)
            {
                MessageBox.Show(
                    $"🔒 Free Version Limit Reached\n\n" +
                    $"You have reached the maximum of {MaxNotesForFreeUsers} notes for free users.\n\n" +
                    $"Activate a license to create unlimited notes!",
                    "Upgrade Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var newNote = new Note { Title = "New Note", Content = "" };
            notes.Add(newNote);
            NotesListBox.SelectedItem = newNote;
            StatusText.Text = $"New note created ({notes.Count}/{(IsUserLicensed() ? "∞" : MaxNotesForFreeUsers.ToString())})";
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListBox.SelectedItem is Note note)
            {
                currentNote = note;
                TitleTextBox.Text = note.Title;
                ContentTextBox.Text = note.Content;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentNote != null)
            {
                // PROTECTION: Check save limit for free users
                if (!IsUserLicensed())
                {
                    int totalSaves = UsageTracker.GetTotalSaveCount();
                    if (totalSaves >= MaxSavesForFreeUsers)
                    {
                        MessageBox.Show(
                            $"🔒 Free Version Save Limit Reached\n\n" +
                            $"You have used all {MaxSavesForFreeUsers} saves available in the free version.\n\n" +
                            $"Activate a license for unlimited saves!",
                            "Upgrade Required",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        return;
                    }

                    // Increment save counter
                    UsageTracker.IncrementSaveCount();
                    int remaining = MaxSavesForFreeUsers - (totalSaves + 1);

                    currentNote.Title = TitleTextBox.Text;
                    currentNote.Content = ContentTextBox.Text;
                    currentNote.Modified = DateTime.Now;
                    NotesListBox.Items.Refresh();
                    StatusText.Text = $"Note saved ({remaining} saves remaining)";
                }
                else
                {
                    // Licensed users - unlimited saves
                    currentNote.Title = TitleTextBox.Text;
                    currentNote.Content = ContentTextBox.Text;
                    currentNote.Modified = DateTime.Now;
                    NotesListBox.Items.Refresh();
                    StatusText.Text = $"Note saved at {DateTime.Now:HH:mm:ss}";
                }
            }
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            var activationWindow = new ActivationWindow(licenseInfo);
            if (activationWindow.ShowDialog() == true)
            {
                licenseInfo = TrialManager.GetLicenseInfo();
                UpdateLicenseUI();
                MessageBox.Show(
                    ObfuscatedStrings.DecryptString(ObfuscatedStrings.ActivationSuccess),
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        // Premium Feature Handlers
        private void BtnTags_Click(object sender, RoutedEventArgs e)
        {
            if (!FeatureGate.CheckFeatureAccess("tags")) return;

            if (currentNote != null)
            {
                string tags = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter tags (comma-separated):",
                    "Add Tags",
                    currentNote.Tags ?? ""
                );
                currentNote.Tags = tags;
                StatusText.Text = "Tags updated successfully";
            }
        }

        private void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (!FeatureGate.CheckFeatureAccess("favorite")) return;

            if (currentNote != null)
            {
                currentNote.IsFavorite = !currentNote.IsFavorite;
                StatusText.Text = currentNote.IsFavorite ? "Added to favorites" : "Removed from favorites";
            }
        }

        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!FeatureGate.CheckFeatureAccess("category")) return;

            if (currentNote != null)
            {
                string category = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter category:",
                    "Set Category",
                    currentNote.Category ?? ""
                );
                currentNote.Category = category;
                StatusText.Text = "Category updated successfully";
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!FeatureGate.CheckFeatureAccess("search")) return;

            string searchTerm = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter search term:",
                "Search Notes"
            );

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var results = notes.Where(n =>
                    n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                MessageBox.Show(
                    $"Found {results.Count} note(s) matching '{searchTerm}'",
                    "Search Results",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!FeatureGate.CheckFeatureAccess("export")) return;

            MessageBox.Show(
                $"Exporting {notes.Count} notes to formats:\n\n" +
                "✓ PDF Export\n" +
                "✓ HTML Export\n" +
                "✓ Markdown Export\n" +
                "✓ JSON Export\n\n" +
                "Export completed successfully!",
                "Export Notes",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// PROTECTION: Check if user has valid license or active trial
        /// </summary>
        private bool IsUserLicensed()
        {
            // Re-fetch to prevent memory patching
            var freshInfo = TrialManager.GetLicenseInfo();

            // Check license first
            if (freshInfo.IsLicensed)
            {
                return LicenseValidator.ValidateLicense(freshInfo.LicenseKey ?? "");
            }

            // Check trial
            if (freshInfo.IsTrialActive && freshInfo.TrialDaysRemaining > 0)
            {
                return true;
            }

            return false;
        }
    }
}
