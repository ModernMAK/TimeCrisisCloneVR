using System;
using UnityEngine;

public class GunMagazine : MonoBehaviour
{
    [SerializeField] private int _maxAmmo = 1;
    [SerializeField] private int _currentAmmo = 0;
    public event EventHandler<ChangedEventArgs<int>> CurrentAmmoChanged;
    public event EventHandler<ChangedEventArgs<int>> MaxAmmoChanged;

	private void Awake()
	{
        if (_maxAmmo < 1)
            _maxAmmo = 1;
        _currentAmmo = _maxAmmo;
	}

	public int MaxAmmo
    {
        get => _maxAmmo;
        set
        {
            if (PropertyHelpers.UpdateValue(ref _maxAmmo, value, out var old))
                OnMaxAmmoChanged(new ChangedEventArgs<int>(old,value));
        }
    }


	public int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            value = Mathf.Clamp(value, 0, MaxAmmo);
            if (PropertyHelpers.UpdateValue(ref _currentAmmo, value, out var old))
                OnCurrentAmmoChanged(new ChangedEventArgs<int>(old,value));
        }
    }

    public bool IsAmmoFull => MaxAmmo <= CurrentAmmo;
    public bool IsAmmoEmpty => 0 >= CurrentAmmo;
    public bool HasAmmo => !IsAmmoEmpty;

    protected virtual void OnMaxAmmoChanged(ChangedEventArgs<int> args)
    {
        MaxAmmoChanged?.Invoke(this, args);
    }

    protected virtual void OnCurrentAmmoChanged(ChangedEventArgs<int> args)
    {
        CurrentAmmoChanged?.Invoke(this, args);
    }
}
