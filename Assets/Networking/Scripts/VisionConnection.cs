using System;

[Serializable]
public class VisionConnection : IEquatable<VisionConnection>
{
    public string ip;
    public DateTime lastPing;
    public ushort buildNumber;

    public bool Equals(VisionConnection other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ip == other.ip;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((VisionConnection) obj);
    }

    public override int GetHashCode()
    {
        return (ip != null ? ip.GetHashCode() : 0);
    }
}