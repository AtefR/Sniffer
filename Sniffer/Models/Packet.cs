using OXGaming.TibiaAPI.Constants;
using System.IO;
using System.Text;

namespace Sniffer.Models
{
    public class Packet
    {
        public PacketType Type { get; set; }
        public byte[] Data { get; set; }
        public BinaryReader Binary { get; set; }
        public short OpCode => Data[0];
        public Packet(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
            Binary = new BinaryReader(new MemoryStream(Data));
        }
        public override string ToString()
        {
            StringBuilder hex = new StringBuilder(Data.Length * 2);
            foreach (byte b in Data)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }
    }
}
