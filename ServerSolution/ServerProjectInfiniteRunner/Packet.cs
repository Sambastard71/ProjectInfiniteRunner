using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServerProjectInfiniteRunner
{
    public class Packet
    {
        private MemoryStream stream;
        private BinaryWriter writer;

        private static uint packetCounter;

        private uint id;

        public uint ID
        {
            get
            {
                return id;
            }
        }

        public bool NeedsAck;
        private uint attempts;

        public void IncreaseAttempts()
        {
            attempts++;
        }

        public uint Attempts
        {
            get
            {
                return attempts;
            }
        }

        public Packet()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
            id = ++packetCounter;
            NeedsAck = false;
        }

        public Packet(params object[] elements) : this()
        {
            foreach (object element in elements)
            {
                if (element is byte)
                {
                    writer.Write((byte)element);
                }
                else if (element is float)
                {
                    writer.Write((float)element);
                }
                else if (element is uint)
                {
                    writer.Write((uint)element);
                }
                else if (element is int)
                {
                    writer.Write((int)element);
                }
                else if (element is bool)
                {
                    writer.Write((bool)element);
                }
            }
        }

        public void Write(byte[] data)
        {
            writer.Write(data);
        }

        public byte[] GetData()
        {
            return stream.ToArray();
        }
    }
}
