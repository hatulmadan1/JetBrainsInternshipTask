using System;
using System.Collections;
using System.Text;

namespace BureaucracySimulator
{
    internal class StampList
    {
        public BitArray StampListArray { get; }

        public StampList(int numberOfStamps)
        {
            StampListArray = new BitArray(numberOfStamps, false);
        }

        public void ChangeByDepartmentRule(Rule rule)
        {
            try
            {
                if (rule.Type == Rule.RuleType.Unconditional)
                {
                    StampListArray[((UnconditionalRule)rule).InStamp] = true;
                    StampListArray[((UnconditionalRule)rule).OutStamp] = false;
                } 
                else if (rule.Type == Rule.RuleType.Conditional)
                {
                    if (StampListArray[((ConditionalRule)rule).ConditionalStampId])
                    {
                        StampListArray[((ConditionalRule)rule).IfTrue.InStamp] = true;
                        StampListArray[((ConditionalRule)rule).IfTrue.OutStamp] = false;
                    }
                    else
                    {
                        StampListArray[((ConditionalRule)rule).IfFalse.InStamp] = true;
                        StampListArray[((ConditionalRule)rule).IfFalse.OutStamp] = false;
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

        /*public override int GetHashCode()
        {
            return StampListArray.GetHashCode();
        }*/

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < StampListArray.Count; i++)
            {
                sb.Append(StampListArray[i] ? '1' : '0');
            }

            return sb.ToString();
        }
    }
}
