using Fishmon.Controller.Input;
using Fishmon.Controller.Actions;
using System.Runtime.InteropServices;
//chatgpt wrote this

namespace Fishmon.Controller.Input;

public sealed class LinuxGameController : IGameController, IDisposable
{
    private const string UInputPath = "/dev/uinput";

    // Linux input event types
    private const ushort EV_SYN = 0x00;
    private const ushort EV_KEY = 0x01;
    private const ushort EV_ABS = 0x03;

    private const ushort SYN_REPORT = 0;

    // Absolute axes used as a D-pad
    private const ushort ABS_X = 0x00;
    private const ushort ABS_Y = 0x01;

    // Linux gamepad buttons
    private const ushort BTN_SOUTH = 0x130;   // A
    private const ushort BTN_EAST = 0x131;    // B
    private const ushort BTN_SELECT = 0x13a;
    private const ushort BTN_START = 0x13b;

    // uinput ioctls
    private const ulong UI_DEV_CREATE = 0x5501;
    private const ulong UI_DEV_DESTROY = 0x5502;
    private const ulong UI_SET_EVBIT = 0x40045564;
    private const ulong UI_SET_KEYBIT = 0x40045565;
    private const ulong UI_SET_ABSBIT = 0x40045567;

    private const int O_WRONLY = 0x0001;
    private const int O_NONBLOCK = 0x0800;

    private int fileDescriptor = -1;

    [DllImport("libc", SetLastError = true)]
    private static extern int open(
        string pathname,
        int flags
    );

    [DllImport("libc", SetLastError = true)]
    private static extern int close(
        int fd
    );

    [DllImport("libc", SetLastError = true)]
    private static extern int ioctl(
        int fd,
        ulong request,
        ulong argument
    );

    [DllImport("libc", SetLastError = true)]
    private static extern long write(
        int fd,
        IntPtr buffer,
        ulong count
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct InputEvent
    {
        public long tv_sec;
        public long tv_usec;

        public ushort type;
        public ushort code;

        public int value;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct UInputUserDevice
	{
   	 [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
   	 public string name;

   	 public ushort bustype;
  	  public ushort vendor;
   	 public ushort product;
   	 public ushort version;

   	 public uint ff_effects_max;

   	 [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
   	 public int[] absmax;

  	  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
  	  public int[] absmin;

  	  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
   	 public int[] absfuzz;

   	 [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
  	 public int[] absflat;
	}

    public LinuxGameController()
    {
        CreateVirtualController();
    }

    private unsafe void CreateVirtualController()
    {
        fileDescriptor = open(
            UInputPath,
            O_WRONLY | O_NONBLOCK
        );

        if (fileDescriptor < 0)
        {
            throw new InvalidOperationException(
                $"Could not open {UInputPath}. errno={Marshal.GetLastWin32Error()}"
            );
        }

        SetCapability(UI_SET_EVBIT, EV_KEY);
        SetCapability(UI_SET_EVBIT, EV_ABS);

        SetCapability(UI_SET_KEYBIT, BTN_SOUTH);
        SetCapability(UI_SET_KEYBIT, BTN_EAST);
        SetCapability(UI_SET_KEYBIT, BTN_START);
        SetCapability(UI_SET_KEYBIT, BTN_SELECT);

        SetCapability(UI_SET_ABSBIT, ABS_X);
        SetCapability(UI_SET_ABSBIT, ABS_Y);

        UInputUserDevice device = new UInputUserDevice{

        	name = "Fishmon Virtual Gamepad",

        	// BUS_USB
        	bustype = 0x03,

       		vendor = 0x1234,
        	product = 0x5678,
        	version = 1,

		ff_effects_max = 0,

		absmax = new int[64],
		absmin = new int[64],
		absfuzz = new int [64],
		absflat = new int[64]
	};

        device.absmin[ABS_X] = -1;
        device.absmax[ABS_X] = 1;

        device.absmin[ABS_Y] = -1;
        device.absmax[ABS_Y] = 1;

        int size = Marshal.SizeOf<UInputUserDevice>();

        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(
                device,
                buffer,
                false
            );

            long bytesWritten =
                write(
                    fileDescriptor,
                    buffer,
                    (ulong)size
                );

            if (bytesWritten < 0)
            {
                throw new InvalidOperationException(
                    $"Failed writing uinput device configuration. errno={Marshal.GetLastWin32Error()}"
                );
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        if (ioctl(
            fileDescriptor,
            UI_DEV_CREATE,
            0
        ) < 0)
        {
            throw new InvalidOperationException(
                $"Failed to create virtual controller. errno={Marshal.GetLastWin32Error()}"
            );
        }

        // Give Linux a moment to register the new device.
        Thread.Sleep(500);

        Console.WriteLine(
            "Fishmon Virtual Gamepad created."
        );
    }

    private void SetCapability(
        ulong request,
        ushort capability)
    {
        if (ioctl(
            fileDescriptor,
            request,
            capability
        ) < 0)
        {
            throw new InvalidOperationException(
                $"uinput ioctl failed. errno={Marshal.GetLastWin32Error()}"
            );
        }
    }

    public void Press(FishAction action)
    {
        switch (action)
        {
            case FishAction.Up:
                PressAxis(
                    ABS_Y,
                    -1
                );
                break;

            case FishAction.Down:
                PressAxis(
                    ABS_Y,
                    1
                );
                break;

            case FishAction.Left:
                PressAxis(
                    ABS_X,
                    -1
                );
                break;

            case FishAction.Right:
                PressAxis(
                    ABS_X,
                    1
                );
                break;

            case FishAction.A:
                PressButton(BTN_SOUTH);
                break;

            case FishAction.B:
                PressButton(BTN_EAST);
                break;

            case FishAction.Start:
                PressButton(BTN_START);
                break;

            case FishAction.Select:
                PressButton(BTN_SELECT);
                break;

            case FishAction.None:
                break;
        }
    }

    private void PressButton(
        ushort button)
    {
        Emit(
            EV_KEY,
            button,
            1
        );

        Sync();

        Thread.Sleep(100);

        Emit(
            EV_KEY,
            button,
            0
        );

        Sync();
    }

    private void PressAxis(
        ushort axis,
        int direction)
    {
        Emit(
            EV_ABS,
            axis,
            direction
        );

        Sync();

        Thread.Sleep(100);

        Emit(
            EV_ABS,
            axis,
            0
        );

        Sync();
    }

    private void Sync()
    {
        Emit(
            EV_SYN,
            SYN_REPORT,
            0
        );
    }

    private void Emit(
        ushort type,
        ushort code,
        int value)
    {
        InputEvent inputEvent = new()
        {
            tv_sec = 0,
            tv_usec = 0,
            type = type,
            code = code,
            value = value
        };

        int size = Marshal.SizeOf<InputEvent>();

        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(
                inputEvent,
                buffer,
                false
            );

            long bytesWritten =
                write(
                    fileDescriptor,
                    buffer,
                    (ulong)size
                );

            if (bytesWritten < 0)
            {
                throw new InvalidOperationException(
                    $"Failed to emit input event. errno={Marshal.GetLastWin32Error()}"
                );
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void Dispose()
    {
        if (fileDescriptor < 0)
        {
            return;
        }

        ioctl(
            fileDescriptor,
            UI_DEV_DESTROY,
            0
        );

        close(fileDescriptor);

        fileDescriptor = -1;
    }
}
