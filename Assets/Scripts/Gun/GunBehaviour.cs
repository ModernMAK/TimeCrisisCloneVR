using System;
using UnityEngine;


public class GunBehaviour : MonoBehaviour, IGun
{
#pragma warning disable 649
    [SerializeField] private Gun _gun;
    [SerializeField] private GunData _gunData;
    [SerializeField] private bool _allowTargetingAssist;

    [SerializeField] private KeyCode _reloadCode = KeyCode.R;
    [SerializeField] private KeyCode _fireButton = KeyCode.Mouse0;
    [SerializeField] private KeyCode _stopButton = KeyCode.S;

    [SerializeField] private Transform _transform;

#pragma warning restore 649

    private void Awake()
    {
        _gun = new Gun();
    }

    private void Start()
    {
        _gun.Initialize(_gunData);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(_stopButton))
        {
            _gun.ReloadingState.StopReloading();
        }

        if (Input.GetKey(_reloadCode))
        {
            _gun.ReloadingState.StartReloading();
        }
        else if (Input.GetKey(_fireButton))
        {
            Fire(_transform.position, _transform.rotation);
            _gun.ReloadingState.StopReloading();
        }
        else if (_gun.ReloadingState.IsReloading)
        {
            Reload();
        }
    }

    private void OnDrawGizmos()
    {
        _gun.OnDrawGizmos();
    }

    public AmmoState AmmoState => _gun.AmmoState;

    public ReloadingState ReloadingState => _gun.ReloadingState;

    public bool CanFire => _gun.CanFire;


    public void Reload()
    {
        _gun.Reload();
    }

    public void Fire(Vector3 spawnPosition, Quaternion orientation)
    {
        _gun.Fire(spawnPosition, orientation);
    }

    public event EventHandler<FiredEventArgs> Fired
    {
        add => _gun.Fired += value;
        remove => _gun.Fired -= value;
    }

    public event EventHandler FiredEmpty
    {
        add => _gun.FiredEmpty += value;
        remove => _gun.FiredEmpty -= value;
    }
}