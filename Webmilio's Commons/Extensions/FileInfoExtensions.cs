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
    }
}