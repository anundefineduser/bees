using UnityEngine;

namespace KOTLIN.Interactions
{
    public class Interactable : MonoBehaviour
    {
        public float InteractDistance = 10f; 

        public virtual void Interact() { }
    }
}
