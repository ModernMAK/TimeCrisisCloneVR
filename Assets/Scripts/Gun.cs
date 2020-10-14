using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FiredEventArgs : EventArgs
{
    public Ray[] Raycasts;
}

[Serializable]
public class Gun : IGun
{
    [Serializable]
    public class GunState
    {
        [SerializeField] private int _currentBullets;
        [SerializeField] private float _lastAction;
        [SerializeField] private bool firstReload;

        public int CurrentBullets
        {
            get => _currentBullets;
            set => _currentBullets = value;
        }

        public bool FirstReload
        {
            get => firstReload;
            set => firstReload = value;
        }

        public float LastAction
        {
            get => _lastAction;
            set => _lastAction = value;
        }
    }

    [SerializeField] private AimCone _aimCone;
    [SerializeField] private MagazineInfo _magazine;
    [SerializeField] private FiringInfo _firingInfo;
    [SerializeField] private GunState _state;
    [SerializeField] private bool _useRandomSpread = false;

    public bool CanFire => (_firingInfo.FireCooldown + _state.LastAction - Time.time <= 0f);

    public bool CanReload => ((_state.FirstReload ? _magazine.InitialReload : _magazine.AdditionalReload) +
        _state.LastAction - Time.time <= 0f);

    public bool HasBullets => _state.CurrentBullets > 0;
    public bool MagazineFull => (_state.CurrentBullets == _magazine.MagazineSize);
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
        if (CanReload && !MagazineFull)
        {
            if (_magazine.FullReload)
            {
                _state.CurrentBullets = _magazine.MagazineSize;
                _state.LastAction = Time.time;
                OnReloading(new EventArgs());
            }
            else
            {
                _state.CurrentBullets++;
                _state.LastAction = Time.time;
                if(_state.FirstReload)
                    OnReloadingStarted(new EventArgs());
                else if (MagazineFull)
                    OnReloadingEnded(new EventArgs());
                else
                    OnReloading(new EventArgs());
                _state.FirstReload = false;
            }
        }
    }

    public void Fire(Vector3 spawnPosition, Quaternion orientation)
    {
        const float MaxBulletTravel = 1024f;
        if (CanFire)
        {
            if (!HasBullets)
            {
                OnFiredEmpty(new EventArgs());
                _state.LastAction = Time.time;
            }
            else
            {
                _debugs = new Ray[_firingInfo.Pellets];
                for (var p = 0; p < _firingInfo.Pellets; p++)
                {
                    var spread = _useRandomSpread ? RandomSpread : GetUniformSpread(p, _firingInfo.Pellets);
                    var forward = _aimCone.CalculateSpreadedForward(spread, orientation);
                    _debugs[p] = new Ray(spawnPosition, forward);
                    if (Physics.Raycast(spawnPosition, forward, MaxBulletTravel))
                    {
                        Debug.Log($"Pellet {p} Hit!");
                    }
                }
                OnFired(new FiredEventArgs(){Raycasts = _debugs});
                _state.CurrentBullets--;
                _state.LastAction = Time.time;
                _state.FirstReload = true;
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
        _state.LastAction = Mathf.NegativeInfinity;
        _state.FirstReload = true;
        _state.CurrentBullets = _magazine.MagazineSize;
    }

    public void Initialize(GunData data)
    {
        _aimCone = data.AimCone;
        _firingInfo = data.FiringInfo;
        _magazine = data.MagazineInfo;
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
        Reloading?.Invoke(this,args);
    }

    protected virtual void OnFired(FiredEventArgs args)
    {
        Fired?.Invoke(this,args);
    }

    protected virtual void OnReloadingStarted(EventArgs args)
    {
        ReloadingStarted?.Invoke(this,args);
    }

    protected virtual void OnReloadingEnded(EventArgs args)
    {
        ReloadingEnded?.Invoke(this,args);
    }

    protected virtual void OnFiredEmpty(EventArgs args)
    {
        FiredEmpty?.Invoke(this,args);
    }
}