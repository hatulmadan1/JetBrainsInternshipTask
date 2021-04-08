using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class Department
    {
        public Rule DepartmentRule { get; private set; }

        public Department(Rule rule)
        {
            DepartmentRule = rule;
        }

        public int ProcessVisit(StumpList stumpList)
        {
            int nextDepartment = -1;
            if (DepartmentRule.Type == Rule.RuleType.Unconditional)
            {
                nextDepartment = ((UnconditionalRule) DepartmentRule).NextDepartment;
            }
            else if (DepartmentRule.Type == Rule.RuleType.Conditional)
            {
                if (stumpList.StumpListArray[((ConditionalRule) DepartmentRule).ConditionalStampId])
                {
                    nextDepartment = ((ConditionalRule) DepartmentRule).IfTrue.NextDepartment;
                }
                else
                {
                    nextDepartment = ((ConditionalRule) DepartmentRule).IfFalse.NextDepartment;
                }
            }
            else
            {
                throw new Exception("Department has no rule type, undefined behaviour");
            }

            stumpList.ChangeByDepartmentRule(DepartmentRule);
            return nextDepartment;
        }
    }
}
