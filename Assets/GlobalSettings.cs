
public static partial class GlobalSettings
{
    public static class Vr
    {
        public static GripSettingMode GripMode;
        public static bool IsGripMode(GripSettingMode mode) => GripMode == mode;
    }
    public static class CheatCodes
    {
        public static bool InfiniteAmmo;
    }
}