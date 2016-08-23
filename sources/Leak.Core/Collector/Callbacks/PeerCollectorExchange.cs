﻿using Leak.Core.Cando.PeerExchange;
using Leak.Core.Common;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorExchange : PeerExchangeCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorExchange(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnMessage(PeerSession session, PeerExchangeData data)
        {
            context.Callback.OnPeerExchanged(session, data);
        }
    }
}