using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Polly;

namespace PollyStudio
{
    class Program
    {
        static void Main(string[] args)
        {   
            //WaitAndRetry();
            Example1();

            Console.ReadLine();
            return;
        }

        public static void WaitAndRetry()
        {
            Action action = Foo;
            int counter = 0;

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


        public static void Example1()
        {
            var policyResult = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3)
                    },
                    (excpetion, timeSpan) =>
                    {
                        Console.WriteLine(
                            $"Logging error... {excpetion.Message} - timespan {timeSpan}");
                    })
                .ExecuteAndCapture(() =>
                {
                    var a = 1;
                    var b = 0;

                    return a / b;
                });

            Console.WriteLine(policyResult.Outcome);
            Console.WriteLine(policyResult.FinalException);
            Console.WriteLine(policyResult.ExceptionType);
            Console.WriteLine(policyResult.Result);
        }
    }
}
