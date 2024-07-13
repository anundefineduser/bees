using KOTLIN.Translation;
using TMPro;
using UnityEngine;

public class DetentionTextScript : MonoBehaviour
{
    private void Start()
    {
        this.text = base.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (this.door.lockTime > 0f)
        {
            this.text.text = TranslationManager.Instance.GetTranslationString("World_Detention_YouHave") + Mathf.CeilToInt(this.door.lockTime) + TranslationManager.Instance.GetTranslationString("World_Detention_SecondsRemain");
        }
        else
        {
            this.text.text = string.Empty;
        }
    }

    public DoorScript door;

    private TMP_Text text;
}