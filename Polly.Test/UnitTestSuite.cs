using System;
using Xunit;

namespace Polly.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(5)]
        public void Retry5Times(int retries)
        {
            int counter = 0;

            try
            {
                Policy
                    .Handle<IndexOutOfRangeException>()
                    .Retry(retries, (exception, i, context) =>
                    {
                        counter = i;
                    })
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
