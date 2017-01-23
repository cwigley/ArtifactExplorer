namespace Simple.ArtifactExplorer
{
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Build.Evaluation;
    using Microsoft.Win32;
    using NLog;
    using Simple.ArtifactExplorer.Domain;
    using Simple.ArtifactExplorer.Properties;
    using Simple.ArtifactExplorer.ViewModels;
    using Squirrel;

    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;
        private ILogger logger = LogManager.GetCurrentClassLogger();

        public  MainWindow()
        {
            this.InitializeComponent();
            this.Title = this.Title + " " +Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            this.TextBoxBuildFile.Text = Settings.Default.MostRecentlyUsed;
            if (File.Exists(this.TextBoxBuildFile.Text))
            {
                LoadProject();
            }            
        }

        

        private async Task MainWindowAsync()
        {
            var mgr = await UpdateManager.GitHubUpdateManager("https://www.github.com/cwigley/artifactexplorer");  
            var result = await mgr.CheckForUpdate();     
            logger.Debug($"CurrentlyInstalledVersion:{result.CurrentlyInstalledVersion}");     
            logger.Debug($"FutureReleaseEntry:{result.FutureReleaseEntry}");   
            logger.Debug($"PackageDirectory:{result.PackageDirectory}");   
            logger.Debug($"ReleasesToApply:{result.ReleasesToApply}");             
        }

        private void ButtonBrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Proj files (*.proj)|*.proj|All files (.)|*.*" };

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                string fileName = fileDialog.FileName;
                this.TextBoxBuildFile.Text = fileName;
                LoadProject();
            }
        }

        private void LoadProject()
        {
            if (ProjectCollection.GlobalProjectCollection.LoadedProjects.Count > 0)
            {
                ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            }

            string projectFile = this.TextBoxBuildFile.Text;
            this.viewModel.BuildProjectFile = BuildProjectFile.Parse(projectFile);
            this.viewModel.Solutions.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            this.TreeView.ItemsSource = this.viewModel.Solutions;

            if (Settings.Default.MostRecentlyUsed != this.TextBoxBuildFile.Text)
            {
                Settings.Default.MostRecentlyUsed = this.TextBoxBuildFile.Text;
                Settings.Default.Save();
            }
        }

        private void ButtonClearFilterClick(object sender, RoutedEventArgs e)
        {
            this.TextBoxSearch.Clear();
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null) Clipboard.SetText(menuItem.Tag.ToString());
        }

        private void BuildMenuItemClick(System.Object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null) this.viewModel.BuildProject(menuItem.Tag.ToString());
        }
    }
}