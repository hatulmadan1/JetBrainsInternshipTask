using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class ConditionalRule : Rule
    {
        public int ConditionalStampId { get; private set; }
        public UnconditionalRule IfTrue { get; private set; }
        public UnconditionalRule IfFalse { get; private set; }

        public ConditionalRule(int conditionalStamp, UnconditionalRule ifTrue, UnconditionalRule ifFalse)
        {
            Type = RuleType.Conditional;
            ConditionalStampId = conditionalStamp;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }
    }
}
