using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    public class SomeStateMachineBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        MethodClickerPointer Yes_Its_Working_Here_Too;

        private void GoToState() => Debug.Log("Working");
    }
}
