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

    [SerializeField] private bool _reloading;
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
            _reloading = false;
        }
        if (Input.GetKey(_reloadCode))
        {
            _reloading = true;
        }
        else if (Input.GetKey(_fireButton))
        {
            Fire(_transform.position, _transform.rotation);
            _reloading = false;
        }
        else

        if (_reloading)
        {
            Reload();
            _reloading = !CanReload;
        }
    }

    private void OnDrawGizmos()
    {
        _gun.OnDrawGizmos();
    }

    public AmmoState AmmoState => _gun.AmmoState;

    public bool CanFire => _gun.CanFire;

    public bool CanReload => _gun.CanReload;


    public void Reload()
    {
        _gun.Reload();
    }

    public void Fire(Vector3 spawnPosition, Quaternion orientation)
    {
        _gun.Fire(spawnPosition, orientation);
    }

    public event EventHandler Reloading
    {
        add => _gun.Reloading += value;
        remove => _gun.Reloading -= value;
    }
    public event EventHandler ReloadingStarted
    {
        add => _gun.ReloadingStarted += value;
        remove => _gun.ReloadingStarted -= value;
    }

    
    public event EventHandler ReloadingEnded
    {
        add => _gun.ReloadingEnded += value;
        remove => _gun.ReloadingEnded -= value;
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