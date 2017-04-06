using System;
using UnityEngine;

public interface IAbility
{
    AbilityType Type { get; }

    void Fire();
}
