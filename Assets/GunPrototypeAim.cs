using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPrototypeAim : MonoBehaviour
{
    [Serializable]
    public struct GunGameObjectInfo
    {
        public GameObject Root;
        // public GameObject Barrel;

        public void SetActive(bool active) => Root.SetActive(active);
    }

    #pragma warning disable 649
    [SerializeField] private KeyCode _prev;
    [SerializeField] private KeyCode _next;

    [SerializeField] private GunGameObjectInfo[] _targets;
    [SerializeField] private int _currentGun;
    private int _prevGun;
    private Camera _camera;
    private const float MaxScanRange = 8f;
    #pragma warning restore 649
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        foreach(var target in _targets)
            target.SetActive(false);
        UpdateActiveGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_prev))
        {
            _currentGun--;
            _currentGun += _targets.Length;
            _currentGun %= _targets.Length;
            UpdateActiveGun();
        }

        if (Input.GetKeyDown(_next))
        {
            _currentGun++;
            _currentGun %= _targets.Length;
            UpdateActiveGun();
        }

        UpdateOrientation();
    }

    void UpdateActiveGun()
    {
        _targets[_prevGun].SetActive(false);
        _targets[_currentGun].SetActive(true);
        _prevGun = _currentGun;
    }

    void UpdateOrientation()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        Vector3 rayDelta;
        var origin = transform.position;
        if (Physics.Raycast(ray, out var hitinfo, MaxScanRange))
        {
            rayDelta = hitinfo.point - origin;
        }
        else
        {
            rayDelta = ray.origin + ray.direction * MaxScanRange - origin;
        }

        transform.rotation = Quaternion.LookRotation(rayDelta);


        // var origin = transform.position;
        // var barrel = _targets[_currentGun].Barrel.transform.position;
        // var barrelDelta = barrel - origin;
        //
        // var ray = _camera.ScreenPointToRay(Input.mousePosition);
        // Vector3 rayDelta;
        // if (Physics.Raycast(ray, out var hitinfo, MaxScanRange))
        // {
        //     rayDelta = hitinfo.point - origin;
        // }
        // else
        // {
        //     rayDelta = ray.origin + ray.direction * MaxScanRange - origin;
        // }
        //
        // var rotation = Quaternion.FromToRotation(rayDelta, barrelDelta);
        // transform.rotation = rotation;
    }
}