using System;
using System.Diagnostics.CodeAnalysis;

namespace Rox.Core
{
    public class Tag : IEquatable<Tag>
    {
        public string Title { get; }
        public string TitleAsId { get; }
        public Tag(string title)
        {
            Title = title;
            TitleAsId = title.ToLower();
        }

        public override int GetHashCode()
        {
            return TitleAsId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Tag tag)
            {
                return Equals(tag);
            }
            return false;
        }

        public bool Equals([AllowNull] Tag other)
            => TitleAsId == other.TitleAsId;
    }
}