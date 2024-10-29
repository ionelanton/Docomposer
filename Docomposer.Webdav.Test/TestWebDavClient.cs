using System;
using System.Collections.Generic;
using Docomposer.Data.Util;
using Docomposer.Utils;
using NUnit.Framework;
using WebDav;

namespace Docomposer.Webdav.Test
{
    [Ignore("WebDavFileHandler must be OK before testing")]
    [TestFixture]
    public class TestWebDavClient
    {

        [Test]
        public void TestWebDavClientCanUseInfinityDepth()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            var clientParams = new WebDavClientParams {BaseAddress = new Uri(ThisApp.DocReuseDocumentsPath())};
            using (var client = new WebDavClient(clientParams))
            {
                var propfindParams = new PropfindParameters
                {
                    //important: Under WebDAV Settings, I needed to set "Allow Property Queries with infinte Depth" to True.
                    Headers = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Depth", "infinity")
                    }
                };

                var result = client.Propfind("", propfindParams).Result;

                Assert.That(result.IsSuccessful, Is.True);
                Assert.That(result.Resources.Count, Is.GreaterThan(0));
            }
            
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds} ms");
        }

        [Test]
        public void TestWebDavClientConfiguredCanLockResource()
        {
            // var url = $"{ThisApp.WebDavBaseUrl()}/1/{DirName.Sections}/Section 1.docx";
            //
            // var client = new WebDavClientConfigured();
            //
            // client.LockFile(url);
            //
            // Assert.True(client.IsRessurceLocked(url));
            //
            // var users = client.GetUsersLockingResource(url);
            //
            // Assert.That(users.Count, Is.EqualTo(1));
            // Assert.That(users[0], Is.EqualTo("System"));
            //
            // client.UnlockFile(url);
            //
            // Assert.False(client.IsRessurceLocked(url));
        }
    }
}