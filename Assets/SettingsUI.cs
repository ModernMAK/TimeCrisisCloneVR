using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GripSettingMode
{
    Toggle,
    Hold
}

public class SettingsUI : MonoBehaviour
{
#pragma warning disable 649
    [Header("Grip Mode")] 
    [SerializeField]
    private TMP_Text _holdText;
    [SerializeField]
    private TMP_Text _toggleText;
    [SerializeField]
    private Button _gripdModeButton;

    [Header("Infinite Ammo")] 
    [SerializeField]
    private TMP_Text _onText;
    [SerializeField]
    private TMP_Text _offText;
    [SerializeField]
    private Button _infiniteAmmoButton;
#pragma warning restore 649

    private void Awake()
    {
        Setup();
        Initialize();
    }

    private void Setup()
    {
        _gripdModeButton.onClick.AddListener(OnGripModeClicked);
        _infiniteAmmoButton.onClick.AddListener(OnInfiniteAmmoClicked);
    }
    
    private void Initialize()
    {
        FixGripModeButton();
        FixInfiniteAmmoButton();
    }
    private void FixGripModeButton()
    {
        _holdText.gameObject.SetActive(GlobalSettings.Vr.IsGripMode(GripSettingMode.Hold));
        _toggleText.gameObject.SetActive(GlobalSettings.Vr.IsGripMode(GripSettingMode.Toggle));
    }
    private void FixInfiniteAmmoButton()
    {
        _onText.gameObject.SetActive(GlobalSettings.CheatCodes.InfiniteAmmo);
        _offText.gameObject.SetActive(!GlobalSettings.CheatCodes.InfiniteAmmo);
    }
    private void OnGripModeClicked()
    {
        //Simple 'toggle' hack
        if (GlobalSettings.Vr.GripMode == GripSettingMode.Hold)
            GlobalSettings.Vr.GripMode = GripSettingMode.Toggle;
        else
            GlobalSettings.Vr.GripMode = GripSettingMode.Hold;
        FixGripModeButton();
    }

    private void OnInfiniteAmmoClicked()
    {
        GlobalSettings.CheatCodes.InfiniteAmmo = !GlobalSettings.CheatCodes.InfiniteAmmo;
        FixInfiniteAmmoButton();
    }
}
