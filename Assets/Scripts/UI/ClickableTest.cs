using UnityEngine;

public class ClickableTest : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //Left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.name == "MathNotebook") // If you are looking at a notebook
            {
                base.gameObject.SetActive(false); //Disable the notebook
            }
        }
    }
}