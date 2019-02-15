using System;

namespace Polly.PolicyManager
{
    public static class PolicyManager
    {
        public static void WithFirewallPolicy(Action action, Action<Exception, int> callback)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(3, onRetry: callback);

            policy.Execute(action);
        }

        public static void WithFirewallAndResultPolicy(Func<string> action, Action<string> success, Action<Exception, int> error)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(3, onRetry: error);

            var result = policy.Execute(action);
            success(result);
        }

        public static void WithFirewallPolicy(Action action, Action<Exception, int, Context> callback)
        {
            Policy policy = Policy
                .Handle<Exception>()
                .Retry(3, onRetry: callback);

            policy.Execute(action);
        }

        public static void With3TimesPolicy(Action action, Action<Exception, TimeSpan> callback)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3)
                }, onRetry: callback);

            policy.Execute(action);            
        }        
    }
}