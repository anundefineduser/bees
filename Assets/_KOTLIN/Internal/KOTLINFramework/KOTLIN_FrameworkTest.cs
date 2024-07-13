#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using KOTLIN.UI;
using KOTLIN.Internal;
using KOTLIN.UI.States;

public class KOTLIN_FrameworkTest : KOTLINEditorSingleton<KOTLIN_FrameworkTest>
{
    public KOTLIN_UIStates stateManager;

    [MenuItem("KOTLIN/FrameworkTest")]
    public static void ShowWindow()
    {
        KOTLIN_UIBASE.createWindow<KOTLIN_FrameworkTest>("framework test hi");
    }

    private void OnEnable()
    {
        stateManager = new KOTLIN_UIStates(new TestState2());
    }

    private void OnGUI()
    {
        stateManager.RenderCurrentState();
    }


    public class TestState : IUIState
    {
        public void RenderUI()
        {
            EditorGUILayout.LabelField("hi person in my phone!!!1", EditorStyles.boldLabel);
            KOTLIN_UIBASE.beginVertical(true); 
            if (GUILayout.Button("go to test 2"))
            {
                Instance.stateManager.SetState(new TestState2());
            }
            KOTLIN_UIBASE.endVertical(); 
        }
    }

    public class TestState2 : IUIState
    {
        public void RenderUI()
        {
            EditorGUILayout.LabelField("johnny this is an intervention, youve got to stop twsting peoples heads! johnny!");
            if (GUILayout.Button("go to test 1"))
            {
                Instance.stateManager.SetState(new TestState());
            }
        }
    }
}
#endif 