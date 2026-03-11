using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace CalculatorApp {
    public class Program {
        public static void Main() {
            new Calculator(new StdinTasker()).Execute();
        }
    }

    public struct Task {
        public enum OpType {
            Add,
            Mul,
            Sub,
            Div
        }

        public float a;
        public float b;
        public OpType opType;
    }

    class StdinTasker : IEnumerable<Task> {
        public IEnumerator<Task> GetEnumerator() {
            string? line;
            while ((line = Console.ReadLine()) != null) {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line == "Halt") break;


                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) continue;

                if (!Enum.TryParse(parts[0], true, out Task.OpType op)) continue;
                if (!float.TryParse(parts[1], out float a)) continue;
                if (!float.TryParse(parts[2], out float b)) continue;

                yield return new Task { a = a, b = b, opType = op };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Calculator {


        public Calculator(IEnumerable<Task> tasker) {
            this.tasker = tasker;
        }

        public void Execute() {
            foreach (var task in tasker) {
                var res = task.opType switch {
                    Task.OpType.Add => task.a + task.b,
                    Task.OpType.Sub => task.a - task.b,
                    Task.OpType.Mul => task.a * task.b,
                    Task.OpType.Div => task.a / task.b,
                    _ => float.NaN,
                };

                System.Console.WriteLine(res);
            }
        }

        private readonly IEnumerable<Task> tasker;
    }
}
