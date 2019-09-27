using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Senko.Discord.Gateway
{
    internal static class FuncExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask InvokeAsync<T1, T2>(this Func<T1, T2, ValueTask> func, T1 arg1, T2 arg2)
        {
            return func?.Invoke(arg1, arg2) ?? default;
        }
    }
}
