using System;
using System.Collections.Generic;

namespace ImplCore.Input
{
    public interface IInputKeeper<T> where T: notnull, IEquatable<T>
    {
        InputItem<T>? GetFirstItem();

        bool TryGetFirstChild(T parent, out T child);

        bool TryGetSecondChild(T parent, out T child);

        bool TryGetPrimaryParent(T child, out T parent);

        bool TryGetAdditionalParents(T child, out IEnumerable<T> parents);
    }
}
