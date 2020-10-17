using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FiredEventArgs : EventArgs
{
    public Ray[] Raycasts;
}

public static class PropertyHelpers
{
    //Returns true if the value was updated, false if the value did not change
    public static bool UpdateValue<T>(ref T field, T value) where T : IEquatable<T>
    {
        var didChange = !field.Equals(value);
        field = value;
        return didChange;
    }
}
[Serializable]
public class AmmoState
{
    [SerializeField] private int _currentAmmo;
    [SerializeField] private int _maxAmmo;
    /// <summary>
    /// A Helper to get the Gun this data is attached to.
    /// A null value indicates that the reference has not been set or the data is not representing the state of a gun
    /// </summary>
    public IGun AttachedGun { get; }
    public event EventHandler<CurrentAmmoChangedArgs> CurrentAmmoChanged;
    public event EventHandler<MaxAmmoChangedArgs> MaxAmmoChanged;

    public int MaxAmmo
    {
        get => _maxAmmo;
        set
        {
            if(PropertyHelpers.UpdateValue(ref _maxAmmo, value))
                OnMaxAmmoChanged(new MaxAmmoChangedArgs() {MaxAmmo = _maxAmmo});
        }
    }


    public int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            if(PropertyHelpers.UpdateValue(ref _currentAmmo, value))
                OnCurrentAmmoChanged(new CurrentAmmoChangedArgs() {CurrentAmmo = _currentAmmo});
            
        }
    }
    
    public bool IsAmmoFull => MaxAmmo <= CurrentAmmo;
    public bool IsAmmoEmpty => 0 >= CurrentAmmo;
    public bool HasAmmo => !IsAmmoEmpty;

    /// <summary>
    /// Ammo changed args use the attached gun if applicable
    /// </summary>
    private object Sender => AttachedGun != null ? (object)AttachedGun : this;
    protected virtual void OnMaxAmmoChanged(MaxAmmoChangedArgs args)
    {
        MaxAmmoChanged?.Invoke(Sender, args);
    }
    protected virtual void OnCurrentAmmoChanged(CurrentAmmoChangedArgs args)
    {
        CurrentAmmoChanged?.Invoke(Sender, args);
    }

    /// <summary>
    /// This is function copies over the values, and does not copy the data source's events.
    /// Events are fired normally when updating.
    /// </summary>
    /// <param name="state">The data source with new values to use.</param>
    /// <remarks>This does not copy events to avoid listeners losing track of their sources. This also means we loosely enforce AmmoData from being destroyed and replaced, which would be more suitable for a structure anyways.</remarks>
    public void CopyFrom(AmmoState state)
    {
        MaxAmmo = state.MaxAmmo;
        CurrentAmmo = state.CurrentAmmo;
    }
}

public class ReloadData
{
    [SerializeField] private bool _fullReload;
    [SerializeField] private float _initialReload;
    [SerializeField] private float _additionalReload;
}

[Serializable]
public class Gun : IGun
{
    public Gun()
    {
        ammoState = new AmmoState();
    }
    
    [SerializeField] private AmmoState ammoState;
    [SerializeField] private AimCone _aimCone;
    [SerializeField] private MagazineInfo _magazine;
    [SerializeField] private FiringInfo _firingInfo;
    [SerializeField] private float _lastAction;
    [SerializeField] private bool firstReload;
    public bool FirstReload
    {
        get => firstReload;
        private set => firstReload = value;
    }

    public float LastAction
    {
        get => _lastAction;
        private set => _lastAction = value;
    }

    [SerializeField] private bool _useRandomSpread = false;

    public AmmoState AmmoState => ammoState;

    public bool CanFire => (_firingInfo.FireCooldown + LastAction - Time.time <= 0f);

    public bool CanReload => ((FirstReload ? _magazine.InitialReload : _magazine.AdditionalReload) +
        LastAction - Time.time <= 0f);
    
    public Vector2 RandomSpread => Random.insideUnitCircle;

    public Vector2 GetUniformSpread(int i, int n, int rings = 2)
    {
        if (i == 0)
            return Vector2.zero; //No spread for uniform first pellet
        //Correct i for remaining pellets
        n--;
        i--;
        var n_per_r = n / rings; //# per ring
        var extra_n = n % n_per_r;
        var r = (i / n_per_r); //ring
        const float TwoPi = Mathf.PI * 2f;
        float percent;
        float scale;
        if (r >= rings - 1)
        {
            scale = 1f;
            var i_r = i - (n_per_r * rings);
            percent = (float) i_r / (n_per_r + extra_n);
        }
        else
        {
            scale = (float) (r + 1) / rings;
            var i_r = i % n_per_r;
            percent = (float) i_r / n_per_r;
        }

        return new Vector2(Mathf.Cos(percent * TwoPi), Mathf.Sin(percent * TwoPi)) * scale;
    }

