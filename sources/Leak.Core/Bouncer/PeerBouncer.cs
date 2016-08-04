﻿using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Bouncer
{
    public class PeerBouncer
    {
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerStorage storage;

        public PeerBouncer(Action<PeerBouncerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerBouncerCallbackNothing();
                with.MaximumNumberOfConnections = 128;
            });

            storage = new PeerBouncerStorage(configuration);
        }

        public bool Accept(NetworkConnectionInfo connection)
        {
            return storage.Count() < configuration.MaximumNumberOfConnections;
        }

        public bool AcceptRemote(NetworkConnection connection)
        {
            if (storage.AddRemote(connection.Identifier, connection.Remote) == false)
            {
                connection.Terminate();
                return false;
            }

            return true;
        }

        public bool AcceptPeer(NetworkConnection connection, PeerHash peer)
        {
            if (storage.AddPeer(connection.Identifier, peer) == false)
            {
                connection.Terminate();
                return false;
            }

            return true;
        }

        public void AttachConnection(NetworkConnection connection)
        {
            storage.AddIdentifier(connection.Identifier);
        }

        public void ReleaseConnection(NetworkConnection connection)
        {
            storage.RemoveIdentifier(connection.Identifier);
        }
    }
}