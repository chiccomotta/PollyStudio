using System;
using System.Collections.Generic;
using System.Text;

namespace PollyStudio
{
    public class FakeComponent
    {
        public static void FailedMethod()
        {
            Random rand = new Random();

            if (rand.Next(0, 2) == 0)
                throw new DivideByZeroException();

            Console.WriteLine("METHOD OK");
        }

        public static string FailedMethodWithResult()
        {
            Random rand = new Random();

            if (rand.Next(0, 2) == 0)
                throw new DivideByZeroException();

            return "Return method ok";
        }
    }
}
