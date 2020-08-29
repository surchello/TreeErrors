using System;
using System.Collections.Generic;
using System.Text;

namespace ImplCore.Input
{
    public interface IInputKeeper
    {
        //NodeValues? GetFirstItem();

        //NodeValues? GetByParent(char parent);

        //List<NodeValues> GetByChild(char child);

        Input.InputItem? GetFirstItem();

        bool TryGetFirstChild(char parent, out char child);

        bool TryGetSecondChild(char parent, out char child);

        bool TryGetPrimaryParent(char child, out char parent);

        bool TryGetAdditionalParents(char child, out IEnumerable<char> parents);
    }
}
