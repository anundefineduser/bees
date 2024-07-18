using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZestyBar : MonoBehaviour
{
    public void OnUse()
    {
        GameControllerScript.Instance.player.stamina = GameControllerScript.Instance.player.maxStamina * 2f;
        GameControllerScript.Instance.ResetItem();
    }
}
