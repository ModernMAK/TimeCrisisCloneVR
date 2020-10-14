using System;
using UnityEngine;

public interface IGun
{
    bool CanFire { get; }
    bool CanReload { get; }
    bool HasBullets { get; }
    bool MagazineFull { get; }
    void Reload();
    void Fire(Vector3 spawnPosition, Quaternion orientation);
    event EventHandler Reloading;
    event EventHandler<FiredEventArgs> Fired;
    event EventHandler ReloadingStarted;
    event EventHandler ReloadingEnded;
    event EventHandler FiredEmpty;
}