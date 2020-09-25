using ImplCore.Tree;
using System;
using System.Collections.Generic;

namespace ImplCore.Input
{
    public interface IInputKeeper<T> where T: notnull, IEquatable<T>
    {
        InputItem<T>? GetFirstItem();

        SearchItemResult<T> GetFirstChild(T parent);

        SearchItemResult<T> GetSecondChild(T parent);

        SearchItemResult<T> GetPrimaryParent(T child);

        SearchItemResult<IEnumerable<T>> GetAdditionalParents(T child);
    }
}
