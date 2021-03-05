using System.Collections.Generic;
using UnityEngine;

public class GunRayCaster : MonoBehaviour
{
    [SerializeField]
    private Transform _rayTransform;
    public Vector3 Origin => _rayTransform.position;
    public Vector3 Direction => _rayTransform.forward;
    public Quaternion Rotation => _rayTransform.rotation;
    public Ray PrimaryRay => new Ray(Origin, Direction);

    public Ray GetRay(AimCone cone, bool useRandomSpread = true)
	{
        var rays = GetRays(1, cone, 2, useRandomSpread);
        return rays[0];
	}
    public Ray[] GetRays(int pellets, AimCone cone, int rings = 2, bool useRandomSpread = true)
    {

        var rays = new Ray[pellets];
        var rotation = Rotation;
        var position = Origin;
        for (var p = 0; p < pellets; p++)
        {
            var spread = useRandomSpread
                ? AimCone.RandomSpread
                : AimCone.GetUniformSpread(p, pellets, rings);
            var forward = cone.CalculateSpreadedForward(spread, rotation);
            rays[p] = new Ray(position, forward);
        }
        return rays;
    }

    const float MaxBulletTravel = 1024f;

    public bool Cast(Ray ray, out RaycastHit hitInfo) => Physics.Raycast(ray, out hitInfo, MaxBulletTravel);
    public IEnumerable<RaycastHit> Cast(IEnumerable<Ray> rays)
    {
        foreach(var ray in rays)
            if (Cast(ray, out var hitInfo))
            {
                yield return hitInfo;
            }
    }

}
