using Polly;
using System;
using System.Collections.Generic;
using static PoliciesManager.PolicyManager;

namespace PollyStudio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                With3TimesPolicy(FakeComponent.FailedMethod,
                    (exception, retryCount) =>
                    {
                        Console.WriteLine($"Retry: {retryCount} -- exception: {exception.Message}");
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                With3TimesPolicy(FakeComponent.FailedMethod,
                    (exception, timeSpan) =>
                    {
                        Console.WriteLine($"timeSpan: {timeSpan} -- exception: {exception.Message}");
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                WithFirewallAndResultPolicy(FakeComponent.FailedMethodWithResult,
                    (result) =>
                    {
                        Console.WriteLine(result);
                    },
                    (exception, retryCount) =>
                    {
                        Console.WriteLine($"Retry: {retryCount} -- exception: {exception.Message}");
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            //WaitAndRetry();
            //Example1();
            //Example2();
            //ExampleWithContext();
            Console.ReadLine();
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

        public static void Example2()
        {
            Policy retryPolicy = Policy
                .Handle<Exception>()
                .Retry(3,
                    onRetry: (exception, retryCount, context) =>
                    {
                        // This policy might be re-used in several parts of the codebase, 
                        // so we allow the logged message to be tailored.
                        Console.WriteLine($"Retry {retryCount} of {context["Operation"]}, due to {exception.Message}.");
                    });


            retryPolicy.Execute(
                action: context => GetCustomerDetailsAsync(1),
                contextData: new Dictionary<string, object> { { "Operation", "GetCustomerDetails" } });

        }

        private static int GetCustomerDetailsAsync(int Id)
        {
            return 1;
        }


        // Context example
        public static void ExampleWithContext()
        {
            var retryPolicy = Policy<string>
                .HandleResult(msg => msg == "error")
                .Retry(3,
                    onRetry: (result, retryCount, context) =>
                    {
                        // This policy might be re-used in several parts of the codebase, 
                        // so we allow the logged message to be tailored.
                        Console.WriteLine($"Retry {retryCount} -- data {result.Result} for Operation {context["Operation"]}");
                    });

            retryPolicy.Execute(
                action: context => GetMessage(1),
                contextData: new Dictionary<string, object> { { "Operation", "GetMessage" } });
        }

        private static string GetMessage(int i)
        {
            return "error";
        }
    }
}
