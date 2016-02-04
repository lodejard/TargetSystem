using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TargetSystem
{
    public interface ITargetProvider : ITargetService
    {
        void AddTargets(IDictionary<string, TargetDefinition> targets);

        void UpdateTargets(IDictionary<string, TargetDefinition> targets);
    }

    public abstract class TargetProvider : ITargetProvider
    {
        public virtual void AddTargets(IDictionary<string, TargetDefinition> targets)
        {
            foreach (var method in GetType().GetTypeInfo().DeclaredMethods)
            {
                var targetAttribute = method.GetCustomAttributes<TargetAttribute>().SingleOrDefault();
                if (targetAttribute == null)
                {
                    continue;
                }

                var name = targetAttribute.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = method.Name;
                }

                TargetDefinition definition;
                if (!targets.TryGetValue(name, out definition))
                {
                    definition = new TargetDefinition();
                    targets[name] = definition;
                }

                definition.ExecuteMethod = services =>
                {
                    var targetManager = services.GetRequiredService<ITargetManager>();

                    var parameters = method.GetParameters();
                    var arguments = new object[parameters.Length];
                    for (var index = 0; index < parameters.Length; index++)
                    {
                        arguments[index] = services.GetService(parameters[index].ParameterType);

                        if (arguments[index] == null)
                        {
                            arguments[index] = targetManager.ExecuteByReturnType(parameters[index].ParameterType);
                        }
                    }
                    return method.Invoke(this, arguments);
                };

                definition.ResultType = method.ReturnType;

                if (!string.IsNullOrWhiteSpace(targetAttribute.DependsOn))
                {
                    foreach (var dependsOn in targetAttribute.DependsOn.Split(
                        new[] { ',', ' ' },
                        StringSplitOptions.RemoveEmptyEntries))
                    {
                        definition.DependsOn.Add(dependsOn);
                    }
                }

                if (!string.IsNullOrWhiteSpace(targetAttribute.BeforeTarget))
                {
                    foreach (var beforeTarget in targetAttribute.BeforeTarget.Split(
                        new[] { ',', ' ' },
                        StringSplitOptions.RemoveEmptyEntries))
                    {

                        TargetDefinition beforeDefinition;
                        if (!targets.TryGetValue(beforeTarget, out beforeDefinition))
                        {
                            beforeDefinition = new TargetDefinition();
                            targets[beforeTarget] = beforeDefinition;
                        }
                        beforeDefinition.DependsOn.Add(name);
                    }
                }
            }
        }

        public void UpdateTargets(IDictionary<string, TargetDefinition> targets)
        {
        }
    }
}
