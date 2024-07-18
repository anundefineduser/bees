using KOTLIN.Translation;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOTLIN.Subtitles
{
    public enum SubtitleType
    {
        TwoD,
        ThreeD,
        FourD
    }

    public class SubtitleManager : Singleton<SubtitleManager>
    {
        [SerializeField] private Canvas SubtitleUI;
        [SerializeField] private GameObject SubtitlePrefab;

        public bool KillSubtitlesOnSceneUnload = true;
        public float subTimeScale;

        private void Start()
        {
            SceneManager.sceneUnloaded += SceneUnLoad;
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= SceneUnLoad;
        }

        private void SceneUnLoad(Scene currentScene)
        {
            if (KillSubtitlesOnSceneUnload)
            {
                foreach (RectTransform subtitle in SubtitleUI.GetComponentsInChildren<RectTransform>())
                {
                    if (subtitle.name != "SubtitleUI")
                    {
                        Destroy(subtitle.gameObject);
                    }
                }


            }
        }

        public void CreateSubtitle(SubtitleType type, string text, float time, bool forever, Color colour, AudioSource audCreator, Transform creator)
        {
                if (type == SubtitleType.FourD) type = SubtitleType.ThreeD; //funni

                GameObject newSubtitle = Instantiate(SubtitlePrefab);
                newSubtitle.transform.SetParent(SubtitleUI.transform, true); //fuck instantiate method 

                Subtitle sub = newSubtitle.GetComponent<Subtitle>();
                sub.text = text;
                sub.color = colour;
                sub.is3D = type == SubtitleType.ThreeD;
                sub.audioCreator = audCreator;
                sub.source = creator;
                sub.time = time;
                sub.dontDestroy = forever;
        }

        public void CreateSubtitleTranslated(SubtitleType type, string key, float time, bool forever, Color colour, AudioSource audCreator, Transform creator)
        {
                if (type == SubtitleType.FourD) type = SubtitleType.ThreeD; //funni

                GameObject newSubtitle = Instantiate(SubtitlePrefab);
                newSubtitle.transform.SetParent(SubtitleUI.transform, true); //fuck instantiate method 

                Subtitle sub = newSubtitle.GetComponent<Subtitle>();
                sub.text = TranslationManager.Instance.GetTranslationString(key);
                sub.color = colour;
                sub.is3D = type == SubtitleType.ThreeD;
                sub.audioCreator = audCreator;
                sub.source = creator;
                sub.time = time;
                sub.dontDestroy = forever;
        }

        public void KillSubtitle(Subtitle title)
        {
            Destroy(title.gameObject);
        }
    }

}
