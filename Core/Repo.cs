using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Specialized;

namespace Rox.Core
{
    class Repo
    {
        public readonly Dictionary<Tag, HashSet<Item>> Dict = new Dictionary<Tag, HashSet<Item>>();
        public readonly HashSet<Item> All = new HashSet<Item>();

        public event NotifyCollectionChangedEventHandler ItemCollectionChanged;

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
                item.Serialize(bw);
            }
        }

        public void Clear()
        {
            Dict.Clear();
            All.Clear();
            ItemCollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        // Clean tags that have no items
        public void CleanUpTags()
        {
            foreach (var e in Dict)
            {
                if (e.Value.Count == 0)
                {
                    Dict.Remove(e.Key);
                }
            }
        }

        public void ModifyItem(Item key, Action<Item> changeMethod)
        {
            if (All.TryGetValue(key, out var actual))
            {
                All.Remove(actual);
                ItemCollectionChanged.Invoke(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, 
                        actual));
                changeMethod(actual);
                All.Add(actual);
                ItemCollectionChanged.Invoke(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add, 
                        actual));
            }
        }
        
        public void AddItem(Item item)
        {
            LinkItemTags(item, item.Tags);
            All.Add(item);
            ItemCollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[]{item}));
        }

        public void RemoveItem(Item item, bool cleanupTags=false)
        {
            All.Remove(item);
            UnlinkItemTags(item, item.Tags, cleanupTags);
            ItemCollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[]{item}));
        }

        public void LinkItemTags(Item item, IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                Link(tag, item);
            }
        }

        public void UnlinkItemTags(Item item, IEnumerable<Tag> tags, bool cleanupTags)
        {
            foreach (var tag in tags)
            {
                if (item.Tags.Contains(tag))
                {
                    Unlink(tag, item, cleanupTags);
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

        private void Unlink(Tag tag, Item item, bool cleanupTag)
        {
            var tagEntry = Dict[tag];
            tagEntry.Remove(item);
            if (cleanupTag && tagEntry.Count == 0)
            {
                Dict.Remove(tag);
            }
            //Clear?
            item.Tags.Remove(tag);
        }
    }
}