    public void Reload()
    {
        if (CanReload && !ammoState.IsAmmoFull)
        {
            if (_magazine.FullReload)
            {
                OnReloading(new EventArgs());
                ammoState.CurrentAmmo = ammoState.MaxAmmo;
                LastAction = Time.time;
            }
            else
            {
                ammoState.CurrentAmmo++;
                LastAction = Time.time;
                if (FirstReload)
                    OnReloadingStarted(new EventArgs());
                else if (ammoState.IsAmmoFull)
                    OnReloadingEnded(new EventArgs());
                else
                    OnReloading(new EventArgs());

                FirstReload = false;
            }
        }
    }

    
    public void Fire(Vector3 spawnPosition, Quaternion orientation)
    {
        const float MaxBulletTravel = 1024f;
        if (CanFire)
        {
            if (!ammoState.HasAmmo)
            {
                OnFiredEmpty(new EventArgs());
                LastAction = Time.time;
            }
            else
            {
                _debugs = new Ray[_firingInfo.Pellets];
                for (var p = 0; p < _firingInfo.Pellets; p++)
                {
                    var spread = _useRandomSpread ? RandomSpread : GetUniformSpread(p, _firingInfo.Pellets);
                    var forward = _aimCone.CalculateSpreadedForward(spread, orientation);
                    _debugs[p] = new Ray(spawnPosition, forward);
                    if (Physics.Raycast(spawnPosition, forward, out var hitInfo, MaxBulletTravel))
                    {
                        if (hitInfo.rigidbody != null)
                        {
                            var shootable = hitInfo.rigidbody.GetComponent<Shootable>();
                            if (shootable != null)
                            {
                                shootable.TakeShot(hitInfo.point, forward, hitInfo.normal);
                            }
                            else
                                Debug.Log($"Pellet {p} Hit!");
                        }
                        else
                            Debug.Log($"Pellet {p} Hit!");
                    }
                }

                OnFired(new FiredEventArgs() {Raycasts = _debugs});
                ammoState.CurrentAmmo--;
                LastAction = Time.time;
                FirstReload = true;
            }
        }
    }


    public event EventHandler Reloading;
    public event EventHandler ReloadingStarted;
    public event EventHandler ReloadingEnded;
    public event EventHandler<FiredEventArgs> Fired;
    public event EventHandler FiredEmpty;


    public void Initialize()
    {
        LastAction = Mathf.NegativeInfinity;
        FirstReload = true;
        ammoState.CurrentAmmo = ammoState.MaxAmmo;
        // _rayCache = new Ray[_firingInfo.Pellets];
    }

    public void Initialize(GunData data)
    {
        _aimCone = data.AimCone;
        _firingInfo = data.FiringInfo;
        _magazine = data.MagazineInfo;
        ammoState.CopyFrom(data.AmmoState);
        Initialize();
    }

    private Ray[] _debugs;
    public Ray[] DebugRays => _debugs;
    
    public void OnDrawGizmos()
    {
        const float DrawRange = 10f;
        const float ImpactSize = 0.1f;
        if (_debugs == null)
            return;
        for (var i = 0; i < _debugs.Length; i++)
        {
            var ray = _debugs[i];
            if (i != 0)
            {
                var hue = 1f / (_debugs.Length - 1) * (i - 1);
                Gizmos.color = Color.HSVToRGB(hue, 1f, 1f);
            }
            else
                Gizmos.color = Color.white;

            {
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * DrawRange);
                Gizmos.DrawSphere(ray.origin + ray.direction * DrawRange, ImpactSize);
            }
        }
    }

    protected virtual void OnReloading(EventArgs args)
    {
        Reloading?.Invoke(this, args);
    }

    protected virtual void OnFired(FiredEventArgs args)
    {
        Fired?.Invoke(this, args);
    }

    protected virtual void OnReloadingStarted(EventArgs args)
    {
        ReloadingStarted?.Invoke(this, args);
    }

    protected virtual void OnReloadingEnded(EventArgs args)
    {
        ReloadingEnded?.Invoke(this, args);
    }

    protected virtual void OnFiredEmpty(EventArgs args)
    {
        FiredEmpty?.Invoke(this, args);
    }
}