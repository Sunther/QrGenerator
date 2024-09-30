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

        //svgCode.CreateBasicFile(
        //    args[0],
        //    args[1],
        //    new Bitmap(args[2]));
        
        svgCode.CreateWiFiFile(
            args[0],
            args[1],
            args[2],
            bitmap: new Bitmap(args[3]));

        ExplorerManagement.OpenFolderContainingFile(args[3]);
    }
}
