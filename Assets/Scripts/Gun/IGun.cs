using System;
using UnityEngine;

public interface IGun
{
    GunMagazine Magazine { get; }
    ReloadingState ReloadingState { get; }

    bool CanFire { get; }

    void Reload();
    //void Fire(Vector3 spawnPosition, Quaternion orientation);
    void PressFire();
    void ReleaseFire();

    event EventHandler<FiredEventArgs> Fired;
    event EventHandler FiredEmpty;

	void Stop();
}