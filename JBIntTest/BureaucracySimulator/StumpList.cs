using System;
using System.Collections;
using System.Text;

namespace BureaucracySimulator
{
    internal class StumpList
    {
        public BitArray StumpListArray { get; }

        public StumpList(int numberOfStumps)
        {
            StumpListArray = new BitArray(numberOfStumps, false);
        }

        public void ChangeByDepartmentRule(Rule rule)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public override int GetHashCode()
        {
            return StumpListArray.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < StumpListArray.Count; i++)
            {
                sb.Append(StumpListArray[i] ? '1' : '0');
            }

            return sb.ToString();
        }
    }
}
