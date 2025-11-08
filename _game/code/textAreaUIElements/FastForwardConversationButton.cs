using UnityEngine;

namespace PixelCrushers.DialogueSystem.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/UI/Standard UI/UI Effects/Standard UI Continue Button Fast Forward")]
    [RequireComponent(typeof(AdvanceButton))]
    public class StandardUIContinueButtonFastForward : PixelCrushers.DialogueSystem.StandardUIContinueButtonFastForward
    {
        public override void OnFastForward()
        {
            Invoke("MyFastForward", GetComponent<AdvanceButton>().fillDuration);
        }

        void MyFastForward()
        {
            base.OnFastForward();
        }



    }

}