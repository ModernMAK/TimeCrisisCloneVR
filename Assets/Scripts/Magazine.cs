using System;
using UnityEngine;

[Serializable]
public struct MagazineInfo
{
    [SerializeField] private int _magazineSize;
    [SerializeField] private bool fullReload;
    [SerializeField] private float _initialReload;
    [SerializeField] private float _additionalReload;

    public int MagazineSize => _magazineSize;
    public bool FullReload => fullReload;
    public float InitialReload => _initialReload;
    public float AdditionalReload => _additionalReload;
}