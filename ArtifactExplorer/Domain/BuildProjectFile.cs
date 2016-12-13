namespace Simple.ArtifactExplorer.Domain
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    public class BuildProjectFile
    {
        public BuildProjectFile()
        {
            this.Items = new ObservableCollection<BuildItem>();

            this.MsBuildSolutions = new ObservableCollection<BuildSolution>();
        }

        public ObservableCollection<BuildItem> Items { get; set; }

        public ObservableCollection<BuildSolution> MsBuildSolutions { get; set; }

        public static BuildProjectFile Parse(string projectFilePath)
        {
            return Parse(new Project(projectFilePath));
        }

        public static BuildProjectFile Parse(Project project)
        {
            if (ProjectCollection.GlobalProjectCollection.LoadedProjects.Count > 0)
            {
                ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            }

            BuildProjectFile result = new BuildProjectFile();

            if (project.AllEvaluatedItems == null)
            {
                return result;
            }

            foreach (ProjectItem projectItem in project.AllEvaluatedItems)
            {
                result.Items.Add(
                    new BuildItem
                        {
                            Identity = projectItem.ItemType,
                            Include = projectItem.UnevaluatedInclude,
                            FinalInclude = projectItem.EvaluatedInclude,
                            IsImported = projectItem.IsImported
                        });

                if (!projectItem.EvaluatedInclude.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string itemEvaluatedInclude = project.DirectoryPath + projectItem.EvaluatedInclude.Replace("/", @"\");
                string solutionFile = Path.GetFullPath(itemEvaluatedInclude);
                if (!File.Exists(solutionFile))
                {
                    throw new FileNotFoundException("Solution file not found.", solutionFile);
                }

                SolutionFile file = SolutionFile.Parse(solutionFile);
                BuildSolution solution = new BuildSolution
                                               {
                                                   FullPath = solutionFile,
                                                   Name = Path.GetFileNameWithoutExtension(solutionFile),
                                                   SolutionFile = file,
                                                   SolutionProjectItem = projectItem,
                                                   ProjectsInSolution =
                                                       new ObservableCollection<ProjectInSolution>(
                                                           file.ProjectsInOrder.Where(p => p.ProjectType != SolutionProjectType.SolutionFolder)
                                                               .OrderBy(x => x.ProjectName))
                                               };

                result.MsBuildSolutions.Add(solution);
            }

            return result;
        }
    }
}