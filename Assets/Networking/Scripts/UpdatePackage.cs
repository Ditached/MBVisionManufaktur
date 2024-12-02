using System;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;


public enum MsgType : ushort
{
    Ping = 0,
    Update = 1,
    Pong = 2,
    RequestChange = 3,
    ResetRotation = 4,
}

public enum AppState : byte
{
    Waiting = 0,
    Running = 1,
}


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpdatePackage
{
    public static bool configMode = false;
    public static AppState globalAppState = AppState.Waiting;
    public static ushort globalChipState = 0;
    public static float globalPlattformRotation = 0f;
    public static float basePlattformRotation = 0f;
    public static float globalPlattformSpeed = 0f;
    public static bool globalRotationRunning = true;
    public static uint nextId = 0;

    public MsgType msgType; // 2 byte
    public AppState appState; // 1 byte
    public uint id; // 4 bytes
    public ushort chipState; // 2 bytes
    public ushort buildNumber; // 2 bytes
    public bool inConfigMode; // 1 byte
    public float plattformRotation; // 4 bytes
    public float plattformSpeed;
    public bool rotationRunning;

    public static UpdatePackage CreateBasePackage()
    {
        return new UpdatePackage()
        {
            id = nextId++,
            chipState = globalChipState,
            appState = globalAppState,
            inConfigMode = configMode,
            plattformRotation = globalPlattformRotation,
            plattformSpeed = globalPlattformSpeed,
            rotationRunning = globalRotationRunning
        };
    }

    public static void ApplyReceivedPackage(UpdatePackage package)
    {
        globalChipState = package.chipState;
        globalAppState = package.appState;
        globalPlattformRotation = package.plattformRotation;
        globalPlattformSpeed = package.plattformSpeed;
        globalRotationRunning = package.rotationRunning;
        configMode = package.inConfigMode;
    }
    
    public UpdatePackage SetMsgType(MsgType type)
    {
        msgType = type;
        return this;
    }
    
    public UpdatePackage SetBuildNumber(ushort bNum)
    {
        buildNumber = bNum;
        return this;
    }
    
    public UpdatePackage SetAppState(AppState state)
    {
        appState = state;
        return this;
    }
    
    public UpdatePackage SetRotationRunning(bool running)
    {
        rotationRunning = running;
        return this;
    }
    

    public static UpdatePackage CreateRequestChangeForAppState(AppState appState)
    {
        return CreateBasePackage().SetMsgType(MsgType.RequestChange).SetAppState(appState);
    }
    
    public static UpdatePackage CreateRequestChangeForPlatformRotationRunning(bool running)
    {
        return CreateBasePackage().SetMsgType(MsgType.RequestChange).SetRotationRunning(running);
    }
    
    public static UpdatePackage CreateResetRotation()
    {
        return CreateBasePackage().SetMsgType(MsgType.ResetRotation);
    }
    
    public static UpdatePackage CreatePong(ushort bNum)
    {
        return CreateBasePackage().SetMsgType(MsgType.Pong).SetBuildNumber(bNum);
    }
    
    public static UpdatePackage CreatePing()
    {
        return CreateBasePackage().SetMsgType(MsgType.Ping);
    }

    public static UpdatePackage CreateChipStatePackage()
    {
        return CreateBasePackage().SetMsgType(MsgType.Update);
    }

    // Helper method to create bytes
    public byte[] ToBytes()
    {
        int size = Marshal.SizeOf(this);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            return arr;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    // Helper method to create struct from bytes
    public static UpdatePackage FromBytes(byte[] arr)
    {
        UpdatePackage str = new UpdatePackage();
        int size = Marshal.SizeOf(str);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(arr, 0, ptr, size);
            str = (UpdatePackage) Marshal.PtrToStructure(ptr, typeof(UpdatePackage));
            return str;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    // Get chipState as binary string
    public string GetChipStateString()
    {
        return Convert.ToString(chipState, 2).PadLeft(16, '0');
    }

    public override string ToString()
    {
        return $"Type={msgType}, AppState={appState}, ID={id}, ChipState={GetChipStateString()}";
    }
}