using System;
using System.Collections.Generic;
using System.Text;

namespace ImplCore.Input
{
    public readonly struct InputItem
    {
        public char Parent { get; }
        public char Child { get; }

        public InputItem(char parent, char child)
        {
            Parent = parent;
            Child = child;
        }
    }
}
