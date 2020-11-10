using System;
using Polly;

namespace ResiliencePolicyManager
{
    public static class PollyPolicyManager
    {
        public const int Once = 1;
        public const int Twice = 2;
        public const int ThreeTimes = 3;

        public static void WithFirewallPolicy(Action action, Action<Exception, int> error)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(ThreeTimes, onRetry: error);

            policy.Execute(action);
        }

        public static void WithFirewallAndResultPolicy(Func<string> action, Action<string> success, Action<Exception, int> error)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(ThreeTimes, onRetry: error);

            var result = policy.Execute(action);
            success(result);
        }

        public static void WithFirewallPolicy(Action action, Action<Exception, int, Context> error)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(ThreeTimes, onRetry: error);

            policy.Execute(action);
        }

        public static void With3TimesPolicy(Action action, Action<Exception, TimeSpan> error)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3)
                }, onRetry: error);

            policy.Execute(action);            
        }        

        public static void With3TimesAndFallbackPolicy(Func<string> action, Action<string> success, Action<Exception, TimeSpan> error)
        {
            // Fallback policy
            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .Fallback(() =>
                {
                    Console.WriteLine("THIS IS A FALLBACK");
                    return "error";
                });

            // Retry policy
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }, onRetry: error);

            var result = fallbackPolicy.Wrap(retryPolicy).Execute(action);
            success(result);
        }        
    }
}