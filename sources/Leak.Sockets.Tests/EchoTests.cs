﻿using Leak.Completion;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Leak.Sockets.Tests
{
    public class EchoTests
    {
        [Test]
        public async Task CanHandleRequest()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                EchoClient client = new EchoClient(factory);

                using (EchoServer server = new EchoServer(factory))
                {
                    worker.Start();
                    server.Start();

                    string payload = "abc";
                    string response = await client.Send(server.Endpoint, payload);

                    Assert.That(response, Is.EqualTo(payload));
                }
            }
        }

        [Test]
        public async Task CanHandleLongRequest()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);
                EchoClient client = new EchoClient(factory);

                using (EchoServer server = new EchoServer(factory))
                {
                    worker.Start();
                    server.Start();

                    string payload = new string('a', 256 * 1024);
                    string response = await client.Send(server.Endpoint, payload);

                    Assert.That(response, Is.EqualTo(payload));
                }
            }
        }
    }
}