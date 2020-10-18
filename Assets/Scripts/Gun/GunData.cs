using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun.asset", menuName = "Custom/Data/Gun")]
public class GunData : ScriptableObject
{
#pragma warning disable 649
    [SerializeField] private AmmoState.Data ammoData;
    [SerializeField] private ReloadingState.Data reloadingData;
    [SerializeField] private AimCone _aimCone;
    [SerializeField] private FiringInfo _firingInfo;
#pragma warning restore 649
    public AmmoState.Data AmmoData => ammoData;
    public ReloadingState.Data ReloadingData => reloadingData;
    public AimCone AimCone => _aimCone;
    public FiringInfo FiringInfo => _firingInfo;
}