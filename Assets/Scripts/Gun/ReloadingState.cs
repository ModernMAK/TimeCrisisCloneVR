using System;
using UnityEngine;

[Serializable]
public class ReloadingState : GunState
{
    [SerializeField] private bool _isReloading;
    [SerializeField] private bool _firstReload;
    [SerializeField] private bool _reloadFullMagazine;
    [SerializeField] private float _initialReload;
    [SerializeField] private float _additionalReload;

    public ReloadingState(IGun gun) : base(gun)
    {
    }

    public bool ReloadFullMagazine => _reloadFullMagazine;
    public float InitialReloadTime => _initialReload;
    public float AdditionalReloadTime => _additionalReload;
    public bool FirstReload => _firstReload;
    public float ReloadTime => FirstReload ? _initialReload : _additionalReload;
    public bool IsReloading => _isReloading;
    
    public bool CanReload(float lastActionTime) => Time.time - lastActionTime >= ReloadTime;
    public void PerformReload(ref float lastActionTime, GunMagazine ammoState)
    {
        if (IsReloading && CanReload(lastActionTime) && !ammoState.IsAmmoFull)
        {
            lastActionTime = Time.time;
            if (ReloadFullMagazine)
            {
                OnReloading();
                ammoState.CurrentAmmo = ammoState.MaxAmmo;
            }
            else
            {
                ammoState.CurrentAmmo++;
                if (FirstReload)
                {
                    OnReloadingStarted();
                }
                else if (ammoState.IsAmmoFull)
                {
                    OnReloadingEnded();
                }
                else
                {
                    OnReloading();
                }
            }


            _firstReload = false;
        }

        if (IsReloading && ammoState.IsAmmoFull)
            StopReloading();
    }


    public void StartReloading()
    {
        if (!_isReloading)
        {
            _firstReload = true;
        }

        _isReloading = true;
    }

    public void StopReloading() => _isReloading = false;
    public event EventHandler Reloading;
    public event EventHandler ReloadingStarted;
    public event EventHandler ReloadingEnded;

    protected virtual void OnReloading()
    {
        Reloading?.Invoke(AttachedGun, EventArgs.Empty);
    }

    protected virtual void OnReloadingStarted()
    {
        ReloadingStarted?.Invoke(AttachedGun, EventArgs.Empty);
    }

    protected virtual void OnReloadingEnded()
    {
        ReloadingEnded?.Invoke(AttachedGun, EventArgs.Empty);
    }
    
    [Serializable]
    public struct Data
    {
        [SerializeField] public bool ReloadFullMagazine;
        [SerializeField] public float InitialReload;
        [SerializeField] public float AdditionalReload;
    }
    /// <summary>
    /// This is function copies over the values, and does not copy the data source's events.
    /// Events are fired normally when updating.
    /// </summary>
    /// <param name="data">The data source with new values to use.</param>
    /// <remarks>This does not copy events to avoid listeners losing track of their sources. This also means we loosely enforce AmmoData from being destroyed and replaced, which would be more suitable for a structure anyways.</remarks>
    public void CopyFrom(Data data)
    {
        _reloadFullMagazine = data.ReloadFullMagazine;
        _initialReload = data.InitialReload;
        _additionalReload = data.AdditionalReload;
    }
}