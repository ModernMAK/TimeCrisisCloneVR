using UnityEngine;

[CreateAssetMenu(fileName = "Gun.asset", menuName = "Custom/Data/Gun")]
public class GunData : ScriptableObject
{
    [SerializeField] private AimCone _aimCone;
    [SerializeField] private MagazineInfo _magazine;
    [SerializeField] private FiringInfo _firingInfo;
    public AimCone AimCone => _aimCone;
    public MagazineInfo MagazineInfo => _magazine;
    public FiringInfo FiringInfo => _firingInfo;

}