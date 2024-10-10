using System.Drawing;
using QrGenerator.Application.Factories;
using QrGenerator.Disk;

namespace QrGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: QrGenerator <content> <output file path>");
            return;
        }

        var svgCode = QrGeneratorFactory.GetQrCodeGenerator();

        if (args.Length == 3)
        {
            svgCode.CreateBasicFile(
                args[0],
                args[1],
                args[2]);

            ExplorerManagement.OpenFolderContainingFile(args[1]);
        }
        else if (args.Length == 4)
        {
            svgCode.CreateWiFiFile(
                args[0],
                args[1],
                args[2],
                args[3]);

            ExplorerManagement.OpenFolderContainingFile(args[2]);
        }
    }
}
