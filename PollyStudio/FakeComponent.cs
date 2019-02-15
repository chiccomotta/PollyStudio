using System;
using System.Collections.Generic;
using System.Text;

namespace PollyStudio
{
    public class FakeComponent
    {
        public static void FailedMethod()
        {
            throw new DivideByZeroException();
        }
    }
}
