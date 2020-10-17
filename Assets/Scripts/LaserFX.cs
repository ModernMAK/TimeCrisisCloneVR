using System;
using UnityEngine;

public class LaserFX : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineLength = 128f;
    [SerializeField] private LayerMask _targetLayers;
    #pragma warning restore 649
    private void Start()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.SetPosition(0, Vector3.zero);
    }

    //Cosmetic, so it should be in Update
    private void Update()
    {
        var tran = _lineRenderer.transform;
        var r = new Ray(
            tran.position,
            tran.forward);
        if (Physics.Raycast(r, out var hitInfo, _lineLength, _targetLayers))
        {
            var dist = (hitInfo.point - tran.position).magnitude;
            _lineRenderer.SetPosition(1, Vector3.forward * dist); 
            _lineRenderer.gameObject.SetActive(true);
        }
        else
        {
            _lineRenderer.SetPosition(1, Vector3.forward * _lineLength); 
        }

    }
}