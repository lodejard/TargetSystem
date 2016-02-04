using System;
using TargetSystem;

public class SomeSharedState : ITargetService
{
    public string Something { get; set; }
}

public class NugetRestoreResults
{
    public string[] AllProjectFolders { get; set; }
}

public class Targets : TargetProvider
{
    [Target(BeforeTarget = "PreCompile")]
    public NugetRestoreResults NugetRestore(SomeSharedState state)
    {
        Console.WriteLine("Doing NuGet Restore!");

        return new NugetRestoreResults
        {
            AllProjectFolders = new[]
            {
                "Alpha",
                "Beta"
            }
        };
    }

    [Target(BeforeTarget = "Compile")]
    public void XUnitTest(SomeSharedState state, NugetRestoreResults nugetResults)
    {
        foreach(var project in nugetResults.AllProjectFolders)
        {
            Console.WriteLine($"NugetRestore did {project}");
        }

        Console.WriteLine("Doing XUnit Test!");
    }
}
