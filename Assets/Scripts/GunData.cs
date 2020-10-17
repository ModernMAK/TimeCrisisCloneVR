using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun.asset", menuName = "Custom/Data/Gun")]
public class GunData : ScriptableObject
{
    #pragma warning disable 649
    [SerializeField] private AmmoState ammoState;
    
    [SerializeField] private AimCone _aimCone;

    [SerializeField] private FiringInfo _firingInfo;
    
    
    [Obsolete("Use AmmoData and ReloadingData")]
    [SerializeField] private MagazineInfo _magazine;
    #pragma warning restore 649
    
    public AmmoState AmmoState => ammoState;

    public AimCone AimCone => _aimCone;
    public FiringInfo FiringInfo => _firingInfo;

    
    [Obsolete("Use AmmoData and ReloadingData")]
    public MagazineInfo MagazineInfo => _magazine;

    #pragma warning restore 618
}