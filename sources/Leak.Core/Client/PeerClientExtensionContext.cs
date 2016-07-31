﻿using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public interface PeerClientExtensionContext
    {
        FileHash GetHash(PeerHash peer);

        ResourceRetriever GetRetriever(PeerHash peer);

        PeerClientCallback GetCallback(PeerHash peer);
    }
}