using System;

public class CurrentAmmoChangedArgs : EventArgs
{
    public int CurrentAmmo;
}
public class MaxAmmoChangedArgs : EventArgs
{
    public int MaxAmmo;
}