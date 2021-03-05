using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class VrOffset : MonoBehaviour
{
    private static Transform container;
    [Tooltip("The Root Transform")]
    public Transform root;
    [Tooltip("The Grip Location")]
    public Transform offset;
    [Tooltip("The VR Transform")]
    public Transform vr;
    private void Awake()
    {
        if (root == null)
            root = transform;
        
        if (container == null)
        {
            const string name = "VR Offset Container";
            container = GameObject.Find(name)?.transform;
            if(container == null)
                container = new GameObject("VR Offset Container").transform;
        }
    }


    public void GetDeltaPositionAndRotation(Transform rootTransform, Transform childTransform, out Vector3 deltaPosition,
        out Quaternion deltaRotation)
    {
        if (rootTransform == null || childTransform == null)
            throw new NullReferenceException("Root and Child transforms cannot be null!");
        if (rootTransform == childTransform) //Garunteed No Delta
        {
            deltaPosition = Vector3.zero;
            deltaRotation = Quaternion.identity;
        }
        else
        {
            var originalRot = rootTransform.rotation;
            var fwd = rootTransform.InverseTransformDirection(childTransform.forward);
            var up = rootTransform.InverseTransformDirection(childTransform.up);
            deltaRotation = Quaternion.LookRotation(fwd, up);
            deltaPosition = rootTransform.InverseTransformPoint(childTransform.position);
        }
    }
    
    
    private void OnEnable()
    {
        vr.SetParent(container,false);
        vr.name = $"{name} (VR Offset)";
        
        if(vr == root || vr == offset)
            throw new NotSupportedException("VR Transform cannot be Root or Grip!");
        GetDeltaPositionAndRotation(root, offset, out var dP, out var dR);
        vr.SetPositionAndRotation(dP,dR);

    }
    
}
