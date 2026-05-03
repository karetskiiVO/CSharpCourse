public static class Program {
    public static void Main() {
        new List<int> { 1, 1, 2, 3, 1, 5, 2, 2, 1, 2 }
            .Distinct()
            .WriteLine();

        new List<string> { "a", "abc", "B", "ccc", "BD" }
            .GroupBy(str => str.Length)
            .WriteLine();

        new Dictionary<string, int> { { "a", 3 }, { "b", 4 }, { "c", 1 } }
            .Merge(
                new Dictionary<string, int> { { "a", 4 }, { "d", 2 } },
                (x1, x2) => x1 + x2
            )
            .WriteLine();

        new List<(int, int)> { (1, 3), (2, 4), (0, 0), (12, 1) }
            .MaxBy(pair => pair.Item2)
            .WriteLine();
    }
}

public static class CollectionUtils {
    public static List<T> Distinct<T>(this List<T> source) {
        ArgumentNullException.ThrowIfNull(source);

        var used = new HashSet<T>();
        var result = new List<T>();

        foreach (var elem in source) {
            if (used.Add(elem)) result.Add(elem);
        }

        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        this List<TValue> source,
        Func<TValue, TKey> keySelector
    ) where TKey : notnull {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = new Dictionary<TKey, List<TValue>>();

        foreach (var item in source) {
            TKey key = keySelector(item);
            if (!groups.TryGetValue(key, out var list)) {
                list = [];
                groups[key] = list;
            }
            list.Add(item);
        }

        return groups;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        this Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver
    ) where TKey : notnull {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(conflictResolver);

        var merged = new Dictionary<TKey, TValue>(first);
        foreach (var (key, val) in second) {
            if (merged.TryGetValue(key, out var existingValue)) {
                merged[key] = conflictResolver(existingValue, val);
                continue;
            }

            merged[key] = val;
        }

        return merged;
    }

    public static T MaxBy<T, TKey>(this List<T> source, Func<T, TKey> selector)
    where TKey : IComparable<TKey> {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        if (source.Count == 0) throw new InvalidOperationException("The source sequence is empty.");

        var argmax = source[0];
        var max = selector(argmax);

        foreach (var elem in source[1..]) {
            var val = selector(elem);

            if (max.CompareTo(val) < 0) {
                max = val;
                argmax = elem;
            }
        }

        return argmax;
    }
}

public static class IOUtils {
    public static void Write<T>(this List<T> list) {
        if (list == null) {
            Console.Out.Write("<null>");
            return;
        }

        if (list.Count == 0) {
            Console.Out.Write("[]");
            return;
        }

        var splitter = "[";
        foreach (var elem in list) {
            Console.Out.Write(splitter);
            IOUtils.Write((dynamic)((elem != null) ? elem : "<null>"));
            splitter = " ";
        }
        Console.Out.Write("]");
    }

    public static void Write<K, V>(this Dictionary<K, V> dict) where K : notnull {
        if (dict == null) {
            Console.Out.Write("<null>");
            return;
        }

        if (dict.Count == 0) {
            Console.Out.Write("{}");
            return;
        }

        var splitter = "{";
        foreach (var (key, val) in dict) {
            Console.Out.Write(splitter);
            IOUtils.Write((dynamic)key);
            Console.Out.Write(":");
            IOUtils.Write((dynamic)((val != null) ? val : "<null>"));
            splitter = " ";
        }
        Console.Out.Write("}");
    }

    public static void Write<T>(this T obj) { Console.Out.Write(obj); }

    public static void WriteLine<T>(this List<T> list) {
        list.Write();
        Console.Out.WriteLine();
    }

    public static void WriteLine<K, V>(this Dictionary<K, V> dict) where K : notnull {
        dict.Write();
        Console.Out.WriteLine();
    }

    public static void WriteLine<T>(this T obj) {
        obj.Write();
        Console.Out.WriteLine();
    }
}
