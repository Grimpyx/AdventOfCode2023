using Memoize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public class MemoizeExampleClass
    {

        /*private Func<int, object> functionMemo;

        public object Function(int value)
        {
            if (functionMemo == null)
            {
                functionMemo = Memoizer.Memoize<int, object>((v) =>
                {
                    // Here comes the original code

                    object result;
                    // ... expensive code ...
                    return result;
                });
            }

            return functionMemo(value);
        }*/


        private Func<string, bool> functionMemo;

        public bool Function(string stringValue)
        {
            if (functionMemo == null)
            {
                functionMemo = Memoizer.Memoize<string, bool>((v) =>
                {
                    // Here comes the original code
                    bool result = true;
                    // ... expensive code ...
                    return result;
                });
            }

            return functionMemo(stringValue);
        }
    }
}
