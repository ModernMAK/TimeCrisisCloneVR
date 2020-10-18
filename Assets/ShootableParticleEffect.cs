using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Shootable))]
public class ShootableParticleEffect : MonoBehaviour
{
    private struct PooledPrefab
    {
        public GameObject GameObject;
        public ParticleSystem ParticleSystem;

    }
    #pragma warning disable 649
    [SerializeField]
    private GameObject _particleSystemPrefab;

    private Queue<PooledPrefab> _pool;
    private Transform _poolContainer;
    #pragma warning restore 649
    
    [Range(0f,1f)]
    [SerializeField] private float _surfaceDampening = 0.5f;   
    [Range(0f,1f)]
    [SerializeField] private float _randomIntensity = 0.5f;    
    private void Start()
    {
        _pool = new Queue<PooledPrefab>();
        _poolContainer = new GameObject("Pool Container").transform;
        _poolContainer.SetParent(transform);
        var shootable = GetComponent<Shootable>();
        shootable.Shot += ShootableOnShot;
    }

    private PooledPrefab GetPair()
    {
        if(_pool.Count > 0)
            return _pool.Dequeue();
        var go = Instantiate(_particleSystemPrefab, _poolContainer, true);
        var ps = go.GetComponent<ParticleSystem>();
        return new PooledPrefab()
        {
            GameObject = go,
            ParticleSystem = ps,
        };
    }

    private void ShootableOnShot(object sender, ShotEventArgs e)
    {
        var item = GetPair();
        
        var go = item.GameObject;
        go.transform.position = e.HitPosition;
        var spreadDir = Vector3.ProjectOnPlane(e.ImpactDirection, e.ImpactNormal);
        var reflectDir = e.ImpactDirection - spreadDir;
        var randomDir =  Random.insideUnitSphere;
        go.transform.rotation = Quaternion.LookRotation((spreadDir  - reflectDir * _surfaceDampening) * (1 - _randomIntensity) + randomDir * _randomIntensity);
        go.SetActive(true);
        
        
        var ps = item.ParticleSystem;
        ps.Play();
        StartCoroutine(WaitForFX(item));
    }

    IEnumerator WaitForFX(PooledPrefab pair)
    {
        yield return new WaitForSeconds(pair.ParticleSystem.main.duration*2f);
        pair.GameObject.SetActive(false);
        _pool.Enqueue(pair);
    }
}
