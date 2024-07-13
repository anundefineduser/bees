using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOTLIN.Translation
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LanguageSelector : MonoBehaviour
    {
        private Dictionary<string, string> FullToSmallName = new Dictionary<string, string>()
        {
            {"English", "EN" },
        };

        public void OnSelectNewLanguage(int id)
        {
            TranslationManager.Instance.SetLanguage(FullToSmallName[GetComponent<TMP_Dropdown>().options[id].text]);
        }
    }
}