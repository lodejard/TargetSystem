using System;
using System.Collections.Generic;

namespace TargetSystem
{
    public class TargetDefinition
    {
        public List<string> DependsOn { get; internal set; } = new List<string>();

        public Type ResultType { get; set; }

        public Func<IServiceProvider, object> ExecuteMethod { get; set; } = services => null;

        public bool HasExecuted { get; set; }

        public object ResultObject { get; set; }
    }
}
