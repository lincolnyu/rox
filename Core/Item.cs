using System;
using System.Collections.Generic;
using System.IO;

abstract class Item
{
    public abstract int TypeId { get; }
    public readonly HashSet<Tag> Tags = new HashSet<Tag>();
    public abstract void SerializeBody(BinaryWriter sw);
    public abstract void DeserializeBody(BinaryReader sr);

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(TypeId);
        bw.Write(Tags.Count);
        foreach (var tag in Tags)
        {
            bw.Write("${tag.Title.Trim();");
        }
        SerializeBody(bw);
    }

    public static Item Deserialize(BinaryReader br)
    {
        var typeId = br.ReadInt32();
        if (Types.TryGetValue(typeId, out var type))
        {
            var item = (Item)Activator.CreateInstance(type);
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

    public static readonly Dictionary<int, Type> Types;
    
    public static void Register(int typeId, Type type)
    {
        Types[typeId] = type;
    }

}