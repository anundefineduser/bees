#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KOTLIN.UI.States
{
    public class KOTLIN_UIStates
    {
        private IUIState currentState;
        private string motive; 

        public KOTLIN_UIStates(IUIState initialState)
        {
            currentState = initialState;
            motive = motivationalPhrases[UnityEngine.Random.Range(0, motivationalPhrases.Length)]; 
        }

        public void SetState(IUIState newState)
        {
            currentState = newState;
        }

        public void RenderCurrentState()
        {
            currentState.RenderUI();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"KOTLIN v1.0 - {motive}", EditorStyles.centeredGreyMiniLabel); 
        }

        private string[] motivationalPhrases =
        {
            "ur doing great :D", 
            "that looks awesome!!",
            "wow! you exist!",
            "ur making peak"
        };

    }

    public interface IUIState
    {
        void RenderUI();
    }

}
#endif