﻿using Leak.Core.Common;
using System.IO;
using System.Security.Cryptography;

namespace Leak.Core.Metafile
{
    public class MetafileDestination
    {
        private readonly MetafileContext context;

        public MetafileDestination(MetafileContext context)
        {
            this.context = context;
        }

        public byte[] ToBytes()
        {
            string path = context.Configuration.Destination;
            byte[] bytes = File.ReadAllBytes(path);

            return bytes;
        }

        public void Write(int block, byte[] data)
        {
            int offset = block * 16384;
            string path = context.Configuration.Destination;

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 16384, FileOptions.None))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(data, 0, data.Length);

                stream.Flush();
                stream.Close();
            }
        }

        public void Validate()
        {
            FileHash computed;
            string path = context.Configuration.Destination;

            using (HashAlgorithm algorithm = SHA1.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                computed = new FileHash(algorithm.ComputeHash(stream));
            }

            if (computed.Equals(context.Configuration.Hash))
            {
                context.IsCompleted = true;
                context.Callback.OnCompleted(computed);
            }
        }
    }
}