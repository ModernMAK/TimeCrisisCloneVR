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
    }

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
        if (_controls.Gun.Fire.ReadBool())
        {
            _gunBehaviour.Fire();
            Debug.Log("Fire");
        }

    }
}
