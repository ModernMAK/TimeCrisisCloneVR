using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LinarMapStep : MonoBehaviour
{
    [SerializeField]
    private LinearDrive _linearDrive;
    [SerializeField]
    private LinearMapping _mapping;

    [SerializeField] private Interactable _interactable;

    [SerializeField]
    [Min(0)]
    private int _additionalSteps = 0;
    
    [SerializeField] private bool _wasAttached;
    private void Start()
    {
        _linearDrive = GetComponent<LinearDrive>();
        _interactable = GetComponent<Interactable>();
        _mapping = _linearDrive.linearMapping;
        _linearDrive.maintainMomemntum = false;
    }

    public bool IsAttached => _interactable.attachedToHand != null;

    void Update()
    {
        if (_wasAttached && !IsAttached)
        {
            var linearStep = 1f / (1 + _additionalSteps);
            var i = _mapping.value / linearStep;
            i = Mathf.RoundToInt(i);
            _mapping.value = i * linearStep;
            
            if ( _linearDrive.repositionGameObject )
            {
                transform.position = Vector3.Lerp( _linearDrive.startPosition.position, _linearDrive.endPosition.position, _linearDrive.linearMapping.value );
            }
        }

        _wasAttached = IsAttached;
        
            

    }
}
