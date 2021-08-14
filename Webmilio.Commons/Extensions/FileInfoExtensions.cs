using System.IO;

namespace Webmilio.Commons.Extensions
{
    public static class FileInfoExtensions
    {
        public static FileInfo Combine(this FileInfo fileInfo, params string[] paths) => new FileInfo(CombineString(fileInfo, paths));
        public static DirectoryInfo Combine(this DirectoryInfo directoryInfo, params string[] paths) => new DirectoryInfo(CombineString(directoryInfo, paths));

        public static string CombineString(this FileSystemInfo fsInfo, params string[] paths)
        {
            string path;

            if (paths.Length == 1)
                path = Path.Combine(fsInfo.FullName, paths[0]);
            else
                path = fsInfo.FullName + Path.Combine(paths);

            return path;
        }

#region Folder Copy
        // Expertly stolen from https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        public static void CopyTo(this DirectoryInfo from, string to) => CopyTo(from, to, false);
        public static void CopyTo(this DirectoryInfo from, string to, bool copySubDirectories) => CopyTo(from, new DirectoryInfo(to), copySubDirectories);
        public static void CopyTo(this DirectoryInfo from, DirectoryInfo to) => CopyTo(from, to, false);

        public static void CopyTo(this DirectoryInfo from, DirectoryInfo to, bool copySubDirectories)
        {
            if (!from.Exists)
                throw new DirectoryNotFoundException($"Directory `{from}` does not exist or could not be found.");

            to.Create(); // Does nothing if it already exists.

            from.EnumerateFiles().DoEnumerable(f => f.CopyTo(to.CombineString(f.Name)));

            if (copySubDirectories)
                from.EnumerateDirectories().DoEnumerable(d => d.CopyTo(to.CombineString(d.Name)));
        }
#endregion
    }
}