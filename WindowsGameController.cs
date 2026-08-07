using System.Runtime.InteropServices;
using Fishmon.Controller.Actions;

namespace Fishmon.Controller.Input;

public class WindowsGameController
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        UIntPtr dwExtraInfo
    );

    private const uint KEYEVENTF_KEYUP = 0x0002;

    public void Press(FishAction action)
    {
        byte? key = GetVirtualKey(action);

        if (key == null)
        {
            return;
        }

        keybd_event(key.Value, 0, 0, UIntPtr.Zero);
        Thread.Sleep(100);
        keybd_event(key.Value, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

    }

    private static byte? GetVirtualKey(FishAction action)
    {

        //Mapping the button layout in the mGBA emulator - WASD for movement, A = X, B = Z, Start = Enter, Select = Backspace
        switch (action)
        {
            case FishAction.Up:
                return 0x57; // W
            
            case FishAction.Down:
                return 0x53; //S

            case FishAction.Left:
                return 0x41; //A

            case FishAction.Right:
                return 0x44; //D

            case FishAction.A:
                return 0x58; //X

            case FishAction.B:
                return 0x5A; //Z

            case FishAction.Start:
                return 0x0D; //Enter
            
            case FishAction.Select:
                return 0x08; //Backspace

            case FishAction.None:
            default:
                return null;
        }
    }
}
