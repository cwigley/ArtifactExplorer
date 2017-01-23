namespace Simple.ArtifactExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    using Microsoft.Build.Construction;

    using Simple.ArtifactExplorer.Domain;

    public class SolutionViewModel
    {
        private readonly BuildSolution solution;

        public SolutionViewModel(BuildSolution solution)
        {
            ObservableCollection<ProjectViewModel> p = new ObservableCollection<ProjectViewModel>();
            this.solution = solution;
            foreach (ProjectInSolution projectInSolution in this.solution.ProjectsInSolution)
            {
                p.Add(new ProjectViewModel(projectInSolution));
            }

            this.Projects = CollectionViewSource.GetDefaultView(p);
        }

        public string Name => this.solution.Name;

        public string FilePath {get {
                Uri path = new Uri(this.solution.FullPath);
                return path.LocalPath;
            } }

        public ICollectionView Projects { get; set; }
    }
}