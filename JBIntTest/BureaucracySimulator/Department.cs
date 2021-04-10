using System;

namespace BureaucracySimulator
{
    internal class Department
    {
        public Rule DepartmentRule { get; }

        public Department(Rule rule)
        {
            DepartmentRule = rule;
        }

        public int ProcessVisit(StumpList stumpList)
        {
            try
            {
                var nextDepartment = -1;
                switch (DepartmentRule.Type)
                {
                    case Rule.RuleType.Unconditional:
                        nextDepartment = ((UnconditionalRule)DepartmentRule).NextDepartment;
                        break;
                    case Rule.RuleType.Conditional:
                        nextDepartment = 
                            stumpList.StumpListArray[((ConditionalRule)DepartmentRule).ConditionalStampId] 
                                ? ((ConditionalRule)DepartmentRule).IfTrue.NextDepartment 
                                : ((ConditionalRule)DepartmentRule).IfFalse.NextDepartment;
                        break;
                    default:
                        throw new Exception("Department has no rule type, undefined behaviour");
                }

                stumpList.ChangeByDepartmentRule(DepartmentRule);
                return nextDepartment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
