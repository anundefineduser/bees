using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(EntranceScript))]
public class EntranceCustomizer : MonoBehaviour
{
    public DecompType type;
    [HideInInspector] public DecompType lastType; 

    private bool changeToPlus;
    private bool change;

    private void OnValidate()
    {
        if (type != lastType)
        {
            Debug.Log("yeahhhhhhhhhhhhhhh change"); 
            lastType = type; 

            switch (type)
            {
                case DecompType.Classic:
                    change = true;
                    changeToPlus = false;
                    break;
                case DecompType.Plus_PreV6:
                    change = true;
                    changeToPlus = true;
                    break;
                case DecompType.Plus_PostV6:
                    change = true;
                    changeToPlus = true;
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }
        }

        GetComponent<EntranceScript>().decompType = type;
    }

    //fix sendmessage error
    private void LateUpdate()
    {
        if (change)
        {
            if (changeToPlus)
            {
                CreatePlusElevator();
                changeToPlus = false; 
            } else
            {
                CreateClassicExit(); 
            }

            change = false; 
        }
    }

    private void CreatePlusElevator()
    {
        GameObject newElevator = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load("DecompEditor/PlusEntrance"));
        newElevator.transform.parent = transform.parent;
        newElevator.transform.position = transform.position;
        newElevator.transform.eulerAngles = transform.eulerAngles;

        newElevator.GetComponent<EntranceScript>().map = GetComponent<EntranceScript>().map;
        newElevator.GetComponent<EntranceCustomizer>().type = DecompType.Plus_PreV6;
        newElevator.GetComponent<EntranceCustomizer>().lastType = DecompType.Plus_PreV6;

        DestroyImmediate(gameObject);
    }

    private void CreateClassicExit()
    {
        GameObject newElevator = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load("DecompEditor/ClassicEntrance"));
        newElevator.transform.parent = transform.parent;
        newElevator.transform.position = transform.position;
        newElevator.transform.eulerAngles = transform.eulerAngles;

        newElevator.GetComponent<EntranceScript>().map = GetComponent<EntranceScript>().map;
        newElevator.GetComponent<EntranceCustomizer>().lastType = DecompType.Classic;
        newElevator.GetComponent<EntranceCustomizer>().type = DecompType.Classic;

        DestroyImmediate(gameObject);
    }
}
