namespace Fishmon.Controller.Actions;


public enum FishAction
{
    None,
    Up,
    Down,
    Left,
    Right,
    A,
    B,
    Select,
    Start,
}

public static class FishActionMapper{

    
    private static readonly FishAction[,] Actions =
        {
            {FishAction.Up, FishAction.A, FishAction.Start},
            {FishAction.Left, FishAction.None, FishAction.Right},
            {FishAction.B, FishAction.Down, FishAction.Select}
        };

    public static FishAction GetActionFromZone(int row, int col)
    {
        if(row < 0 || row >= 3)
            throw new ArgumentOutOfRangeException(nameof(row));
        
        if(col < 0 || col >= 3)
            throw new ArgumentOutOfRangeException(nameof(col));

        return Actions[row, col];
    }

}

