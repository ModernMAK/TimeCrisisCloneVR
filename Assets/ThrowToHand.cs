
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ThrowToHand : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Boolean launch;
    [SerializeField] private SteamVR_Action_Boolean reset;
    [SerializeField] private Hand hand;

    [SerializeField] private bool _useSpeed = true;
    [SerializeField] private bool _useLateral = true;
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

    // private Vector3 GetProjectileMotion(Vector3 target, Vector3 start, float angleDeg)
    // {
    //     Vector3 dir = target - start; // get Target Direction
    //     float height = dir.y; // get height difference
    //     dir.y = 0; // retain only the horizontal difference
    //     float dist = dir.magnitude; // get horizontal direction
    //     float a = angleDeg * Mathf.Deg2Rad; // Convert angle to radians
    //     dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle.
    //     dist += height / Mathf.Tan(a); // Correction for small height differences
    //
    //     // Calculate the velocity magnitude
    //     float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
    //     return velocity * dir.normalized; 
    // }
    private Vector3 GetProjectileMotionVelocitySpeed(Vector3 target, Vector3 start, float speed = 1f)
    {
        // var delta = target - start;
        // var groundDelta = Vector3.ProjectOnPlane(delta, Vector3.forward);
        // var rot= Quaternion.FromToRotation(groundDelta.normalized,delta.normalized);
        // return GetProjectileMotion(target, start, rot.eulerAngles.y);
        
        Vector3 delta = (target - start);
        var time = delta.magnitude / speed;
        return GetProjectileMotion(delta, Physics.gravity, time);
    }
    private Vector3 GetProjectileMotionTime(Vector3 target, Vector3 start, float time = 1f)
    {
        // var delta = target - start;
        // var groundDelta = Vector3.ProjectOnPlane(delta, Vector3.forward);
        // var rot= Quaternion.FromToRotation(groundDelta.normalized,delta.normalized);
        // return GetProjectileMotion(target, start, rot.eulerAngles.y);
        
        Vector3 delta = (target - start);
        return GetProjectileMotion(delta, Physics.gravity, time);
    }

    
    private Vector3 GetProjectileMotionLateralSpeed(Vector3 target, Vector3 start, float lateralSpeed = 1f)
    {
        Vector3 grav = Physics.gravity;
        Vector3 delta = (target - start);
        Vector3 deltaOffGrav = Vector3.ProjectOnPlane(delta,grav);
        var time = deltaOffGrav.magnitude / lateralSpeed;
        return GetProjectileMotion(delta, grav, time);
    }
    private Vector3 GetProjectileMotion(Vector3 delta, Vector3 grav, float time)
    {
        return delta / time + -grav * (0.5f * time);
    }
    void LaunchToHand()
    {
        // var delta = hand.transform.position - transform.position;
        // var yTime = Mathf.Sqrt(delta.y / Physics.gravity.y);
        //
        // var desiredV = Vector3.up * yTime * Physics.gravity.y + delta;
        //
        
        // var p1 = Vector3.zero;
        // var p2 = hand.transform.position - transform.position;
        //
        // var dY = p2.y;
        // var dXZ = new Vector2(p2.x,p2.z);
        //
        // var time = sqrt(2 * (P2.y - P1.y*P2.x/P1.x)/(g.y*(1 - P1.x/P2.x)))
        // //v = P2/t2 - t2 * g/2;
        //
        // var start = transform.position;
        // var dest = hand.transform.position;
        // var delta = dest - start;
        // var xz = Vector3.ProjectOnPlane(delta, Vector3.up);
        // var y = Vector3.Project(delta, Vector3.up);
        // var ty = y.y / 9.8f; //Gravity (m / (m/s) = s)
        // Debug.Log(ty);
        //

        // var desiredV = (y / ty + xz);
        // var extraY = Vector3.zero;//Vector3.up * _additionalHeight;
        var desiredV = 
            _useSpeed ? 
                (
                    _useLateral ? 
                    GetProjectileMotionLateralSpeed(hand.objectAttachmentPoint.position, transform.position, _speed) : 
                    GetProjectileMotionVelocitySpeed(hand.objectAttachmentPoint.position,transform.position,_speed)
                ) :
                GetProjectileMotionTime(hand.objectAttachmentPoint.position, transform.position, _arcTime);
        // var desiredV = GetProjectileMotion(hand.objectAttachmentPoint.position, transform.position,_maxSpeed);
        _rigidbody.velocity = desiredV;

    }
}
