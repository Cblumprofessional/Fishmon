using Fishmon.Controller.Actions;

namespace Fishmon.Controller.Input;

public interface IGameController
{
    void Press(FishAction action);
}