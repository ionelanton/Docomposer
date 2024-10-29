using System;
using NUnit.Framework;

namespace Docomposer.Utils.Test;


[TestFixture]
public class TestFileSystemHandler
{
    [Ignore("Manual test")]
    [Test]
    public void TestDocExists()
    {
        const string path = $@"C:\Users\johnny\AppData\Local\Docomposer\Documents\2\Documents";
        const string documentName = "Document 3";

        var documentIsOpen = new FileSystemHandler().IsWordDocumentOpen(path, documentName);
        
        Assert.True(documentIsOpen);
    }
}