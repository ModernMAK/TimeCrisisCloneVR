using System;
using UnityEngine;

[Serializable]
public struct MagazineInfo
{
    #pragma warning disable 649
    [Obsolete]
    [SerializeField] private int _magazineSize;
    [SerializeField] private bool fullReload;
    [SerializeField] private float _initialReload;
    [SerializeField] private float _additionalReload;
    #pragma warning restore 649
    
    [Obsolete]
    public int MagazineSize => _magazineSize;
    public bool FullReload => fullReload;
    public float InitialReload => _initialReload;
    public float AdditionalReload => _additionalReload;
}