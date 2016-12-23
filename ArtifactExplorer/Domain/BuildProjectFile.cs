// <copyright file="BuildProjectFile.cs" company="AirWatch by VMWare">
//  Copyright (c) 2016 AirWatch by VMWare. All rights reserved.
//  This product is protected by copyright and intellectual property laws in the United States and other countries as well as by international treaties.
//  AirWatch products may be covered by one or more patents listed at http://www.vmware.com/go/patents
// </copyright>
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

            this.BuildSolutions = new ObservableCollection<BuildSolution>();
        }

        public ObservableCollection<BuildSolution> BuildSolutions { get; set; }

        public ObservableCollection<BuildItem> Items { get; set; }

        public static BuildProjectFile Parse(string projectFilePath)
        {
            if (string.IsNullOrEmpty(projectFilePath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(projectFilePath));
            }

            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException($"Project file not found. {projectFilePath}", projectFilePath);
            }

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

                string solutionFile = ResolveSolutionFile(projectItem.EvaluatedInclude, project.DirectoryPath);
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
                                                         file.ProjectsInOrder.Where(p => p.ProjectType != SolutionProjectType.SolutionFolder).OrderBy(x => x.ProjectName))
                                             };

                result.BuildSolutions.Add(solution);
            }

            return result;
        }

        private static string ResolveSolutionFile(string evaluatedInclude, string projectDirectoryPath)
        {
            string itemEvaluatedInclude;
            string item = evaluatedInclude.Replace("/", @"\");
            if (File.Exists(item))
            {
                itemEvaluatedInclude = item;
            }
            else
            {
                itemEvaluatedInclude = projectDirectoryPath + item;
            }

            string solutionFile = Path.GetFullPath(itemEvaluatedInclude);
            return solutionFile;
        }
    }
}