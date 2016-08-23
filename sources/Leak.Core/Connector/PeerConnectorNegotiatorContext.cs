﻿using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public class PeerConnectorNegotiatorContext : HandshakeNegotiatorActiveContext
    {
        private readonly FileHash hash;
        private readonly PeerConnectorConfiguration configuration;
        private readonly NetworkConnection connection;

        public PeerConnectorNegotiatorContext(FileHash hash, PeerConnectorConfiguration configuration, NetworkConnection connection)
        {
            this.hash = hash;
            this.configuration = configuration;
            this.connection = connection;
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public HandshakeOptions Options
        {
            get
            {
                HandshakeOptions options = HandshakeOptions.None;

                if (configuration.Extensions)
                {
                    options = options | HandshakeOptions.Extended;
                }

                return options;
            }
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            configuration.Callback.OnHandshake(negotiated, new PeerConnectorHandshake(handshake));
        }

        public void OnException(Exception ex)
        {
            configuration.Callback.OnException(connection, ex);
        }

        public void OnDisconnected()
        {
            configuration.Callback.OnDisconnected(connection);
        }
    }
}