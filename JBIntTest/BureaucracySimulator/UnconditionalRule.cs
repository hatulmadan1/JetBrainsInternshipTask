using System;

namespace BureaucracySimulator
{
    internal class UnconditionalRule : Rule
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
