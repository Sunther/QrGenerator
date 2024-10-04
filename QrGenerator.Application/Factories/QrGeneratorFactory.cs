using QrGenerator.Application.QrCodeCoders;

namespace QrGenerator.Application.Factories;

public static class QrGeneratorFactory
{
    public static SvgQrCoder GetQrCodeGenerator()
    {
        return new SvgQrCoder();
    }
}
