using System;
using NUnit.Framework;

namespace Docomposer.Utils.Test;

[TestFixture]
public class TestThisApp
{
    [Test]
    public void TestGuid()
    {
        Console.WriteLine(Guid.NewGuid());
    }
}