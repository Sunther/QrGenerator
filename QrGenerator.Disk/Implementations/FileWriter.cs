using System.Drawing;

namespace QrGenerator.Disk;

public class FileWriter
{
    public void CreateFile(string filePath, string content)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        File.WriteAllText(filePath, content);
    }

    public void CreateFile(string filePath, Bitmap bitmap)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        bitmap.Save(filePath);
    }
}
