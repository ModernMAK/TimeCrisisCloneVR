using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(GunBehaviour))]
public class GunVRInput : MonoBehaviour
{
    
    [Header("SteamVR Bindings")]
    [SerializeField] private SteamVR_Action_Boolean _reloadAction;
    [SerializeField] private SteamVR_Action_Boolean _fireAction;
    [SerializeField] private SteamVR_Action_Boolean _stopAction;
    
    [SerializeField]
    private Interactable _interactable;
    
    private GunBehaviour _gunBehaviour;
    public bool IsHeld => _interactable.attachedToHand != null;
    public SteamVR_Input_Sources Source => _interactable.attachedToHand.handType;
    private void Awake()
    {
        if(_interactable == null)
            _interactable = GetComponent<Interactable>();
        if (_interactable == null)
            enabled = false;
        _gunBehaviour = GetComponent<GunBehaviour>();
    }

    void Update()
    {
        if(!IsHeld)
            return;
    
        if (_stopAction.GetState(Source))
        {
            _gunBehaviour.ReloadingState.StopReloading();
        }
        
        if (_reloadAction.GetState(Source))
        {
            _gunBehaviour.ReloadingState.StartReloading();
        }
        else if (_fireAction.GetState(Source))
        {
            _gunBehaviour.Fire();
            _gunBehaviour.ReloadingState.StopReloading();
        }
        else if (_gunBehaviour.ReloadingState.IsReloading)
        {
            _gunBehaviour.Reload();
        }
        
    }
}
