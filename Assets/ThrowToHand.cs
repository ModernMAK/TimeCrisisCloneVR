
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public static class AngularVelocityHelper
{
    public static Vector3 CalculateVelocity(Quaternion target, Quaternion start, float time)
    {
        var deltaRotation = target * Quaternion.Inverse(start);
        deltaRotation.ToAngleAxis(out var angleInDegrees, out var rotationAxis);
 
        var angularDisplacement = rotationAxis * (angleInDegrees * Mathf.Deg2Rad);    
        var angularSpeed = angularDisplacement / time;
        // var 
        
        // btQuaternion diffQuater = gyroQuater - boxQuater;
        // btQuaternion diffConjQuater;
        // velQuater = ((diffQuater * 2) / d_time) * conjBoxQuater;
        // var deltaRotation = Quaternion.
        // var deltaEuler = deltaRotation.eulerAngles * Mathf.Deg2Rad;
        // var velocity = deltaEuler / time;
        return angularSpeed;
    }
}
public static class ProjectileMotion
{
    
    public static Vector3 CalculateVelocity(Vector3 delta, Vector3 gravity, float time)
    {
        return delta / time + -gravity * (0.5f * time);
    }
    public static Vector3 CalculateVelocityFromMaxSpeed(Vector3 target, Vector3 start, float speed = 1f)
    {
        var delta = (target - start);
        var time = delta.magnitude / speed;
        return CalculateVelocity(delta, Physics.gravity, time);
    }
    public static Vector3 CalculateVelocityFromTime(Vector3 target, Vector3 start, float time = 1f)
    {
        var delta = (target - start);
        return CalculateVelocity(delta, Physics.gravity, time);
    }
    public static Vector3 CalculateVelocityFromMaxSpeedAndTime(Vector3 target, Vector3 start, float speed, float time, float blend = 0.5f)
    {
        var delta = (target - start);
        var speedTime = delta.magnitude / speed;
        var blendTime = Mathf.Lerp(speedTime, time, blend);
        return CalculateVelocity(delta, Physics.gravity, blendTime);
    }
    public static float CalculateBlendedTime(Vector3 target, Vector3 start, float speed, float time, float blend = 0.5f)
    {
        var delta = (target - start);
        var speedTime = delta.magnitude / speed;
        var blendTime = Mathf.Lerp(speedTime, time, blend);
        return blendTime;
    }

    
    // private Vector3 CalculateVelocityFromMaxLateralSpeed(Vector3 target, Vector3 start, float lateralSpeed = 1f)
    // {
    //     Vector3 grav = Physics.gravity;
    //     Vector3 delta = (target - start);
    //     Vector3 deltaOffGrav = Vector3.ProjectOnPlane(delta,grav);
    //     var time = deltaOffGrav.magnitude / lateralSpeed;
    //     return CalculateVelocity(delta, grav, time);
    // }
    // private Vector3 GetProjectileMotionSmartLateralSpeed(Vector3 target, Vector3 start, float lateralSpeed = 1f)
    // {
    //     Vector3 grav = Physics.gravity;
    //     Vector3 delta = (target - start);
    //     Vector3 deltaOffGrav = Vector3.ProjectOnPlane(delta,grav);
    //     Vector3 deltaOnGrav = Vector3.Project(delta,grav);
    //     float time;
    //     if (deltaOnGrav.sqrMagnitude > deltaOffGrav.sqrMagnitude)
    //         time = delta.magnitude / lateralSpeed;
    //     else
    //         time = deltaOffGrav.magnitude / lateralSpeed;
    //     return CalculateVelocity(delta, grav, time);
    // }
}
public class ThrowToHand : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Boolean launch;
    [SerializeField] private SteamVR_Action_Boolean reset;
    [SerializeField] private Hand hand;

    [SerializeField] private bool _useSpeed = true;
    [SerializeField] private float _arcTime = 0.75f;
    [SerializeField] private float _speed = 2.5f;
    
    private Vector3 _originalPos;
    private Quaternion _originalRotation;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _originalPos = transform.position;
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        if(launch.GetStateDown(SteamVR_Input_Sources.Any))
            LaunchToHand();
        else if(reset.GetStateDown(SteamVR_Input_Sources.Any))
            ResetObj();
        
    }

    void ResetObj()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.position = _originalPos;
        transform.rotation = _originalRotation;

    }

    void LaunchToHand()
    {
        var target = hand.objectAttachmentPoint.position;
        var start = transform.position;
        var desiredV =
            _useSpeed
                ? ProjectileMotion.CalculateVelocityFromMaxSpeed(target, start, _speed)
                : ProjectileMotion.CalculateVelocityFromTime(target, start, _arcTime);
        _rigidbody.velocity = desiredV;

    }
}
