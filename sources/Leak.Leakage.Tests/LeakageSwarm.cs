﻿using System;

namespace Leak.Leakage.Tests
{
    public class LeakageSwarm : IDisposable
    {
        private readonly LeakageNode sue;
        private readonly LeakageNode bob;
        private readonly LeakageNode joe;

        public LeakageSwarm(LeakageNode sue, LeakageNode bob, LeakageNode joe)
        {
            this.sue = sue;
            this.bob = bob;
            this.joe = joe;
        }

        public LeakageNode Sue
        {
            get { return sue; }
        }

        public LeakageNode Bob
        {
            get { return bob; }
        }

        public LeakageNode Joe
        {
            get { return joe; }
        }

        public void Dispose()
        {
            sue.Dispose();
            bob.Dispose();
            joe.Dispose();
        }
    }
}