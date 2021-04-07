using System;
using System.Collections.Generic;
using System.Linq;

namespace ColorsLib.Models
{
    public class Solution
    {
        public void Solve(int A, List<int> B)
        {
            var Max = Math.Pow(B.Count(), A) % 1000000007;
            var temp = 0;
            foreach (var x in B)
            {

            }

        }
        public long FindValue(int x, int A)
        {
            if (x + 1 == A)
                return 1;
            else
            {
                var temp = (A - x);
                return FindValue(x + 1, A);
            }
        }
    }
}
