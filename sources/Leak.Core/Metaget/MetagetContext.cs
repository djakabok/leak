﻿using Leak.Core.Core;
using Leak.Core.Glue;
using Leak.Core.Metafile;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public class MetagetContext
    {
        private readonly GlueService glue;
        private readonly string destination;
        private readonly MetagetHooks hooks;
        private readonly MetagetConfiguration configuration;
        private readonly LeakQueue<MetagetContext> queue;
        private readonly MetafileService metafile;

        private MetamineBitfield metamine;

        public MetagetContext(GlueService glue, string destination, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            this.glue = glue;
            this.destination = destination;
            this.hooks = hooks;
            this.configuration = configuration;

            metafile = CreateMetafile();
            queue = new LeakQueue<MetagetContext>(this);
        }

        private MetafileService CreateMetafile()
        {
            string path = destination + ".metainfo";
            MetafileHooks hooks = new MetafileHooks
            {
                OnMetafileVerified = data =>
                {
                    this.hooks.CallMetadataDiscovered(data.Hash, data.Metainfo);
                }
            };

            return new MetafileService(glue.Hash, path, hooks);
        }

        public MetamineBitfield Metamine
        {
            get { return metamine; }
            set { metamine = value; }
        }

        public MetagetConfiguration Configuration
        {
            get { return configuration; }
        }

        public LeakQueue<MetagetContext> Queue
        {
            get { return queue; }
        }

        public MetafileService Metafile
        {
            get { return metafile; }
        }

        public MetagetHooks Hooks
        {
            get { return hooks; }
        }

        public GlueService Glue
        {
            get { return glue; }
        }
    }
}