﻿using Polly;
using ResiliencePolicyManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PollyStudio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TaskExceptionTest.TestTaskException();
            var res = TupleExample.Example().Result;

            if (res.IsSuccess)
            {
                Console.WriteLine(res.message);
                res.numbers.ToList().ForEach(Console.WriteLine);
            }

            Console.ReadKey();
            return;
            PollyPolicyManager.With3TimesAndFallbackPolicy(FakeComponent.FailedMethodWithResult,
                (result) =>
                {
                    Console.WriteLine(result);
                },
                (exception, retryCount) =>
                {
                    Console.WriteLine($"Retry: {retryCount} -- exception: {exception.Message}");
                });

            WaitAndRetryWithFallback();
            Example1();
            Example2();
            ExampleWithContext();
            Console.ReadLine();
        }

        public static void WaitAndRetryWithFallback()
        {
            Func<int> action = FooInt;
            int counter = 0;

            var fallbackPolicy = Policy<int>
                .Handle<Exception>()
                .Fallback<int>(cancellationToken =>
                {
                    Console.WriteLine("THIS IS FALLBACK");
                    return 0;
                });

            var retryPolicy = Policy
                .Handle<DivideByZeroException>()
                .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3)
                    },
                    (res, timeSpan, context) =>
                    {
                        //Console.WriteLine($"Logging error... {excpetion.Message} - timespan {timeSpan}");
                        Console.WriteLine($"result is {res}");
                    });

            var result = fallbackPolicy.Wrap(retryPolicy).Execute(action);
        }

        public static void Foo()
        {           
            throw new DivideByZeroException();
        }

        public static int FooInt()
        {
            throw new DivideByZeroException();
            return 100;
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
