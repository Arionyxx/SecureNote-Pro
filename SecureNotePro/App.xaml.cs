using System.Windows;

namespace SecureNotePro
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // PROTECTION: Early anti-debugging check on application startup
            Protection.AntiDebug.PerformStartupChecks();
        }
    }
}