using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimedClassInstanciation
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            do
            {
                input = Console.ReadLine();

                try
                {
                    if (input == "1")
                    {
                        Exception ex1 = new ArgumentNullException();
                        TimedClass timedClass = new TimedClass(ex1);                       
                    }
                    else if (input == "2")
                    {
                        Exception ex2 = new ApplicationException();
                        TimedClass timedClass = new TimedClass(ex2);
                    }

                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    Console.WriteLine($"-------------------- Ex count: {ex.InnerException.Message}");
                }
            } while (input != "x");
        }
    }

    class TimedClass
    {
        private static DateTime instanceTime;
        private static Type lastExceptionType = null;
        private static int exceptionCount = 0;
        private readonly double IntervalSeconds = 10;

        public TimedClass(Exception ex)
        {
            DateTime newInstanceTime = DateTime.Now;

            Console.WriteLine($"Last exception: {lastExceptionType}");
            Console.WriteLine($"Current exception: {ex.GetType()}");
            if (lastExceptionType != null)
            {
                Console.WriteLine($"Compare Exception Types: {lastExceptionType == ex.GetType()}");
                if (ex.GetType() == lastExceptionType)
                {
                    exceptionCount++;
                    Console.WriteLine($"Same exception");
                    if (newInstanceTime.Subtract(instanceTime).TotalSeconds < IntervalSeconds)
                    {
                        Console.WriteLine($"Last instance: {newInstanceTime}. Wait {IntervalSeconds}s");
                        throw new InvalidOperationException($"Same exception in less than {IntervalSeconds}s: {ex.GetType()}. ", new Exception($"{exceptionCount}"));
                    }
                    else
                    {
                        Console.WriteLine($"Last instance: {newInstanceTime}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Last instance: {newInstanceTime}. Ok");
                }
            }

            instanceTime = DateTime.Now;
            lastExceptionType = ex.GetType();

            Console.WriteLine($"New instance on {newInstanceTime} after {exceptionCount} tries");

            exceptionCount = 0;
        }
    }
}
