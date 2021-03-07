using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Shootable))]
public class ShootableBulletHole : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private ParticleSystem _particleSystem;

    private Transform _transform;
#pragma warning restore 649

    private List<ShotEventArgs> cache;

    private void Start()
    {
        var shootable = GetComponent<Shootable>();
        shootable.Shot += ShootableOnShot;
        _transform = _particleSystem.transform;
        cache = new List<ShotEventArgs>();
    }

    // private void OnDrawGizmos()
    // {
    //     if (cache != null)
    //         for (var i = 0; i < cache.Count; i++)
    //         {
    //             var e = cache[i];
    //             Gizmos.color = Color.white;
    //             Gizmos.DrawSphere(e.HitPosition, 0.1f);
    //             Gizmos.color = Color.red;
    //             Gizmos.DrawRay(e.HitPosition, e.ImpactNormal);
    //             Gizmos.color = Color.yellow;
    //             Gizmos.DrawRay(e.HitPosition, e.ImpactDirection);
    //         }
    // }

    private void ShootableOnShot(object sender, ShotEventArgs e)
    {
        // cache.Add(e);
        var localPos = _transform.InverseTransformPoint(e.HitPosition + e.ImpactNormal*0.001f);
        var localNormal = _transform.InverseTransformDirection(e.ImpactNormal);
        // var randAngle = Random.value * 360f;
        // var randRotation = Quaternion.AngleAxis(randAngle, localNormal);
         var localRotation = Quaternion.LookRotation(localNormal);
        // var desiredRotation = localRotation * randRotation;
        // var localNormal = _transform.InverseTransformDirection(e.ImpactNormal);
        var p = new ParticleSystem.EmitParams()
        {
            // position = e.HitPosition, 
            // axisOfRotation = Quaternion.LookRotation(e.ImpactNormal),
            //
            position = localPos,
            axisOfRotation = localNormal,
            rotation3D = localRotation.eulerAngles
        };
        _particleSystem.Emit(p, 1);
    }
}