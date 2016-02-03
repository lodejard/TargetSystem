using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TargetSystem
{
    public interface ITargetManager : ITargetService
    {
        object Execute(string name);
    }

    public class TargetManager : ITargetManager
    {
        private readonly IServiceProvider _services;
        private readonly Lazy<Dictionary<string, TargetDefinition>> _targets;

        public TargetManager(IServiceProvider services)
        {
            _services = services;
            _targets = new Lazy<Dictionary<string, TargetDefinition>>(() =>
            {
                var targets = new Dictionary<string, TargetDefinition>();
                foreach (var provider in services.GetRequiredService<IEnumerable<ITargetProvider>>())
                {
                    provider.AddTargets(targets);
                }
                return targets;
            });
        }

        public object Execute(string name)
        {
            TargetDefinition definition;
            if (!_targets.Value.TryGetValue(name, out definition))
            {
                definition = new TargetDefinition();
                _targets.Value[name] = definition;
            }

            foreach (var dependsOn in definition.DependsOn)
            {
                Execute(dependsOn);
            }

            if (definition.HasExecuted == false)
            {
                definition.HasExecuted = true;

                Console.WriteLine($"Executing {name}");
                definition.TargetResult = definition.TargetAction.Invoke(_services);
            }

            return definition.TargetResult;
        }
    }
}

