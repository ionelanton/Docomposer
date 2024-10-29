using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Clippit;
using DocumentFormat.OpenXml.Packaging;
using Clippit.Word;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Gif;

namespace Docomposer.Utils.Converters
{
    public class DocxConverter
    {
        public static string ConvertToHtml(string filePath)
        {
            var fi = new FileInfo(filePath);
            var byteArray = File.ReadAllBytes(fi.FullName);
            var outputDirectory = Directory.GetCurrentDirectory();

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (var wDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var destFileName = new FileInfo(fi.Name.Replace(".docx", ".html"));
                    if (!string.IsNullOrEmpty(outputDirectory))
                    {
                        var di = new DirectoryInfo(outputDirectory);
                        if (!di.Exists)
                        {
                            throw new OpenXmlPowerToolsException("Output directory does not exist");
                        }

                        destFileName = new FileInfo(Path.Combine(di.FullName, destFileName.Name));
                    }

                    var pageTitle = fi.FullName;
                    var part = wDoc.CoreFilePropertiesPart;
                    if (part != null)
                    {
                        pageTitle = (string) Enumerable.FirstOrDefault(part.GetXDocument().Descendants(DC.title)) ??
                                    fi.FullName;
                    }

                    // TODO: Determine max-width from size of content area.
                    var settings = new WmlToHtmlConverterSettings()
                    {
                        //AdditionalCss = "body { margin: 1cm 1cm 1cm 1cm; max-width: 20cm; padding: 0; }",
                        AdditionalCss = "",
                        PageTitle = pageTitle,
                        FabricateCssClasses = true,
                        CssClassPrefix = "doc-reuse-",
                        RestrictToSupportedLanguages = false,
                        RestrictToSupportedNumberingFormats = false,
                        ImageHandler = imageInfo =>
                        {
                            var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                            ImageFormat imageFormat = extension switch
                            {
                                "png" => ImageFormat.Png,
                                "gif" => ImageFormat.Gif,
                                "bmp" => ImageFormat.Bmp,
                                "jpeg" => ImageFormat.Jpeg,
                                "tiff" => ImageFormat.Tiff,
                                "x-wmf" => ImageFormat.Wmf,
                                _ => null
                            };

                            // If the image format isn't one that we expect, ignore it,
                            // and don't return markup for the link.
                            if (imageFormat == null)
                                return null;

                            string base64 = null;
                            try
                            {
                                using (var ms = new MemoryStream())
                                {
                                    //imageInfo.Bitmap.Save(ms, imageFormat);
                                    if (imageFormat.Equals(ImageFormat.Png))
                                        imageInfo.Image.SaveAsPng(ms);
                                    if (imageFormat.Equals(ImageFormat.Gif))
                                        imageInfo.Image.SaveAsGif(ms);
                                    if (imageFormat.Equals(ImageFormat.Bmp))
                                        imageInfo.Image.SaveAsBmp(ms);
                                    if (imageFormat.Equals(ImageFormat.Jpeg))
                                        imageInfo.Image.SaveAsJpeg(ms);
                                    if (imageFormat.Equals(ImageFormat.Tiff))
                                        imageInfo.Image.SaveAsTiff(ms);
                                    if (imageFormat.Equals(ImageFormat.Wmf))
                                        imageInfo.Image.SaveAsGif(ms);
                                    var ba = ms.ToArray();
                                    base64 = System.Convert.ToBase64String(ba);
                                }
                            }
                            catch (System.Runtime.InteropServices.ExternalException)
                            {
                                return null;
                            }

                            //var format = imageInfo.Bitmap.RawFormat;

                            var format = imageInfo.Image.Metadata.DecodedImageFormat;
                            
                            var codec = ImageCodecInfo.GetImageDecoders()
                                .First(c => format != null && format.MimeTypes.Contains(c.MimeType));
                            var mimeType = codec.MimeType;

                            var imageSource = $"data:{mimeType};base64,{base64}";

                            var img = new XElement(Xhtml.img,
                                new XAttribute(NoNamespace.src, imageSource),
                                imageInfo.ImgStyleAttribute,
                                imageInfo.AltText != null ? new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                            return img;
                        }
                    };
                    var htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);

                    // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
                    // we are using HTML5.
                    var html = new XDocument(new XDocumentType("html", null, null, null), htmlElement);

                    // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
                    // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities will not be serialized properly.

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);
                    //File.WriteAllText(destFileName.FullName, htmlString, Encoding.UTF8);

                    return htmlString;
                }
            }
        }

        public static string ConvertToHtml(WordprocessingDocument wDoc)
        {
            // TODO: Determine max-width from size of content area.
            var settings = new WmlToHtmlConverterSettings()
            {
                //AdditionalCss = "body { margin: 1cm 1cm 1cm 1cm; max-width: 20cm; padding: 0; }",
                AdditionalCss = "",
                PageTitle = "Preview",
                FabricateCssClasses = true,
                CssClassPrefix = "doc-reuse-",
                RestrictToSupportedLanguages = false,
                RestrictToSupportedNumberingFormats = false,
                ImageHandler = imageInfo =>
                {
                    var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                    ImageFormat imageFormat = extension switch
                    {
                        "png" => ImageFormat.Png,
                        "gif" => ImageFormat.Gif,
                        "bmp" => ImageFormat.Bmp,
                        "jpeg" => ImageFormat.Jpeg,
                        "tiff" => ImageFormat.Gif,
                        "x-wmf" => ImageFormat.Wmf,
                        _ => null
                    };

                    // If the image format isn't one that we expect, ignore it,
                    // and don't return markup for the link.
                    if (imageFormat == null)
                        return null;

                    string base64 = null;
                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            //imageInfo.Bitmap.Save(ms, imageFormat);
                            if (imageFormat.Equals(ImageFormat.Png))
                                imageInfo.Image.SaveAsPng(ms);
                            if (imageFormat.Equals(ImageFormat.Gif))
                                imageInfo.Image.SaveAsGif(ms);
                            if (imageFormat.Equals(ImageFormat.Bmp))
                                imageInfo.Image.SaveAsBmp(ms);
                            if (imageFormat.Equals(ImageFormat.Jpeg))
                                imageInfo.Image.SaveAsJpeg(ms);
                            if (imageFormat.Equals(ImageFormat.Tiff))
                                imageInfo.Image.SaveAsTiff(ms);
                            if (imageFormat.Equals(ImageFormat.Wmf))
                                imageInfo.Image.SaveAsGif(ms);
                            var ba = ms.ToArray();
                            base64 = System.Convert.ToBase64String(ba);
                        }
                    }
                    catch (System.Runtime.InteropServices.ExternalException)
                    {
                        return null;
                    }

                    //var format = imageInfo.Bitmap.RawFormat;
                    var format = imageInfo.Image.Metadata.DecodedImageFormat;
                    var codec = ImageCodecInfo.GetImageDecoders()
                        .First(c => format != null && format.MimeTypes.Contains(c.MimeType));
                    var mimeType = codec.MimeType;

                    var imageSource = $"data:{mimeType};base64,{base64}";

                    var img = new XElement(Xhtml.img,
                        new XAttribute(NoNamespace.src, imageSource),
                        imageInfo.ImgStyleAttribute,
                        imageInfo.AltText != null ? new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                    return img;
                }
            };
            var htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);

            // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
            // we are using HTML5.
            var html = new XDocument(new XDocumentType("html", null, null, null), htmlElement);

            // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
            // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
            // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
            // for detailed explanation.
            //
            // If you further transform the XML tree returned by ConvertToHtmlTransform, you
            // must do it correctly, or entities will not be serialized properly.

            var htmlString = html.ToString(SaveOptions.DisableFormatting);
            //File.WriteAllText(destFileName.FullName, htmlString, Encoding.UTF8);

            return htmlString;
        }
    }
}