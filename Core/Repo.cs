using System.Collections.Generic;
using System.IO;
using System;

class Repo
{
    public readonly Dictionary<Tag, HashSet<Item>> Dict = new Dictionary<Tag, HashSet<Item>>();
    public readonly HashSet<Item> All = new HashSet<Item>();

    public void Deserialize(BinaryReader br)
    {
        try
        {
            Clear();
            while (true)
            {
                var item = Item.Deserialize(br);
                AddItem(item);
            }
        }
        catch (EndOfStreamException)
        {
        }
        catch (IOException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }

    public void Serialize(BinaryWriter bw)
    {
        foreach (var item in All)
        {
            //item.Serialize();
        }
    }

    public void Clear()
    {
        Dict.Clear();
        All.Clear();
    }

    public void AddItem(Item item)
    {
        LinkItemTags(item, item.Tags);
        All.Add(item);
    }

    public void RemoveItem(Item item)
    {
        All.Remove(item);
        UnlinkItemTags(item, item.Tags);
    }

    public void LinkItemTags(Item item, ICollection<Tag> tags)
    {
        foreach (var tag in tags)
        {
            Link(tag, item);
        }
    }

    public void UnlinkItemTags(Item item, ICollection<Tag> tags)
    {
        foreach (var tag in tags)
        {
            if (item.Tags.Contains(tag))
            {
                Unlink(tag, item);
            }
        }
    }

    private void Link(Tag tag, Item item)
    {
        item.Tags.Add(tag);
        if (!Dict.TryGetValue(tag, out var val))
        {
            Dict[tag] = val = new HashSet<Item>();
        }
        val.Add(item);
    }

    private void Unlink(Tag tag, Item item)
    {
        Dict[tag].Remove(item);
        //Clear?
        item.Tags.Remove(tag);
    }
}