using System;
using System.Collections.Generic;
using System.IO;

namespace Rox.Core
{
    abstract class Item
    {
        public interface ICreator
        {
            Item Create();
            Item Buffered { get; }
        }
        class Creator<T> : ICreator where T : Item, new()
        {
            public Creator()
            {
                _buffered = new T();
            }
            public Item Create()
            {
                var t = _buffered;
                _buffered = new T();
                return t;
            }
            public Item Buffered => _buffered;
            private T _buffered;
        }
        public abstract int TypeId { get; }
        public HashSet<Tag> Tags { get; private set;} = new HashSet<Tag>();
        public abstract void SerializeBody(BinaryWriter bw);
        public abstract void DeserializeBody(BinaryReader br);

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Tags.Count);
            foreach (var tag in Tags)
            {
                bw.Write("${tag.Title.Trim()}");
            }
            bw.Write(TypeId);
            SerializeBody(bw);
        }

        public static Item Deserialize(BinaryReader br)
        {
            var tagCount = br.ReadInt32();
            var tags = new HashSet<Tag>();
            while (tags.Count < tagCount)
            {
                tags.Add(new Tag(br.ReadString()));
            }
            var typeId = br.ReadInt32();
            if (Types.TryGetValue(typeId, out var creator))
            {
                var item = creator.Create();
                item.Tags = tags;
                item.TypedDeserialize(br);
                return item;
            }
            return null;
        }

        private void TypedDeserialize(BinaryReader br)
        {
            var typeId = br.ReadInt32();
            var tagsCount = br.ReadInt32();
            Tags.Clear();
            for (var i = 0; i < tagsCount; i++)
            {
                var tag = br.ReadString();
                Tags.Add(new Tag(tag));
            }
            DeserializeBody(br);
        }

        public static readonly Dictionary<int, ICreator> Types = new Dictionary<int, ICreator>();
        
        public static void Register<T>() where T : Item, new()
        {
            var creator = new Creator<T>();
            Types[creator.Buffered.TypeId] = creator;
        }
    }
}
