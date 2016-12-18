﻿using System;
using Leak.Common;
using Leak.Memory;

namespace Leak.Glue.Tests
{
    public class GlueSide : IDisposable
    {
        private readonly NetworkConnection connection;
        private readonly Handshake handshake;
        private readonly GlueHooks hooks;

        public GlueSide(NetworkConnection connection, Handshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;
            this.hooks = new GlueHooks();
        }

        public PeerHash Peer
        {
            get { return handshake.Remote; }
        }

        public Handshake Handshake
        {
            get { return handshake; }
        }

        public GlueInstance Build(params string[] plugins)
        {
            FileHash hash = FileHash.Random();
            GlueConfiguration configuration = new GlueConfiguration();

            foreach (string plugin in plugins)
            {
                configuration.Plugins.Add(new GlueNamedPlugin(plugin));
            }

            GlueFactory factory = new GlueFactory(new BufferedBlockFactory());
            GlueService service = factory.Create(hash, hooks, configuration);

            return new GlueInstance(service);
        }

        public GlueHooks Hooks
        {
            get { return hooks; }
        }

        public NetworkConnection Connection
        {
            get { return connection; }
        }

        public void Dispose()
        {
        }
    }
}