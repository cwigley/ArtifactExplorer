namespace Simple.ArtifactExplorer.ViewModels
{
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Microsoft.Build.Construction;

    using Simple.ArtifactExplorer.Properties;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly ProjectInSolution projectInSolution;

        public ProjectViewModel(ProjectInSolution projectInSolution)
        {
            this.projectInSolution = projectInSolution;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name => Path.GetFileNameWithoutExtension(this.projectInSolution.AbsolutePath);

        public string FilePath => this.projectInSolution.AbsolutePath;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}