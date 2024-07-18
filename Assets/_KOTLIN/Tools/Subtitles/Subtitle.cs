using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace KOTLIN.Subtitles
{
    public class Subtitle : MonoBehaviour
    {
        public string text;
        public Vector3 position;
        public Color color;
        public AudioSource audioCreator;
        public Transform source;

        public bool is3D;
        public bool dontDestroy;
        public float time;

        [SerializeField] private TextMeshProUGUI textObject;
        [SerializeField] private RectTransform background;
        [SerializeField] private bool useCharColor;
        [SerializeField] private Character charToGet;
        private float asp;

        private void Start()
        {
            textObject.text = text;
            textObject.color = color;

            asp = (float)Screen.width / (float)Screen.height;
            //we just set the scale and position to the 2d pos for minor optimization
            background.localScale = new Vector3(0.6717044f, 0.6717044f, 0.6717044f);
            background.anchoredPosition = new Vector3(0f, -266.66f / asp, 0f); //-150
        }

        //from voias hell school subtitle system 
        private void LateUpdate()
        {
            if (!dontDestroy)
            {
                if (time > 0f)
                {
                    time -= Time.unscaledDeltaTime * SubtitleManager.Instance.subTimeScale;
                }
                else
                {
                    SubtitleManager.Instance.KillSubtitle(this);
                    return;
                }
            }

            if (!is3D)
            {
                return;
            }

            //save these vals for reuse so we reduce overhead
            Vector3 camPos = GameControllerScript.Instance.cameraTransform.position;
            Vector3 proPos = source.position;
            float distance = Vector3.Distance(proPos, camPos);
            float maxDist = audioCreator.maxDistance;
            float minDist = audioCreator.minDistance;
            float spatial = audioCreator.spatialBlend;
            //hide and return early in the case of log rolloff
            if (distance > maxDist && spatial > 0.5f)
            {
                background.localScale = new Vector3(0f, 0f, 1f);
                return;
            }
            //calculate where to put the subtitle by via position
            float circ = Mathf.Atan2(camPos.z + proPos.z, camPos.x + proPos.x) / 57.29578f + GameControllerScript.Instance.cameraTransform.rotation.eulerAngles.y + 180f;
            //offset for panning
            float circOffset = 100f * audioCreator.panStereo;
            //default width
            float quickWidth = 248.88f / asp;
            //calculated with using spread
            float circWidth = Mathf.Lerp(quickWidth, -quickWidth, audioCreator.spread / 360f); //140
                                                                                               //set position using spatial blending, trig, and previously calculated stuff
                                                                                               //also should figure out how to make this asp account for being upside-down
            background.anchoredPosition = new Vector3((Mathf.Cos(circ * 0.017453292f) * circWidth * spatial) + circOffset, Mathf.Lerp(-266.66f / asp, Mathf.Sin(circ * 0.017453292f) * circWidth, spatial), 0f);
            float rolloffScale = 1f;
            switch (audioCreator.rolloffMode)
            {
                case AudioRolloffMode.Custom:
                    //its just an animation curve
                    rolloffScale = audioCreator.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(distance / maxDist);
                    break;
                case AudioRolloffMode.Linear:
                    //linear is linear tho
                    rolloffScale = Mathf.Lerp(1f, 0f, (distance / maxDist) - (minDist / maxDist));
                    break;
                case AudioRolloffMode.Logarithmic:
                    //rollofffactor cant be gotten so we just use 1
                    float rolloffFactor = 1f;
                    rolloffScale = minDist * (1f / (1f + rolloffFactor * (distance - 1f)));
                    break;
            }

            //multiply in volume and clamp scale
            rolloffScale = Mathf.Clamp01(audioCreator.volume * rolloffScale);
            //revenge of the spatial blend
            float calculatedScale = Mathf.Lerp(1f, rolloffScale, spatial);
            //resize to be smaller
            calculatedScale *= 0.6717044f;
            //and now we actually set it
            background.localScale = new Vector3(calculatedScale, calculatedScale, 0.6717044f);
        }
    }
}
