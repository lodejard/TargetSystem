namespace TargetSystem.StandardTargets
{
    public class Targets : TargetProvider
    {
        private readonly ITargetManager _targetManager;

        public Targets(ITargetManager targetManager)
        {
            _targetManager = targetManager;
        }

        [Target(DependsOn = "Compile")]
        public void Default()
        {
        }

        [Target(DependsOn = "PreClean")]
        public void Clean()
        {
            _targetManager.ExecuteByName("PostClean");
        }

        [Target(DependsOn = "Clean, PreCompile")]
        public void Compile()
        {
            _targetManager.ExecuteByName("PostCompile");
        }

        [Target(DependsOn = "Compile, PrePackage")]
        public void Package()
        {
            _targetManager.ExecuteByName("PostPackage");
        }
    }
}
