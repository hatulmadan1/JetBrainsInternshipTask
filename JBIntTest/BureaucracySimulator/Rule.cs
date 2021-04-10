using System;

namespace BureaucracySimulator
{
    internal class Rule
    {
        public enum RuleType
        {
            Conditional,
            Unconditional
        }

        public RuleType Type { get; protected set; }
    }
}
