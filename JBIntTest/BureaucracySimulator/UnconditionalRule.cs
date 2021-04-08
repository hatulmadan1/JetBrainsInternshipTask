using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class UnconditionalRule : Rule
    {
        public int InStump { get; protected set; }
        public int OutStump { get; protected set; }
        public int NextDepartment { get; protected set; }

        public UnconditionalRule(int inStump, int outStump, int nextDepartment)
        {
            Type = RuleType.Unconditional;
            InStump = inStump;
            OutStump = outStump;
            NextDepartment = nextDepartment;
        }
    }
}
