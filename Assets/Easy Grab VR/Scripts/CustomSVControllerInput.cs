using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSVControllerInput : SVControllerInput
{
     public override bool GripAutoHolds => GlobalSettings.Vr.IsGripMode(GripSettingMode.Toggle);

     public override bool SurpressForcedErrors => true;

     public override SVInputButton GripButton => SVInputButton.SVButton_Grip;
}
