using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TargetSystem
{
    public interface ITargetManager : ITargetService
    {
        object ExecuteByName(string name);

        object ExecuteByReturnType(Type returnType);
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
                foreach (var provider in services.GetRequiredService<IEnumerable<ITargetProvider>>())
                {
                    provider.UpdateTargets(targets);
                }
                return targets;
            });
        }

        public object ExecuteByReturnType(Type returnType)
        {
            var definitions = _targets.Value.Where(kv => kv.Value.ResultType == returnType);

            if (!definitions.Any())
            {
                throw new Exception($"No targets return type {returnType}");
            }

            if (definitions.Count() != 1)
            {
                throw new Exception($"Multiple targets return type {returnType}");
            }

            return ExecuteTarget(definitions.Single().Key, definitions.Single().Value);
        }

        public object ExecuteByName(string name)
        {
            TargetDefinition definition;
            if (!_targets.Value.TryGetValue(name, out definition))
            {
                definition = new TargetDefinition();
                _targets.Value[name] = definition;
            }

            return ExecuteTarget(name, definition);
        }

        private object ExecuteTarget(string name, TargetDefinition definition)
        {
            foreach (var dependsOn in definition.DependsOn)
            {
                ExecuteByName(dependsOn);
            }

            if (definition.HasExecuted == false)
            {
                definition.HasExecuted = true;

                Console.WriteLine($"Executing {name}");
                definition.ResultObject = definition.ExecuteMethod(_services);
            }

            return definition.ResultObject;
        }
    }
}

