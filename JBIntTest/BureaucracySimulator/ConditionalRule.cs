using System;

namespace BureaucracySimulator
{
    internal class ConditionalRule : Rule
    {
        public int ConditionalStampId { get; }
        internal UnconditionalRule IfTrue { get; }
        internal UnconditionalRule IfFalse { get; }

        public ConditionalRule(int conditionalStamp, UnconditionalRule ifTrue, UnconditionalRule ifFalse)
        {
            Type = RuleType.Conditional;
            ConditionalStampId = conditionalStamp;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }
    }
}
