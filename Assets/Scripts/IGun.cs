using System;
using UnityEngine;

public interface IGun
{
    AmmoState AmmoState { get; }
    
    bool CanFire { get; }
    bool CanReload { get; }
    
    void Reload();
    void Fire(Vector3 spawnPosition, Quaternion orientation);

    event EventHandler Reloading;
    event EventHandler<FiredEventArgs> Fired;
    event EventHandler ReloadingStarted;
    event EventHandler ReloadingEnded;
    event EventHandler FiredEmpty;
}