using System.Collections.Generic;

namespace Hollow.Controls.Toast;

internal static class ToastPool
{
    private static readonly Stack<Toast> Pool = new();

    internal static Toast Get()
    {
        return Pool.Count >= 1 ? Pool.Pop() : new Toast();
    }

    internal static void Return(Toast toast) => Pool.Push(toast);

    internal static void Return(IEnumerable<Toast> toasts)
    {
        foreach (var toast in toasts)
            Pool.Push(toast);
    }
}