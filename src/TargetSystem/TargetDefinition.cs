using System;
using System.Collections.Generic;

namespace TargetSystem
{
    public class TargetDefinition
    {
        public Func<IServiceProvider, object> TargetAction { get; set; } = services => null;

        public bool HasExecuted { get; set; }

        public List<string> DependsOn { get; internal set; } = new List<string>();
        public object TargetResult { get; set; }
    }
}
