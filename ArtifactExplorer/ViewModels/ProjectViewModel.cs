namespace Simple.ArtifactExplorer.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;
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

        public string FilePath {get {
                Uri path = new Uri(this.projectInSolution.AbsolutePath);
                return path.LocalPath;
            } }

        public void Build()
        {
            Project project = new Project(this.FilePath);
            project.Build();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}