using System;
using Xunit;

namespace Polly.Test
{
    public class UnitTest1
    {
        private static int counter = 0;

        [Fact]
        public void Retry5Times()
        {
           var retries = 5;

            try
            {
                Policy
                    .Handle<IndexOutOfRangeException>()
                    .Retry(retries, (exception, i, context) => { counter = i; })
                    .Execute(FailedProcedure);
            }
            catch (Exception ex)
            {
                Assert.Equal(retries, counter);
            }            
        }

        // Always throws an exception
        public static void FailedProcedure()
        {
            throw new IndexOutOfRangeException();
        }
    }
}
