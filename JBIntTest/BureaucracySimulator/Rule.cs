using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class Rule
    {
        public enum RuleType
        {
            Conditional,
            Unconditional
        }

        public RuleType Type { get; protected set; }
    }
}
