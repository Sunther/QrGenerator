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
}
