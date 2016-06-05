﻿using Leak.Core.Encoding;
using System;

namespace Leak.Core.Net
{
    public static class PeerExtendedExtensions
    {
        public static void Bencoded(this PeerExtendedConfiguration configuration, BencodedValue value)
        {
            configuration.Content = Bencoder.Encode(value);
        }

        public static void Handshake(this PeerExtendedConfiguration configuration, PeerExtendedMapping mapping)
        {
            configuration.Id = 0;
            configuration.Bencoded(mapping.Encode());
        }

        public static BencodedValue Decode(this PeerExtended message)
        {
            return Bencoder.Decode(message.Content);
        }

        public static void Handle(this PeerExtended message, PeerChannel channel, Action<PeerExtendedCallback> configurer)
        {
            PeerExtendedCallback callback = new PeerExtendedCallback();
            configurer.Invoke(callback);

            if (message.Id == 0)
            {
                callback.OnHandshake.Invoke(channel, new PeerExtendedMapping(with => with.Decode(message.Decode())));
            }

            if (message.Id > 0)
            {
                string name = callback.Mapping.FindName(message.Id);
                Action<PeerChannel, PeerExtended> handler;

                callback.OnMessage.TryGetValue(name, out handler);
                handler?.Invoke(channel, message);
            }
        }
    }
}