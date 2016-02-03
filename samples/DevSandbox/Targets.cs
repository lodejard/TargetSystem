using System;
using TargetSystem;

public class State : ITargetService
{
    public string Something { get; set; }
}

public class Targets : TargetProvider
{
    [Target(BeforeTarget = "PreCompile")]
    public string NugetRestore(State something, ITargetManager t)
    {
        Console.WriteLine("Doing NuGet Restore!");

        return "I Like Pie";
    }

    [Target(BeforeTarget = "Compile")]
    public void XUnitTest(State something, ITargetManager t)
    {
        var blah = t.Execute("NugetRestore");

        Console.WriteLine($"NugetRestore said {blah}");

        Console.WriteLine("Doing XUnit Test!");
    }
}
