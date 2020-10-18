using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum GunActionState
{
    Idle,
    Firing,
    Reloading,
}

public class FiniteStateMachine<TState>
{
    public class StateInfo
    {
        public StateInfo()
        {
            Enter = new Dictionary<TState, Action>();
            Exit = new Dictionary<TState, Action>();
            TransitionPredicates = new Dictionary<TState, Func<bool>>();
        }

        public Action Update { get; private set; }
        public Dictionary<TState, Action> Enter { get; private set; }
        public Dictionary<TState, Action> Exit { get; private set; }
        public Dictionary<TState, Func<bool>> TransitionPredicates { get; private set; }

        public void PerformUpdate()
        {
            if (Update != null)
                Update();
        }

        public void PerformEnter(TState oldState)
        {
            if (Enter.TryGetValue(oldState, out var action)) action();
        }

        public void PerformExit(TState newState)
        {
            if (Exit.TryGetValue(newState, out var action)) action();
        }

        public bool CheckTransition(out TState newState)
        {
            foreach (var kvp in TransitionPredicates)
            {
                var state = kvp.Key;
                var pred = kvp.Value;
                if (pred())
                {
                    newState = state;
                    return true;
                }
            }

            newState = default;
            return false;
        }

        public StateInfo SetUpdate(Action action)
        {
            Update = action;
            return this;
        }

        public StateInfo SetEnter(TState enter, Action action)
        {
            Enter[enter] = action;
            return this;
        }

        public StateInfo SetExit(TState exit, Action action)
        {
            Exit[exit] = action;
            return this;
        }

        public StateInfo SetTransition(TState transition, Func<bool> predicate)
        {
            TransitionPredicates[transition] = predicate;
            return this;
        }
    }

    private Dictionary<TState, StateInfo> _stateTable;
    private TState _currentState;
    public StateInfo CurrentStateInfo => _stateTable[_currentState];

    public FiniteStateMachine()
    {
        _stateTable = new Dictionary<TState, StateInfo>();
        _currentState = default;
    }

    public StateInfo RegisterState(TState state)
    {
        if (_stateTable.TryGetValue(state, out var info) && info != null)
            return _stateTable[state] = new StateInfo();
        else
            throw new InvalidOperationException("State already registered.");
    }

    //Allow forced state changes
    public void ChangeState(TState newState)
    {
        if (newState.Equals(_currentState))
            return;
        CurrentStateInfo.PerformExit(newState);
        var oldState = _currentState;
        _currentState = newState;
        CurrentStateInfo.PerformExit(oldState);
    }

    public void UpdateStateAndTransition()
    {
        UpdateState();
        UpdateTransition();
    }

    public void UpdateState() => CurrentStateInfo.Update();

    public void UpdateTransition()
    {
        if (CurrentStateInfo.CheckTransition(out var newState))
            ChangeState(newState);
    }
}

public class GunFiniteStateMachine : GunState
{
    private FiniteStateMachine<GunActionState> _internalStateMachine;

    void Initialize()
    {
        _internalStateMachine.RegisterState(GunActionState.Idle)
            .SetEnter(GunActionState.Reloading, Reloading_Enter_Idle)
            .SetEnter(GunActionState.Firing, Reloading_Enter_Idle)
            .SetUpdate(Update_Idle);

        // _internalStateMachine.RegisterState(GunActionState.Firing)
        //     .SetEnter()
    }

    void Update_Idle()
    {
    }

    void Reloading_Enter_Idle()
    {
        AttachedGun.ReloadingState.StopReloading();
    }

    public GunFiniteStateMachine(IGun gun) : base(gun)
    {
        _internalStateMachine = new FiniteStateMachine<GunActionState>();
        Initialize();
    }
}

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
            if (!_ammoState.HasAmmo && !GlobalSettings.CheatCodes.InfiniteAmmo)
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
                if (!GlobalSettings.CheatCodes.InfiniteAmmo)
                {
                    _ammoState.CurrentAmmo--;
                    _reloadingState.StopReloading();
                }
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