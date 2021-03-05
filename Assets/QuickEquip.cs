using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class QuickEquip : MonoBehaviour
{
    public Hand.AttachmentFlags flags;
    public Hand _hand;
    void Update()
    {
        _hand.AttachObject(gameObject, GrabTypes.Scripted, flags, gameObject.transform);
        _hand.HideGrabHint();
        enabled = false;
    }

}
