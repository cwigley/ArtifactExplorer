namespace Simple.ArtifactExplorer.Domain
{
    using System.Collections.ObjectModel;

    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    public class BuildSolution
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

        public ObservableCollection<ProjectInSolution> ProjectsInSolution { get; set; }

        public SolutionFile SolutionFile { get; set; }

        public ProjectItem SolutionProjectItem { get; set; }
    }
}