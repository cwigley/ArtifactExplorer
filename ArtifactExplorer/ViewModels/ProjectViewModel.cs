namespace Simple.ArtifactExplorer.ViewModels
{
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Microsoft.Build.Construction;

    using Simple.ArtifactExplorer.Properties;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly ProjectInSolution projectInSolution;

        private Visibility visible;

        public ProjectViewModel(ProjectInSolution projectInSolution)
        {
            this.projectInSolution = projectInSolution;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name => Path.GetFileNameWithoutExtension(this.projectInSolution.AbsolutePath);

        public Visibility Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (value == this.visible)
                {
                    return;
                }

                this.visible = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}