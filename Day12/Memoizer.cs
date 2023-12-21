using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoize
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
    // From https://trenki2.github.io/blog/2018/12/31/memoization-in-csharp/
    public static class Memoizer
    {
        public static Func<R> Memoize<R>(Func<R> func)
        {
            object? cache = null;
            return () =>
            {
                if (cache == null)
                    cache = func();
                return (R)cache;
            };
        }

        public static Func<A, R> Memoize<A, R>(Func<A, R> func)
        {
            return a =>
            {
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
                Dictionary<A, R> cache = [];
                if (cache.TryGetValue(a, out R value))
                    return value;
                value = func(a);
                cache.Add(a, value);
                return value;
            };
        }

        public static Func<A, R> ThreadSafeMemoize<A, R>(Func<A, R> func)
        {
            var cache = new ConcurrentDictionary<A, R>();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            return argument => cache.GetOrAdd(argument, func);
        }
    }

    public static class MemoizerExtensions
    {
        public static Func<R> Memoize<R>(this Func<R> func)
        {
            return Memoizer.Memoize(func);
        }

        public static Func<A, R> Memoize<A, R>(this Func<A, R> func)
        {
            return Memoizer.Memoize(func);
        }

        public static Func<A, R> ThreadSafeMemoize<A, R>(this Func<A, R> func)
        {
            return Memoizer.ThreadSafeMemoize(func);
        }
    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
}
