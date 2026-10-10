using System;

[Serializable]
public class AudioKey
{
    public string Key = string.Empty;

    public static implicit operator string(AudioKey audioKey) => audioKey?.Key ?? string.Empty;

    public static bool operator ==(AudioKey a, AudioKey b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Key == b.Key;
    }

    public static bool operator !=(AudioKey a, AudioKey b) => !(a == b);
    public override bool Equals(object obj) => obj is AudioKey other && this == other;
    public override int GetHashCode() => Key != null ? Key.GetHashCode() : 0;
}