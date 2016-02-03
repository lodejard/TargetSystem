using System;

namespace TargetSystem
{
    public class TargetAttribute : Attribute
    {
        public string Name { get; set; }
        public string DependsOn { get; set; }
        public string BeforeTarget { get; set; }
    }
}
