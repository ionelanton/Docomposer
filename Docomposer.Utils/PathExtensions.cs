namespace Docomposer.Utils
{
    public static class PathExtensions
    {
        public static string AsLinuxPath(this string path)
        {
            return path.Replace("\\", "/");
        }
    }
}