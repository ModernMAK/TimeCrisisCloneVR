using System;
using UnityEngine;

public class GunVRUIManager : MonoBehaviour
{
    [SerializeField]
    private GunMagazine _magazine;
    #pragma warning disable 649
    [SerializeField] private TMPro.TMP_Text _ammoText;

    [SerializeField] private VisualAmmoManager[] _visualAmmoManagers;
#pragma warning restore 649
	private void Awake()
	{
        if (_magazine == null)
            _magazine = GetComponent<GunMagazine>();
        if (_magazine == null)
            enabled = false;
    }
	void Start()
    {
        Initialize(_magazine);
        _magazine.MaxAmmoChanged += SetupVisualAmmos;
        _magazine.CurrentAmmoChanged += UpdateAmmoText;
        _magazine.CurrentAmmoChanged += UpdateVisualAmmos;
        
    }

    void Initialize(GunMagazine magazine)
    {
        TryUpdateText(_ammoText, magazine.CurrentAmmo);
        foreach (var visualAmmo in _visualAmmoManagers)
        {
            visualAmmo.ResizeIconCount(magazine.MaxAmmo);
            visualAmmo.SetActive(magazine.MaxAmmo);
        }
    }

    void SetupVisualAmmos(object sender, ChangedEventArgs<int> args)
    {
        foreach (var vAmmo in _visualAmmoManagers)
        {
            vAmmo.ResizeIconCount(args.After);
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

    void UpdateAmmoText(object sender, ChangedEventArgs<int> args) => TryUpdateText(_ammoText, args.After);
    void UpdateVisualAmmos(object sender, ChangedEventArgs<int> args)
    {
        foreach (var visualAmmo in _visualAmmoManagers)
        {
            visualAmmo.SetActive(args.After);
        }
    }
}
