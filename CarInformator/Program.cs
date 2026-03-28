using System.Collections;

namespace CarInformatorApp {
    class Program {
        public static void Main() {
            var carTypeEnumerator = new StdinEnumEnumerator<CarFactory.CarType>();

            foreach (var carType in carTypeEnumerator) {
                var car = CarFactory.CreateCar(carType);
                Console.WriteLine(car.GetDescription());
            }
        }

        class StdinEnumEnumerator<T> : IEnumerable<T>
        where T : struct, Enum {
            public IEnumerator<T> GetEnumerator() {
                string? line;
                while ((line = Console.ReadLine()) != null) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line == "Help") {
                        Console.WriteLine("Available car types:");
                        foreach (var name in Enum.GetNames(typeof(T))) {
                            Console.WriteLine($"\t{name}");
                        }
                        continue;
                    }

                    if (line == "Halt") break;

                    if (!Enum.TryParse(line, true, out T carType)) continue;

                    yield return carType;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
    }
}

