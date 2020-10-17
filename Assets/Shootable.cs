using System;
using UnityEngine;

public class ShotEventArgs : EventArgs
{
    public Vector3 HitPosition;
    public Vector3 ImpactDirection;
    public Vector3 ImpactNormal;
}
public class Shootable : MonoBehaviour
{
    public event EventHandler<ShotEventArgs> Shot;

    public void TakeShot(Vector3 point, Vector3 direction, Vector2 impactNormal) => OnShot(new ShotEventArgs()
        {
            HitPosition = point,
            ImpactDirection = direction,
            ImpactNormal = impactNormal
        });
    
    protected virtual void OnShot(ShotEventArgs e)
    {
        Shot?.Invoke(this, e);
    }
}
