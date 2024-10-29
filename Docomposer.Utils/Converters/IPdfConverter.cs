namespace Docomposer.Utils.Converters
{
    public interface IPdfConverter
    {
        bool ConvertDocxToPdf(string inputDocxFile, string outputPdfFile);
    }
}