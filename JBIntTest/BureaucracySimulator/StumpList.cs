using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class StumpList
    {
        public BitArray StumpListArray { get; }

        public StumpList(int numberOfStumps)
        {
            StumpListArray = new BitArray(numberOfStumps, false);
        }

        public void ChangeByDepartmentRule(Rule rule)
        {
            if (rule.Type == Rule.RuleType.Unconditional)
            {
                StumpListArray[((UnconditionalRule)rule).InStump] = true;
                StumpListArray[((UnconditionalRule)rule).OutStump] = false;
            } 
            else if (rule.Type == Rule.RuleType.Conditional)
            {
                if (StumpListArray[((ConditionalRule)rule).ConditionalStampId])
                {
                    StumpListArray[((ConditionalRule)rule).IfTrue.InStump] = true;
                    StumpListArray[((ConditionalRule)rule).IfTrue.OutStump] = false;
                }
                else
                {
                    StumpListArray[((ConditionalRule)rule).IfFalse.InStump] = true;
                    StumpListArray[((ConditionalRule)rule).IfFalse.OutStump] = false;
                }
            }
            else
            {
                throw new Exception("Rule has no type, undefined behaviour");
            }
        }

        public override int GetHashCode()
        {
            return StumpListArray.GetHashCode();
        }
    }
}
