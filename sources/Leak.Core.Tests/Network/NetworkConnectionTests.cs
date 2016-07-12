﻿using FluentAssertions;
using Leak.Core.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Tests.Network
{
    [TestFixture]
    public class NetworkConnectionTests
    {
        [Test]
        public void CanSendAndReceiveData()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                byte[] payload = { 1, 2, 3 };
                NetworkOutgoingMessage outgoing = new NetworkOutgoingMessage(payload);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Send(outgoing);
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(payload);
                }
            }
        }

        [Test]
        public void CanReceiveMessagesInParts()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                NetworkOutgoingMessage part1 = new NetworkOutgoingMessage(1, 2);
                NetworkOutgoingMessage part2 = new NetworkOutgoingMessage(3);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Send(part1);
                    passive.Receive(handler);

                    handler.Any.WaitOne(TimeSpan.FromSeconds(3));
                    active.Send(part2);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(1, 2, 3);
                }
            }
        }

        [Test]
        public void CanReceiveMessagesInMerge()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                active.Send(new NetworkOutgoingMessage(1, 2, 3, 4, 5, 6));

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(1, 2, 3);
                }

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(4, 5, 6);
                }
            }
        }

        [Test]
        public void CanHandleDisconnectedConnection()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                Socket passive = container["passive"];
                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Receive(handler);
                    passive.Shutdown(SocketShutdown.Both);
                    passive.Disconnect(false);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.IsDisconnected.Should().BeTrue();
                }
            }
        }

        [Test]
        public void CanHandleDisposedConnection()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                Socket passive = container["passive"];
                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Receive(handler);
                    passive.Dispose();

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.IsDisconnected.Should().BeTrue();
                }
            }
        }

        [Test]
        public void CanEncryptTransferredData()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                active = new NetworkConnection(active, with => with.Encryptor = new IncrementalEncryptor());
                NetworkOutgoingMessage outgoing = new NetworkOutgoingMessage(1, 2, 3);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Send(outgoing);
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(2, 3, 4);
                }
            }
        }

        [Test]
        public void CanDecryptTransferredData()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                passive = new NetworkConnection(passive, with => with.Decryptor = new DecrementalDecryptor());
                NetworkOutgoingMessage outgoing = new NetworkOutgoingMessage(2, 3, 4);

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    active.Send(outgoing);
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(1, 2, 3);
                }
            }
        }

        [Test]
        public void CanSwitchToDecryption()
        {
            using (SocketContainer container = new SocketContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                NetworkConnection active = new NetworkConnection(container["active"], NetworkConnectionDirection.Outgoing);
                NetworkConnection passive = new NetworkConnection(container["passive"], NetworkConnectionDirection.Incoming);

                active.Send(new NetworkOutgoingMessage(1, 2, 3, 5, 6, 7));

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(1, 2, 3);
                }

                passive = new NetworkConnection(passive, with => with.Decryptor = new DecrementalDecryptor());

                using (IncomingMessageHandler handler = new IncomingMessageHandler(3))
                {
                    passive.Receive(handler);

                    handler.Ready.WaitOne(TimeSpan.FromSeconds(3));
                    handler.ToBytes().Should().Equal(4, 5, 6);
                }
            }
        }

        private class IncomingMessageHandler : NetworkIncomingMessageHandler, IDisposable
        {
            private readonly int size;
            private readonly List<byte[]> parts;
            private readonly ManualResetEvent onReady;
            private readonly ManualResetEvent onAny;

            private bool isDisconnected;

            public IncomingMessageHandler(int size)
            {
                this.size = size;

                parts = new List<byte[]>();
                onReady = new ManualResetEvent(false);
                onAny = new ManualResetEvent(false);
            }

            public WaitHandle Any
            {
                get { return onAny; }
            }

            public WaitHandle Ready
            {
                get { return onReady; }
            }

            public bool IsDisconnected
            {
                get { return isDisconnected; }
            }

            public byte[] ToBytes()
            {
                byte[] data = new byte[0];

                foreach (byte[] part in parts)
                {
                    Array.Resize(ref data, part.Length + data.Length);
                    Array.Copy(part, 0, data, data.Length - part.Length, part.Length);
                }

                return data;
            }

            public void OnMessage(NetworkIncomingMessage message)
            {
                if (message.Length >= size)
                {
                    parts.Add(message.ToBytes(0, size));
                    message.Acknowledge(size);
                    onReady.Set();
                }
                else
                {
                    onAny.Set();
                    message.Continue(this);
                }
            }

            public void OnException(Exception ex)
            {
            }

            public void OnDisconnected()
            {
                isDisconnected = true;
                onReady.Set();
            }

            public void Dispose()
            {
                onReady.Dispose();
                onAny.Dispose();
            }
        }

        private class IncrementalEncryptor : NetworkConnectionEncryptor
        {
            public override byte[] Encrypt(byte[] data)
            {
                byte[] result = new byte[data.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = (byte)(data[i] + 1);

                return result;
            }
        }

        private class DecrementalDecryptor : NetworkConnectionDecryptor
        {
            public override byte[] Decrypt(byte[] data)
            {
                byte[] result = new byte[data.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = (byte)(data[i] - 1);

                return result;
            }
        }
    }
}