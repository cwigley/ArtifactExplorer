namespace Simple.ArtifactExplorer.Domain
{
    public class BuildItem
    {
        public string FinalInclude { get; set; }

        public string Identity { get; set; }

        public string Include { get; set; }

        public bool IsImported { get; set; }
    }
}