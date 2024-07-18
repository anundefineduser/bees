public class PickupScript : KOTLIN.Interactions.Interactable
{
    public override void Interact()
    {
        gameObject.SetActive(false);
        GameControllerScript.Instance.CollectItem(ItemID);
    }

    [UnityEngine.SerializeField] private int ItemID; 
}