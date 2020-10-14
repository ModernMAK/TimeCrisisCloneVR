using System;
using UnityEngine;

[Serializable]
public struct AimCone
{
    public AimCone(float effectiveRange, float effectiveSpread)
    {
        _cone = new Vector3(effectiveSpread,effectiveSpread,effectiveRange);
    }
    public AimCone(float effectiveRange, float effectiveSpread, float effectiveClimb)
    {
        _cone = new Vector3(effectiveSpread,effectiveClimb,effectiveRange);
    }
    public AimCone(Vector3 cone)
    {
        _cone = cone;
    }
    public float EffectiveSpread => _cone.x;
    public float EffectiveClimb => _cone.y;
    public float EffectiveRange => _cone.z;
    [SerializeField]
    private Vector3 _cone;
    public Vector3 Cone => _cone;

    public Vector3 CalculateSpreadedForward(Vector2 spread, Quaternion rotation = default) =>
        CalculateSpreadedForward(Cone, spread, rotation);
    
    public static Vector3 CalculateSpreadedForward(Vector3 cone, Vector2 spread, Quaternion rotation = default)
    {
        var spreadCone = new Vector3(cone.x * spread.x, cone.y * spread.y, cone.z);
        return rotation * spreadCone;
    }
}