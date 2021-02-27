using System;
using System.IO;

namespace WebmilioCommons.Sandbox.Networking
{
    public class FakeBinaryWriter : BinaryWriter
    {
        public override void Write(byte value)
        {
            Console.WriteLine("WS: {0}", value);

            base.Write(value);
        }

        public override void Write(short value)
        {
            Console.WriteLine("WS: {0}", value);

            base.Write(value);
        }
    }
}