using System;
using System.Threading.Tasks;

namespace PollyStudio
{
    internal class TaskExceptionTest
    {
        public static void TestTaskException()
        {
            var task1 = Task.Run(() => { throw new CustomException("This exception is expected!"); });

            try
            {
                task1.Wait();
                Console.ReadLine();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    // Handle the custom exception.
                    if (e is CustomException)
                        Console.WriteLine(e.Message);

                    // Rethrow any other exception.
                    else
                        throw e;
                }
            }
        }
    }

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}