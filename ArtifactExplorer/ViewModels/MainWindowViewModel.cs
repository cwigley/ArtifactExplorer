namespace Simple.ArtifactExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Logging;
    using Simple.ArtifactExplorer.Domain;
    using Simple.ArtifactExplorer.Properties;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BuildProjectFile buildProjectFile;

        private string filterText;

        public event PropertyChangedEventHandler PropertyChanged;

        public BuildProjectFile BuildProjectFile
        {
            get
            {
                return this.buildProjectFile;
            }

            set
            {
                this.buildProjectFile = value;
                if (this.Solutions == null)
                {
                    this.Solutions = CollectionViewSource.GetDefaultView(new ObservableCollection<SolutionViewModel>());
                    this.Solutions.Filter = this.FilterSolution;
                }

                ObservableCollection<SolutionViewModel> solutions =
                    this.Solutions.SourceCollection as ObservableCollection<SolutionViewModel>;
                
                solutions.Clear();

                foreach (BuildSolution buildSolution in this.buildProjectFile.BuildSolutions)
                {
                    SolutionViewModel solutionViewModel = new SolutionViewModel(buildSolution) { Projects = { Filter = this.FilterProject } };
                    solutions.Add(solutionViewModel);
                }
            }
        }

        public string FilterText
        {
            get
            {
                return this.filterText;
            }

            set
            {
                if (value == this.filterText)
                {
                    return;
                }

                this.filterText = value;
                this.OnPropertyChanged();

                if (this.Solutions==null)
                {
                    return;
                }

                foreach (SolutionViewModel solutionViewModel in this.Solutions.SourceCollection)
                {
                    solutionViewModel.Projects.Refresh();
                }

                this.Solutions.Refresh();
            }
        }

        public ICollectionView Solutions { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool FilterProject(object o)
        {
            ProjectViewModel project = o as ProjectViewModel;
            return project != null && project.Name.Contains(this.FilterText, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool FilterSolution(object o)
        {
            SolutionViewModel solution = o as SolutionViewModel;
            return !solution.Projects.IsEmpty;
        }

        internal void BuildProject(String v)
        {
            Project project = new Project(v);
                project.Build(new FileLogger());
        }
    }
}