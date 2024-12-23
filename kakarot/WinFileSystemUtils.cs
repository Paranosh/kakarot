using System.IO;

namespace kakarot
{
    internal static class WinFileSystemUtils
    {
        private const long NOT_EXISTS = -1;

        public static long GetFolderSize(string DirectoryPath)
        {
            long num1 = 0;
            int num2 = 0;
            if (!Directory.Exists(DirectoryPath))
                return -1;
            DirectoryInfo directoryInfo = new DirectoryInfo(DirectoryPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                num1 += file.Length;
                ++num2;
            }
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                num1 += GetFolderSize(directory.FullName);
                ++num2;
            }
            return num1 + num2 * 32;
        }

        public static long GetFileSize(string FilePath) => new FileInfo(FilePath).Length;
    }
}
