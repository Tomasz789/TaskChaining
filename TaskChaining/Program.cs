using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TasksLib;
#pragma warning disable S1118 // Utility classes should not have public constructors

namespace TaskChaining
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var taskClass = new TaskChainingClass();
            Random rnd = new Random();

            Task<int[]> generateArrayTask = Task.Run(() =>
                {
                    Console.WriteLine("Started");
                    var rnd = new Random();
                    int min = 0, max = 1;
                    do
                    {
                        min = rnd.Next();
                        max = rnd.Next();
                    }
                    while (min > max);
                    var res = taskClass.GenerateIntegerArray(10, min, max);
                    Console.WriteLine("Generated array values: ");
                    PrintCurrentArrayValues(res);
                    Console.WriteLine();
                    Console.WriteLine("Average value of generated array items: {0}", taskClass.GetAverageValue(res));
                    return res;
                }
            );

            await generateArrayTask.ContinueWith(task =>
                {
                    Task.Delay(500).Wait();
                    int rndVal = rnd.Next();
                    Console.WriteLine("Multiplication value (random): {0}", rndVal);
                    Console.WriteLine();
                    return taskClass.ReturnMultipliedArray(generateArrayTask.Result, rndVal);
                }
            )
            .ContinueWith(antecedent => 
            {
                Task.Delay(500).Wait();
                taskClass.SortArrayAscending(antecedent.Result);
                Console.WriteLine($"Result after multiplication task - avg value: {taskClass.GetAverageValue(antecedent.Result)} ");
                PrintCurrentArrayValues(antecedent.Result);
                Console.WriteLine();
                Console.WriteLine("Average value after mult. " + taskClass.GetAverageValue(antecedent.Result));
            });
        }

        private static void PrintCurrentArrayValues(int [] array)
        {
            if (!array.Any())
            {
                Console.WriteLine("Array is empty - there's nothing to print!");
            }
            foreach (var item in array)
            {
                Console.Write(item + "\t");
            }

            Console.WriteLine();
        }
    }
}
