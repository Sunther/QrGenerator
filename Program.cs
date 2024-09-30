using QrGenerator.Application.Factories;
using QrGenerator.Disk;

namespace QrGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        var svgCode = QrGeneratorFactory.GetQrCodeGenerator();
        var path = "C:\\Workspaces\\Repos_Tests\\QRCode.svg";

        svgCode.CreateBasicFile(
            "The text which should be encoded.",
            path);

        ExplorerManagement.OpenFolderContainingFile(path);
    }
}
