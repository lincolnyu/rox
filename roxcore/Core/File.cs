using System.Collections.Generic;
using System.IO;

namespace Rox.Core
{
    public class File : Item
    {
        public override int TypeId => 1;

        // Absolute path of the file
        public string Path {get; set;}

        // All other duplicates
        public List<string> Duplicates { get; } = new List<string>();
        
        public override void DeserializeBody(BinaryReader br)
        {
            Path = br.ReadString();
        }

        public override void SerializeBody(BinaryWriter bw)
        {
            bw.Write(Path);
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO path same
            return Path == ((File)obj).Path;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Path.GetHashCode();
        } 
    }
}