namespace Simple.ArtifactExplorer
{
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Build.Evaluation;
    using Microsoft.Win32;

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
    }
}