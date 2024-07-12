using UnityEngine;

namespace KOTLIN
{
    public class Interactable : MonoBehaviour
    {
        public float InteractDistance = 10f; 

        public virtual void Interact() { }
    }
}
