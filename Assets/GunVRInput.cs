using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(IGun))]
public class GunVRInput : MonoBehaviour
{

    [Header("SteamVR Bindings")]
    [SerializeField] private SteamVR_Action_Boolean _reloadAction;
    [SerializeField] private SteamVR_Action_Boolean _fireAction;
    [SerializeField] private SteamVR_Action_Boolean _stopAction;

    [SerializeField]
    private Interactable _interactable;

    private IGun _gunBehaviour;
    public bool IsHeld => _interactable.attachedToHand != null;
    public SteamVR_Input_Sources Source => _interactable.attachedToHand.handType;
    private void Awake()
    {
        if (_interactable == null)
            _interactable = GetComponent<Interactable>();
        if (_interactable == null)
            enabled = false;
        _gunBehaviour = GetComponent<IGun>();
    }

    void Update()
    {
        if (!IsHeld)
            return;

        if (_stopAction.GetState(Source))
        {
            _gunBehaviour.Stop();
        }
        if (_reloadAction.GetState(Source))
        {
            _gunBehaviour.Reload();
        }
        if (_fireAction.GetState(Source))
        {
            _gunBehaviour.Fire();
        }

    }
}


public static class InputActionHelper
{
    public static bool ReadBool(this InputAction action, float threshold = 0.5f) => action.ReadValue<float>() >= threshold; 
}
