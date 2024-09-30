using System.Diagnostics;

namespace QrGenerator.Disk;

public static class ExplorerManagement
{
    public static void OpenFolderContainingFile(string filePath)
    {
        var folderPath = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = folderPath,
            UseShellExecute = true,
            CreateNoWindow = true,
        });
    }
}
