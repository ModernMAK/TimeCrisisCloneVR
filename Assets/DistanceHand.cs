using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class DistanceHand : MonoBehaviour
{
    [SerializeField] private Transform _raycaster;
    public float startFlick = 1.5f;
    public float endFlick = 0.5f;
    private bool _hasFlicked;
    private Hand _hand;

    [Range(0f,1f)]
    [SerializeField] private float _speedToTimeBlend = 0.5f;
    [SerializeField] private float maxSpeed = 2.5f;
    [SerializeField] private float maxTime = 0.75f;
    [SerializeField]
    private SteamVR_Action_Boolean _confirmPull;

    private bool _isGripped;
    [SerializeField] private bool useBefore;

    private Throwable _targetThrowable;
    private Vector3 _startFlickVelocity;
    private Vector3 _startFlickAngularVelocity;
    private void Awake()
    {
        _hand = GetComponent<Hand>();
        if(_confirmPull != null)
        {
            _confirmPull.AddOnChangeListener(ConfirmPull, _hand.handType);
        }
    }

    public bool FlickStarted => !_hasFlicked && _hand.GetTrackedObjectAngularVelocity().magnitude > startFlick;
    public bool FlickEnded => _hasFlicked && _hand.GetTrackedObjectAngularVelocity().magnitude < endFlick;

    private void ConfirmPull(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        _isGripped = newState;
        if (_isGripped)
        {
            if (_hand.AttachedObjects.Count > 0)
                _targetThrowable = null;
            else if (Physics.Raycast(_raycaster.position, _raycaster.forward, out var hitinfo, 100f))
            {
                var rb = hitinfo.rigidbody;
                if(rb != null)
                    _targetThrowable = rb.GetComponent<Throwable>();
            }
            else
                _targetThrowable = null;
        }
        else
            _targetThrowable = null;
    }

    private void Update()
    {
        var flickDone = false;
        var flickBegun = false;
        if (FlickStarted)
        {
            flickBegun = true;
            _hasFlicked = true;
        }
        else if(FlickEnded)
        {
            _hasFlicked = false;
            flickDone = true;
        }
        if (_targetThrowable != null && (useBefore ? flickBegun : flickDone))
        {
            var r =_targetThrowable.GetComponent<Rigidbody>();
            var start = r.transform.position;
            var end = _hand.objectAttachmentPoint.position;
            var blendedTime = ProjectileMotion.CalculateBlendedTime(end, start, maxSpeed, maxTime, _speedToTimeBlend);
            var v = ProjectileMotion.CalculateVelocityFromTime(end,start,blendedTime);
             var av = AngularVelocityHelper.CalculateVelocity(_hand.objectAttachmentPoint.rotation,r.transform.rotation, blendedTime);
            r.velocity = v;
             r.angularVelocity = av;
            _targetThrowable = null;
        }
    }
}
