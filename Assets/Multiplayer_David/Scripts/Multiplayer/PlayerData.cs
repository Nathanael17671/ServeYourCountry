using UnityEngine;
using System;

public class PlayerData : IEquatable<PlayerData>
{
    public ulong clientId;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId;
    }
}
