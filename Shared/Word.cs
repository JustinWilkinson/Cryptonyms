using System;

namespace Cryptonyms.Shared
{
    public class EditableWord : IEquatable<EditableWord>
    {
        public string Text { get; set; }

        public bool Editable { get; set; }

        public bool Equals(EditableWord other) => Text == other.Text;
    }
}