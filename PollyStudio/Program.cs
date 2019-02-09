using System;
using System.Diagnostics;
using Polly;

namespace PollyStudio
{
    class Program
    {
        static void Main(string[] args)
        {
            Action action = Foo;

            var policy = Policy 
                .Handle<DivideByZeroException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3)
                }, (excpetion, timeSpan) =>
                {
                    Console.WriteLine($"Logging error... {excpetion.Message} - timespan {timeSpan}");
                });
             
            try
            {
                policy.Execute(action);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Last exception handling. Program terminated.");
                Console.ReadLine();
            }            
        }

        public static void Foo()
        {
            throw new DivideByZeroException();
        }    
    }
}
