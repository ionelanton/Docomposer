using System;
using NUnit.Framework;

namespace Docomposer.Utils.Test;

[TestFixture]
public class TestBase64Encoder
{
    [Test]
    public void TestEncryptionAndDecryption()
    {
        var guid = Guid.NewGuid().ToString();

        var encodedGuid = Base64Encoder.Encode(guid);
        var decodedGuid = Base64Encoder.Decode(encodedGuid);
        
        Assert.That(guid, Is.EqualTo(decodedGuid));
    }
}