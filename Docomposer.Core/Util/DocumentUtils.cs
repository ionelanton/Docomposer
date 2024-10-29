using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace Docomposer.Core.Util;

public static class DocumentUtils
{
    public static string ValidateWordDocument(Stream stream, string docFile = "")
    {
        var errors = $"{docFile}\n";

        using var wordprocessingDocument = WordprocessingDocument.Open(stream, true);
        try
        {
            var validator = new OpenXmlValidator();
            var count = 0;
            foreach (var error in validator.Validate(wordprocessingDocument))
            {
                errors += "\n-------------------------------------------\n";
                count++;
                errors += "Error " + count;
                errors += ", Description: " + error.Description;
                errors += "\nErrorType: " + error.ErrorType;
                errors += "\nNode: " + error.Node;
                
                if (error.Path is not null)
                {
                    errors += "\nPath: " + error.Path.XPath;
                }
                if (error.Part is not null)
                {
                    errors += "\nPart: " + error.Part.Uri;
                }
            }
        }

        catch (Exception ex)
        {
            errors +=  ex.Message;
        }

        return errors;
    }
}