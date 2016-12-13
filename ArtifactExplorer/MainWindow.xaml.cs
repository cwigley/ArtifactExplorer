namespace Simple.ArtifactExplorer
{
    using System.Windows;

    using Microsoft.Build.Evaluation;

    using Simple.ArtifactExplorer.Domain;
    using Simple.ArtifactExplorer.Properties;
    using Simple.ArtifactExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            this.TextBoxBuildFile.Text = Settings.Default.MostRecentlyUsed;
        }

        private void ButtonLoadClick(object sender, RoutedEventArgs e)
        {
            if (ProjectCollection.GlobalProjectCollection.LoadedProjects.Count > 0)
            {
                ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            }

            string projectFile = this.TextBoxBuildFile.Text;
            this.viewModel.BuildProjectFile = BuildProjectFile.Parse(projectFile);

            this.TreeView.ItemsSource = this.viewModel.Solutions;

            if (Settings.Default.MostRecentlyUsed != this.TextBoxBuildFile.Text)
            {
                Settings.Default.MostRecentlyUsed = this.TextBoxBuildFile.Text;
                Settings.Default.Save();
            }
        }
    }
}