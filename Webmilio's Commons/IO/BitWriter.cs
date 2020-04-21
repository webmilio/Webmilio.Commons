using System;
using System.IO;
using System.Threading.Tasks;

namespace Webmilio.Commons.IO
{
    public class BitWriter
    {
        public const int
            BYTE_LENGTH = 8,
            SHORT_LENGTH = BYTE_LENGTH * 2,
            INT_LENGTH = SHORT_LENGTH * 2,
            LONG_LENGTH = INT_LENGTH * 2;

        private byte currentBit = 0;


        public BitWriter(Stream baseStream, int length)
        {
            BaseStream = baseStream;
            Length = length;
        }


        public void Write(byte b)
        {
            Console.WriteLine(b);
            //BaseStream.WriteByte(b);
        }

        public void Write(ushort s)
        {
            Write((byte)(s > byte.MaxValue ? byte.MaxValue : s));

            s = (ushort)(s >> BYTE_LENGTH);
            Write((byte) s);
        }

        public void Write(int i)
        {
            Write((ushort)(i > ushort.MaxValue ? ushort.MaxValue : i));

            i = i >> SHORT_LENGTH;
            Write((ushort)i);
        }


        public void Flush() => BaseStream.Flush();
        public async Task FlushAsync() => await BaseStream.FlushAsync();


        public Stream BaseStream { get; }

        public int Length { get; set; }
    }
}