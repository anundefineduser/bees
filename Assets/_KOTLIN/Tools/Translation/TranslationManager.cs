using Newtonsoft.Json;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace KOTLIN.Translation
{

    public class TranslationManager : Singleton<TranslationManager>
    {
        private Dictionary<string, string> currentTranslation;
        private bool translationError;

        private void Awake()
        {
            FindTranslationJSON("EN");
        }

        private void FindTranslationJSON(string lang)
        {
            string JSONPath = $"{Application.streamingAssetsPath}/Subtitles_{lang}.json";
            string contents = string.Empty;

            if (File.Exists(JSONPath))
            {
                contents = File.ReadAllText(JSONPath);
                currentTranslation = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
            }
            else
            {
                Debug.LogWarning($"Couldn't find subtitle file for {lang} at: {JSONPath}, attempt EN fallback");
                JSONPath = $"{Application.streamingAssetsPath}/Subtitles_EN.json";

                if (File.Exists(JSONPath))
                {
                    contents = File.ReadAllText(JSONPath);
                    currentTranslation = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
                }
                else
                {
                    Debug.LogError($"Couldn't find subtitle file for EN fallback at: {JSONPath}, youre fucked buddy");
                    translationError = true;
                }
            }
        }

        public string GetTranslationString(string key)
        {
            if (translationError == true)
            {
                return "KOTLIN_ERR_NO_TRANSLATION";
            }

            currentTranslation.TryGetValue(key, out string val);
            if (val == null)
            {
                return "KOTLIN_ERR_KEY_NOT_FOUND";
            }

            return currentTranslation[key];
        }

        public void SetLanguage(string abreivation)
        {
            FindTranslationJSON(abreivation);
            foreach (TranslationObject obj in FindObjectsOfType<TranslationObject>(true)) { obj.Translate(); }
        }
    }
}