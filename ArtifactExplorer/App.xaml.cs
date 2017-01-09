namespace Simple.ArtifactExplorer
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Squirrel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        
        private void ApplicationDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, e.Exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        
    }
}