using System;
using UnityEngine;

public interface IGun
{
    AmmoState AmmoState { get; }
    ReloadingState ReloadingState { get; }

    bool CanFire { get; }

    void Reload();
    void Fire(Vector3 spawnPosition, Quaternion orientation);

    event EventHandler<FiredEventArgs> Fired;
    event EventHandler FiredEmpty;
}