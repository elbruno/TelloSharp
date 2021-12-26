using TelloSharp;
 
using static TelloSharp.Messages;

namespace TelloSharp.Test
{
    public static class Program
    {
        static Messages messages = new();

        public static void Main()
        {
            TestPacketToBuffer();
        }

        private static void TestPacketToBuffer()
        {
            Packet p = new Packet();


            p.header = msgHdr;
            p.toDrone = true;
            p.packetType = PacketType.ptSet;
            p.messageID = MessageTypes.msgDoTakeoff;
            p.sequence = 0;
            p.payload = new byte[0];
            byte[] b = PacketToBuffer(p);
            //  204
            //  88
            //  0
            //  124
            //  104
            //  84
            //  0
            //  0
            //  0
            //  178
            //  137


            byte[] correct = new byte[] { 0xcc, 0x58, 0, 0x7c, 0x68, 0x54, 0, 0, 0, 0xb2, 0x89 };

            for (int i = 0; i < correct.Length; i++)
            {
                if(b[i] != correct[i])
                {
                    Console.WriteLine("error not equal");
                }
            }
        }
    }
}
