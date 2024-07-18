using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{ 
    public void OnUse()
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GameControllerScript.Instance.alarmClock, GameControllerScript.Instance.playerTransform.position, GameControllerScript.Instance.cameraTransform.rotation);
        gameObject.GetComponent<AlarmClockScript>().baldi = GameControllerScript.Instance.baldiScrpt;
        GameControllerScript.Instance.ResetItem();
    }
}
