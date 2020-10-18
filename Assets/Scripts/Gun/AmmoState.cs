using System;
using UnityEngine;

[Serializable]
public class AmmoState : GunState
{
    [SerializeField] private int _currentAmmo;
    [SerializeField] private int _maxAmmo;
    public event EventHandler<CurrentAmmoChangedArgs> CurrentAmmoChanged;
    public event EventHandler<MaxAmmoChangedArgs> MaxAmmoChanged;

    public AmmoState(IGun gun) : base(gun)
    {
        _currentAmmo = _maxAmmo = 1;
    }

    public int MaxAmmo
    {
        get => _maxAmmo;
        set
        {
            if (PropertyHelpers.UpdateValue(ref _maxAmmo, value))
                OnMaxAmmoChanged(new MaxAmmoChangedArgs() {MaxAmmo = _maxAmmo});
        }
    }

    public int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            if (PropertyHelpers.UpdateValue(ref _currentAmmo, value))
                OnCurrentAmmoChanged(new CurrentAmmoChangedArgs() {CurrentAmmo = _currentAmmo});
        }
    }

    public bool IsAmmoFull => MaxAmmo <= CurrentAmmo;
    public bool IsAmmoEmpty => 0 >= CurrentAmmo;
    public bool HasAmmo => !IsAmmoEmpty;

    protected virtual void OnMaxAmmoChanged(MaxAmmoChangedArgs args)
    {
        MaxAmmoChanged?.Invoke(AttachedGun, args);
    }

    protected virtual void OnCurrentAmmoChanged(CurrentAmmoChangedArgs args)
    {
        CurrentAmmoChanged?.Invoke(AttachedGun, args);
    }

    
    [Serializable]
    public struct Data
    {
        [SerializeField] public int MaxAmmo;
    }
    /// <summary>
    /// This is function copies over the values, and does not copy the data source's events.
    /// Events are fired normally when updating.
    /// </summary>
    /// <param name="data">The data source with new values to use.</param>
    /// <remarks>This does not copy events to avoid listeners losing track of their sources. This also means we loosely enforce AmmoData from being destroyed and replaced, which would be more suitable for a structure anyways.</remarks>
    public void CopyFrom(Data data)
    {
        MaxAmmo = data.MaxAmmo;
        CurrentAmmo = MaxAmmo;
    }
}