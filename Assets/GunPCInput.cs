using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(IGun))]
public class GunPCInput : MonoBehaviour
{

    [Header("PC Bindings")]
    [SerializeField] private TimeCrisisCloneVR _controls;

    private IGun _gunBehaviour;
    private void Awake()
    {
        _gunBehaviour = GetComponent<IGun>();
        if(_controls == null)
            _controls = new TimeCrisisCloneVR();
        _controls.Gun.Enable();
        _prevFire = false;
    }
    private bool _prevFire;
    void Update()
    {
        if (_controls.Gun.Stop.ReadBool())
        {
            _gunBehaviour.Stop();
        }
        if (_controls.Gun.Reload.ReadBool())
        {
            _gunBehaviour.Reload();
        }
        var fire = _controls.Gun.Fire.ReadBool();
        if (fire != _prevFire)
        {
            if (fire)
            {
                _gunBehaviour.PressFire();
            }
            else
            {
                _gunBehaviour.ReleaseFire();
            }
            _prevFire = fire;
        }

    }
}
