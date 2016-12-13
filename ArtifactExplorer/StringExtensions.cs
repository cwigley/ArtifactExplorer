namespace Simple.ArtifactExplorer
{
    using System;

    public static class StringExtensions
    {
        public static bool Contains(this string source, string filter, StringComparison comparer)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return source.IndexOf(filter, comparer) > -1;
        }
    }
}