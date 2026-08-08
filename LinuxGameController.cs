using Fishmon.Controller.Input;
using Fishmon.Controller.Actions;

public class LinuxGameController : IGameController
{
    public void Press(FishAction action)
    {
        Console.WriteLine($"Linux Input: {action}");
    }
}