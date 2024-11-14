using System;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;


// Enum for message types (uses 1 byte)
public enum MsgType : byte
{
    Ping = 0,
    Update = 1,
    Pong = 2,
    Reserved2 = 3
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpdatePackage
{
    public static uint nextId = 0;

    [FormerlySerializedAs("messageType")] public MsgType msgType; // 1 byte
    public uint id; // 4 bytes
    public ushort chipState; // 2 bytes

    public static UpdatePackage CreatePong()
    {
        return new UpdatePackage()
        {
            msgType = MsgType.Pong,
            id = nextId++,
            chipState = 0
        };
    }

    public static UpdatePackage CreatePing()
    {
        return new UpdatePackage
        {
            msgType = MsgType.Ping,
            id = nextId++,
            chipState = 0
        };
    }

    public static UpdatePackage CreateChipStatePackage(ushort chipState)
    {
        return new UpdatePackage
        {
            msgType = MsgType.Update,
            id = nextId++,
            chipState = chipState
        };
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
        // Convert to binary and pad with zeros to 16 bits
        return Convert.ToString(chipState, 2).PadLeft(16, '0');
    }

    public override string ToString()
    {
        // Shows both binary and hex representation
        return $"Type={msgType} ID={id}, ChipState={GetChipStateString()}";
    }
}