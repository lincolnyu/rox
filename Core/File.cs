using System.IO;

namespace Rox.Core
{
    class File : Item
    {
        public override int TypeId => 1;
        public string Path {get; set;}
        public override void DeserializeBody(BinaryReader br)
        {
            Path = br.ReadString();
        }

        public override void SerializeBody(BinaryWriter bw)
        {
            bw.Write(Path);
        }
    }
}