using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterSVControllerInput : SVControllerInput
{
     public override bool GripAutoHolds => GlobalSettings.Vr.IsGripMode(GripSettingMode.Toggle);
}
