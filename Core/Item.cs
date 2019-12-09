using System;
using System.Collections.Generic;
using System.IO;

abstract class Item
{
    public abstract int TypeId { get; }
    public readonly HashSet<Tag> Tags = new HashSet<Tag>();
    public abstract void SerializeBody(StreamWriter sw);
    public abstract void DeserializeBody(StreamReader sr)

    public void Serialize(StreamWriter sw)
    {
        sw.Write(TypeId);
        sw.Write(Tags.Count);
        foreach (var tag in Tags)
        {
            sw.Write("${tag.Title.Trim();");
        }
        SerializeBody(sw);
    }

    public bool Deserialize(StreamReader sr)
    {
        sr.Read(TypeId);
    }

    public static readonly Dictionary<int, Type> Types;
    
    public static void Register(int typeId, Type type)
    {
        Types[typeId] = type;
    }

    public static Item Deserialize(StreamReader sr)
    {
        foreach (var t in Types.Keys)
        {
            var newItem = Types[t];
            if (newItem.TryDeserialize(sr))
            {
                Types[t] = (Item)Activator.CreateInstance(t);
                return newItem;
            }
        }
        return null;
    }
}