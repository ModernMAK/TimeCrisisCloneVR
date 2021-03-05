using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using Valve.VR.InteractionSystem;


public class GunBehaviour : MonoBehaviour, IGun
{
#pragma warning disable 649
    private Gun _gun;
    [SerializeField] private GunData _gunData;
    [SerializeField] private Transform _transform;

// [Header("Options")]
//     [SerializeField] private bool _allowTargetingAssist;
    // [Header("Legacy PC")] [SerializeField] private KeyCode _reloadCode = KeyCode.R;
    // [SerializeField] private KeyCode _fireCode = KeyCode.Mouse0;
    // [SerializeField] private KeyCode _stopCode = KeyCode.S;
    //
    // [Header("PC")] [SerializeField] private InputAction _reloadAction;
    // [SerializeField] private InputAction _fireAction;
    // [SerializeField] private InputAction _stopAction;
    //
    // [Header("SteamVR")] [SerializeField] private SteamVR_Action_Boolean _reloadSVRAction;
    // [SerializeField] private SteamVR_Action_Boolean _fireSVRAction;
    // [SerializeField] private SteamVR_Action_Boolean _stopSVRAction;
    // private Interactable _interactable;


#pragma warning restore 649

    private void Awake()
    {
        _gun = new Gun();
        // _interactable = GetComponent<Interactable>();
    }

    private void Start()
    {
        _gun.Initialize(_gunData);
    }

    // public bool IsHeld => _interactable != null && _interactable.attachedToHand != null;

//     public bool Get(KeyCode legacy, SteamVR_Action_Single steamvr, InputAction action)
//     {
// #if ENABLE_INPUT_SYSTEM
//         if (action.ReadValue<float>() >= 0.5f)
//             return true;
// #elif ENABLE_LEGACY_INPUT_MANAGER
//         if (Input.GetKey(legacy))
//             return true;
// #endif
//         
// #if ENABLE_VR
//         if (IsHeld)
//         {
//             Debug.Log("held");
//             if (steamvr.GetAxis(_interactable.attachedToHand.handType) >= 0.5f)
//             {
//                 Debug.Log("pressed");
//                 
//                 return true;
//             }
//         }
// #endif
//         return false;
//         
//     }
//     public bool Get(KeyCode legacy, SteamVR_Action_Boolean steamvr, InputAction action)
//     {
//         #if ENABLE_INPUT_SYSTEM
//         if (action.ReadValue<float>() >= 0.5f)
//             return true;
//         #elif ENABLE_LEGACY_INPUT_MANAGER
//         if (Input.GetKey(legacy))
//             return true;
//         #endif
//         
//         #if ENABLE_VR
//         if (steamvr != null)
//         {
//             if (IsHeld)
//             {
//                 if (steamvr.GetState(_interactable.attachedToHand.handType))
//                 {
//                     return true;
//                 }
//             }
//         }
// #endif
//         return false;
//     }
//     
    
    
    // Update is called once per frame
    // void Update()
    // {
    //     if (Get(_stopCode,_stopSVRAction,_stopAction))
    //     {
    //         _gun.ReloadingState.StopReloading();
    //     }
    //
    //     if (Get(_reloadCode,_reloadSVRAction,_reloadAction))
    //     {
    //         _gun.ReloadingState.StartReloading();
    //     }
    //     else if (Get(_fireCode,_fireSVRAction,_fireAction))
    //     {
    //         Fire(_transform.position, _transform.rotation);
    //         _gun.ReloadingState.StopReloading();
    //     }
    //     else if (_gun.ReloadingState.IsReloading)
    //     {
    //         Reload();
    //     }
    // }

    private void OnDrawGizmos()
    {
        if(_gun != null)
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
    public void Fire() => Fire(_transform.position, _transform.rotation);

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