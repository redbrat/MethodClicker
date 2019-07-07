using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    public class SomeStateMachineBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        McPtr Yes_Its_Working_Here_Too;

        private void GoToState()
        {
            Debug.Log("Working");
        }
    }
}
