
using Repository;

namespace Repository {
    public interface IEntity {
        int ID { get; }
    }

    public class Repository<T> where T : class, IEntity {
        public void Add(T item) {
            var id = item.ID;

            if (data.ContainsKey(id)) throw new InvalidOperationException();
            data.Add(id, item);
        }

        public bool Remove(int id) {
            if (!data.ContainsKey(id)) return false;
            data.Remove(id);
            return true;
        }

        public T? GetByID(int id) {
            if (!data.ContainsKey(id)) return null;
            return data[id];
        }

        public IReadOnlyList<T> GetAll() => [.. data.Values];

        public int Count => data.Count;

        public IReadOnlyList<T> Find(Predicate<T> predicate) =>
            [.. data.Values.Where(elem => predicate(elem))];

        readonly Dictionary<int, T> data = [];
    }

    public class Product : IEntity {
        public int cost = 0;
        public int id = -1;

        public int ID => id;
    }

    public class User : IEntity {
        public string name = "";

        public int ID => name.Length == 0 ? -1 : name[0];
    }
}

public static class Program {
    public static void Main() {
        Console.Out.WriteLine("users:");

        var users = new User[] {
            new() { name = "Aa" },
            new() { name = "A" },
            new() { name = "B" },
            new() { name = "Ca" },
        };

        var userRepository = new Repository<User>();
        foreach (var user in users) {
            try {
                userRepository.Add(user);
            } catch (InvalidOperationException) { }
        }

        Console.Out.WriteLine($"unique ids: {userRepository.Count}");
        foreach (var user in userRepository.GetAll()) {
            Console.Out.WriteLine($"user: {user.name} id: {user.ID}");
        }

        Console.Out.WriteLine("products:");
        var random = new Random(42);

        var productRepository = new Repository<Product>();

        var products = Enumerable
            .Range(0, 20)
            .Select(_ => new Product() {
                id = random.Next(0, 20),
                cost = random.Next(0, 400),
            });

        Console.Out.WriteLine("id\t|\tcost");
        products.ForEach(p => Console.Out.WriteLine($"{p.id}\t|\t{p.cost}"));
        products = products
            .Where(p => {
                try {
                    productRepository.Add(p);
                    return true;
                } catch (InvalidOperationException) {
                    Console.Out.WriteLine($"Multiply id: {p.id}");
                    return false;
                }
            })
            .NopeAgregate();

        Enumerable
            .Range(0, 4)
            .Select(_ => random.Next(0, 20))
            .ForEach(id => {
                var prod = productRepository.GetByID(id);
                if (prod == null) {
                    Console.Out.WriteLine($"id: {id} not found");
                } else {
                    Console.Out.WriteLine($"found id: {id} with cost: {prod.cost}");
                }
            });

        var costThreshold = 100;

        Console.WriteLine("products where cost more than {0}: {1}",
            costThreshold,
            productRepository.Find(prod => prod.cost > costThreshold).Count
        );
    }
}

public static class CollectionUtils {
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach (var val in enumerable) action(val);
    }

    public static IEnumerable<T> NopeAgregate<T>(this IEnumerable<T> enumerable) {
        foreach (var elem in enumerable) ;
        return enumerable;
    }

    public static T RandomElem<T>(this T[] arr, Random random) {
        if (arr == null || arr.Length == 0) throw new InvalidOperationException();
        return arr[random.Next(0, arr.Length)];
    }
}
