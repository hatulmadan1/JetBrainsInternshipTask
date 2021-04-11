using System;

namespace BureaucracySimulator
{
    internal class UnconditionalRule : Rule
    {
        public int InStamp { get; protected set; }
        public int OutStamp { get; protected set; }
        public int NextDepartment { get; protected set; }

        public UnconditionalRule(int inStamp, int outStamp, int nextDepartment)
        {
            Type = RuleType.Unconditional;
            InStamp = inStamp;
            OutStamp = outStamp;
            NextDepartment = nextDepartment;
        }
    }
}
