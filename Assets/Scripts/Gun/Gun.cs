using System;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class Gun : IGun
{
    public Gun()
    {
        _ammoState = new AmmoState(this);
        _reloadingState = new ReloadingState(this);
    }

    protected Gun(IGun attachedGun)
    {
        _ammoState = new AmmoState(attachedGun);
        _reloadingState = new ReloadingState(attachedGun);
    }

    [SerializeField] private AmmoState _ammoState;
    [SerializeField] private ReloadingState _reloadingState;
    [SerializeField] private AimCone _aimCone;
    [SerializeField] private FiringInfo _firingInfo;
    [SerializeField] private float _lastAction;

    public float LastAction
    {
        get => _lastAction;
        private set => _lastAction = value;
    }

    [SerializeField] private bool _useRandomSpread = false;

    public AmmoState AmmoState => _ammoState;
    public ReloadingState ReloadingState => _reloadingState;

    public bool CanFire => (_firingInfo.FireCooldown + LastAction - Time.time <= 0f);


    public void Reload()
    {
        _reloadingState.StartReloading();
        _reloadingState.PerformReload(ref _lastAction, AmmoState);
    }


    public void Fire(Vector3 spawnPosition, Quaternion orientation)
    {
        const float MaxBulletTravel = 1024f;
        if (CanFire)
        {
            if (!_ammoState.HasAmmo)
            {
                OnFiredEmpty(new EventArgs());
                LastAction = Time.time;
                _reloadingState.StopReloading();
            }
            else
            {
                _debugs = new Ray[_firingInfo.Pellets];
                for (var p = 0; p < _firingInfo.Pellets; p++)
                {
                    var spread = _useRandomSpread
                        ? AimCone.RandomSpread
                        : AimCone.GetUniformSpread(p, _firingInfo.Pellets);
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
                _ammoState.CurrentAmmo--;
                _reloadingState.StopReloading();
                LastAction = Time.time;
            }
        }
    }

    public event EventHandler<FiredEventArgs> Fired;
    public event EventHandler FiredEmpty;


    public void Initialize()
    {
        LastAction = Mathf.NegativeInfinity;
        _ammoState.CurrentAmmo = _ammoState.MaxAmmo;    
    }

    public void Initialize(GunData data)
    {
        _aimCone = data.AimCone;
        _firingInfo = data.FiringInfo;
        _ammoState.CopyFrom(data.AmmoData);
        _reloadingState.CopyFrom(data.ReloadingData);
        _reloadingState.StopReloading();
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


    protected virtual void OnFired(FiredEventArgs args)
    {
        Fired?.Invoke(this, args);
    }


    protected virtual void OnFiredEmpty(EventArgs args)
    {
        FiredEmpty?.Invoke(this, args);
    }
}