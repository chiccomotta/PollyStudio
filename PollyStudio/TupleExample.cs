using System.Threading.Tasks;

namespace PollyStudio
{
    internal class TupleExample
    {
        public static async Task<(bool IsSuccess, int[] numbers, string message)> Example()
        {
            return await Task.Run(() =>
            {
                return (true, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, "OK");
            });
        }
    }
}