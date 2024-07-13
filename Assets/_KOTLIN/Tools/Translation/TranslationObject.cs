using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TranslationType
{
    TMPText_UI,
    TMPText,
    Text
}

namespace KOTLIN.Translation
{

    public class TranslationObject : MonoBehaviour
    {
        [SerializeField] private string TranslationKey;
        [SerializeField] private TranslationType type;

        private string TranslatedText;

        private void Start()
        {
            Translate();
        }

        public void Translate()
        {
            TranslatedText = TranslationManager.Instance.GetTranslationString(TranslationKey);

            if (type == TranslationType.Text)
            {
                GetComponent<Text>().text = TranslatedText;
            }

            if (type == TranslationType.TMPText)
            {
                GetComponent<TextMeshPro>().text = TranslatedText;
            }

            if (type == TranslationType.TMPText_UI)
            {
                GetComponent<TextMeshProUGUI>().text = TranslatedText;
            }
        }
    }
}