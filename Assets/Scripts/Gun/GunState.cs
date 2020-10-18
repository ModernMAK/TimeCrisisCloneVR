using System;

[Serializable]
public abstract class GunState
{
    public GunState(IGun gun)
    {
        AttachedGun = gun;
        if (gun == null)
            throw new NullReferenceException("Gun State cannot have a null gun.");
    }

    /// <summary>
    /// A Helper to get the Gun this data is attached to.
    /// A null value indicates that the reference has not been set or the data is not representing the state of a gun
    /// </summary>
    public IGun AttachedGun { get; private set; }
}