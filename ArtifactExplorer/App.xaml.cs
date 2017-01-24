namespace Simple.ArtifactExplorer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using NLog;
    using Squirrel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private void ApplicationDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, e.Exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                logger.Debug("++OnStartUp");
                base.OnStartup(e);

                using (var mgr = await UpdateManager.GitHubUpdateManager("https://www.github.com/cwigley/artifactexplorer"))
                {
                    logger.Debug(mgr.ApplicationName);
                    var result = await mgr.UpdateApp((percent)=> 
                    {
                        logger.Debug($"Updating {percent}");
                    });
                    logger.Debug($"Version: {result.Version}");
                }
                logger.Debug("--OnStartUp");
            }
            catch(Exception exception)
            {
                logger.Error(exception, exception.Message);
            }
        }
    }
}