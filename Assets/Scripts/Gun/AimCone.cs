using System;
using UnityEngine;

[Serializable]
public struct AimCone
{
    public AimCone(float effectiveRange, float effectiveSpread)
    {
        _cone = new Vector3(effectiveSpread, effectiveSpread, effectiveRange);
    }

    public AimCone(float effectiveRange, float effectiveSpread, float effectiveClimb)
    {
        _cone = new Vector3(effectiveSpread, effectiveClimb, effectiveRange);
    }

    public AimCone(Vector3 cone)
    {
        _cone = cone;
    }

    public float EffectiveSpread => _cone.x;
    public float EffectiveClimb => _cone.y;
    public float EffectiveRange => _cone.z;
    [SerializeField] private Vector3 _cone;
    public Vector3 Cone => _cone;

    public Vector3 CalculateSpreadedForward(Vector2 spread, Quaternion rotation = default) =>
        CalculateSpreadedForward(Cone, spread, rotation);

    public static Vector3 CalculateSpreadedForward(Vector3 cone, Vector2 spread, Quaternion rotation = default)
    {
        var spreadCone = new Vector3(cone.x * spread.x, cone.y * spread.y, cone.z);
        return rotation * spreadCone;
    }

    public static Vector2 RandomSpread => UnityEngine.Random.insideUnitCircle;

    public static Vector2 GetUniformSpread(int pellet, int totalPellets, int rings = 2)
    {
        if (pellet == 0)
            return Vector2.zero; //No spread for uniform first pellet
        //Correct i for remaining pellets
        totalPellets--;
        pellet--;
        var n_per_r = totalPellets / rings; //# per ring
        var extra_n = totalPellets % n_per_r;
        var r = (pellet / n_per_r); //ring
        const float TwoPi = Mathf.PI * 2f;
        float percent;
        float scale;
        if (r >= rings - 1)
        {
            scale = 1f;
            var i_r = pellet - (n_per_r * rings);
            percent = (float) i_r / (n_per_r + extra_n);
        }
        else
        {
            scale = (float) (r + 1) / rings;
            var i_r = pellet % n_per_r;
            percent = (float) i_r / n_per_r;
        }

        return new Vector2(Mathf.Cos(percent * TwoPi), Mathf.Sin(percent * TwoPi)) * scale;
    }
}