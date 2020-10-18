using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GripSettingMode
{
    Toggle,
    Hold
}

public static class GlobalSettings
{
    public static class Vr
    {
        public static GripSettingMode GripMode;
        public static bool IsGripMode(GripSettingMode mode) => GripMode == mode;
    }
    
}
public class SettingsUI : MonoBehaviour
{
    [Header("Grip Mode")] 
    [SerializeField]
    private TMP_Text _holdText;
    [SerializeField]
    private TMP_Text _toggleText;
    [SerializeField]
    private Button _gripdModeButton;


    private void Awake()
    {
        _gripdModeButton.onClick.AddListener(OnGripModeClicked);
        
    }

    private void Initialize()
    {
        FixGripModeButtons();
    }
    private void FixGripModeButtons()
    {
        _holdText.gameObject.SetActive(GlobalSettings.Vr.IsGripMode(GripSettingMode.Hold));
        _toggleText.gameObject.SetActive(GlobalSettings.Vr.IsGripMode(GripSettingMode.Toggle));
    }
    private void OnGripModeClicked()
    {
        //Simple 'toggle' hack
        if (GlobalSettings.Vr.GripMode == GripSettingMode.Hold)
            GlobalSettings.Vr.GripMode = GripSettingMode.Toggle;
        else
            GlobalSettings.Vr.GripMode = GripSettingMode.Hold;
        FixGripModeButtons();
    }
}
