using System;
using UnityEngine;

public class GunVRUIManager : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] private TMPro.TMP_Text _ammoText;

    [SerializeField] private VisualAmmoManager[] _visualAmmoManagers;
    #pragma warning restore 649
    // Start is called before the first frame update
    void Start()
    {
     
        var gun = GetComponent<IGun>();
        Initialize(gun);
        gun.AmmoState.MaxAmmoChanged += SetupVisualAmmos;
        gun.AmmoState.CurrentAmmoChanged += UpdateAmmoText;
        gun.AmmoState.CurrentAmmoChanged += UpdateVisualAmmos;
        
    }

    void Initialize(IGun gun)
    {
        TryUpdateText(_ammoText, gun.AmmoState.CurrentAmmo);
        foreach (var visualAmmo in _visualAmmoManagers)
        {
            visualAmmo.ResizeIconCount(gun.AmmoState.MaxAmmo);
            visualAmmo.SetActive(gun.AmmoState.MaxAmmo);
        }
    }

    void SetupVisualAmmos(object sender, MaxAmmoChangedArgs args)
    {
        foreach (var vAmmo in _visualAmmoManagers)
        {
            vAmmo.ResizeIconCount(args.MaxAmmo);
        }
    }
    
    

    void TryUpdateText<T>(TMPro.TMP_Text ui, T text)
    {
        if (ui == null)
            Debug.LogWarning("UI Missing!");
        else
        {
            ui.text = text.ToString();
        }
    }

    void UpdateAmmoText(object sender, CurrentAmmoChangedArgs args) => TryUpdateText(_ammoText, args.CurrentAmmo);
    void UpdateVisualAmmos(object sender, CurrentAmmoChangedArgs args)
    {
        foreach (var visualAmmo in _visualAmmoManagers)
        {
            visualAmmo.SetActive(args.CurrentAmmo);
        }
    }
}
