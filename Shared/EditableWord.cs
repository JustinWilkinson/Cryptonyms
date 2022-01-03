using System;

namespace Cryptonyms.Shared
{
    public sealed record EditableWord : IEquatable<EditableWord>
    {
        public string Text { get; set; }

        public bool Editable { get; set; }

        public bool Equals(EditableWord other) => Text == other.Text;

        public override int GetHashCode() => base.GetHashCode();
    }
}