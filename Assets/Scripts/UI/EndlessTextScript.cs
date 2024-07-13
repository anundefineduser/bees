using KOTLIN.Translation;
using TMPro;
using UnityEngine;

public class EndlessTextScript : MonoBehaviour
{
    private void Start()
    {

        this.text.text = string.Concat(new object[]
        {
            TranslationManager.Instance.GetTranslationString("MENU_Play_Endless"),
            "\n",
            TranslationManager.Instance.GetTranslationString("MENU_Play_HighScore"),
            PlayerPrefs.GetInt("HighBooks"),
            " ",
            TranslationManager.Instance.GetTranslationString("Notebooks")
        });
    }

    public TMP_Text text;
}