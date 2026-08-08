using Fishmon.Controller.Actions;

namespace Fishmon.Controller.Input;

public class InputManager
{
    private FishAction currentAction = FishAction.None;

    private DateTime actionEnteredTime = DateTime.Now;
    private DateTime lastInputTime = DateTime.MinValue;

    private readonly TimeSpan dwellTime;
    private readonly TimeSpan cooldown;

    public InputManager(
        double dwellMilliseconds = 500,
        double coolDownMilliseconds = 500)
    {
        dwellTime = TimeSpan.FromMilliseconds(dwellMilliseconds);
        cooldown = TimeSpan.FromMilliseconds(coolDownMilliseconds);
    }

    public bool ShouldSendInput(FishAction detectedAction)
    {
        DateTime now = DateTime.Now;

        if(detectedAction != currentAction)
        {
            currentAction = detectedAction;
            actionEnteredTime = now;

            return false;
        }

        if(currentAction == FishAction.None)
        {
            return false;
        }

        bool hasDwelled = now - actionEnteredTime >= dwellTime;

        bool coolDownFinished = now - lastInputTime >= cooldown;

        if(hasDwelled && coolDownFinished)
        {
            lastInputTime = now;
            return true;
        }
        return false;
    }
}