using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _bulletEjectAnim;
    [SerializeField] private ParticleSystem _pelletAnim;

    void Start()
    {
        var gun = GetComponent<IGun>();
        gun.Fired += PlayBulletEject;
        gun.Fired += PlayPelletProjectile;
    }

    void PlayBulletEject(object sender, FiredEventArgs args) => _bulletEjectAnim.Emit(1);

    void PlayPelletProjectile(object sender, FiredEventArgs args)
    {
        const float Speed = 100f;
        var sharedArgs = new ParticleSystem.EmitParams();
        for (var i = 0; i < args.Raycasts.Length; i++)
        {
            sharedArgs.velocity = args.Raycasts[i].direction * Speed;
            _pelletAnim.Emit(sharedArgs, 1);                
        }

    }
}