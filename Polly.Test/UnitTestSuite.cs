using System;
using System.Diagnostics;
using Polly.CircuitBreaker;
using Xunit;

namespace Polly.Test
{
    public class UnitTest1
    {
        private static int counter = 0;

        [Fact]
        public void RetryForever()
        {
            Policy
                .Handle<IndexOutOfRangeException>()
                .RetryForever(exception => { counter++; })
                .Execute(Foo);
        }

        public static void Foo()
        {
            if(counter != 100)
                throw new IndexOutOfRangeException();
        }
    }
}
