using Fishmon.Controller.Actions;
using OpenCvSharp;

namespace Fishmon.Controller.Mapping;

public static class ZoneMapper
{
    public static FishAction GetActionFromPosition(
        Point position,
        int frameWidth,
        int frameHeight
    )
    {
        int cellWidth = frameWidth / 3;
        int cellHeight = frameHeight / 3;  

        int col = position.X / cellWidth;
        int row = position.Y / cellHeight;

        if(col > 2)
        {
            col = 2;
        }
        if(row > 2)
        {
            row = 2;
        }

        return FishActionMapper.GetActionFromZone(row, col);
    }
}