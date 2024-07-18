using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots : MonoBehaviour
{
    public void OnUse()
    {
        GameControllerScript.Instance.player.ActivateBoots();
        base.StartCoroutine(GameControllerScript.Instance.BootAnimation());
        GameControllerScript.Instance.ResetItem();
    }
}
