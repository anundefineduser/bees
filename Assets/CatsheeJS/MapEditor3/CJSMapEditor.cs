#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.AI;

//DO NOT LOOK IN THIS CODE, IT IS PURE HELL!!!!!! or fix it for me mwa
public class CJSMapEditor : EditorWindow
{
    //
    public static CJSMapEditor Instance; 

    private enum EditorWindowState
    {
        Home,
        Create,
        Edit,
        Texigen, //i love baldi modding, and to anyone who is not johnster, if youre reading this. i love you too :)
        Tutorial
    }

    private enum EditMode
    {
        Create,
        Edit,
        Delete,
    }

    private enum TileObject
    {
        Wall, 
        Floor, 
        Ceiling,
        Tile, 
        Null
    }

    private enum Decomp
    {
        PorkyPowers,
        BASIC,
        Simplified
    }

    private EditorWindowState state;
    private EditMode editState;
    private TileObject currSelectionType;
    private Decomp currDecomp; 

    private Vector3 createPos;
    private Vector2 textureScrollPosition;
    private Vector2 editWindowScrollPosition; 

    private bool isEditing;
    private bool connectTiles = true; 
    private bool isMakingRoom; 
    ///
    [SerializeField] private UnityEngine.Object TilePrefab;
    [SerializeField] private UnityEngine.Object DoorPrefab; 
    [SerializeField] private UnityEngine.Object EditorTools;

    [Space()]
    [SerializeField] private Tag ToolsTag;
    [SerializeField] private Tag TilesTag;
    [SerializeField] private Tag AITag; 
    [Space()]
    [SerializeField] private MapTextureProfile DefaultTextureProfile; 

    //Create and init the map editor 
    [MenuItem("CJS Tools/Ultimate Map Editor")]
    private static void CreateWindow()
    {
        CJSMapEditor window = GetWindow<CJSMapEditor>(false, "Ultimate Map Editor");
        window.minSize = new Vector2(300, 400);

        window.Init(); 
    }

    private void Init()
    {
        isEditing = false;
        CreateEditTools();
    }

    private void OnDestroy()
    {
        isEditing = false;
        CreateEditTools();
    }

    private void OnSelectionChange()
    {
        if (!isEditing) return;
        if (Selection.activeGameObject == null) return; 

        Transform activeTrans = Selection.activeGameObject.transform;
        if (activeTrans.parent == null) return;

        Transform activeParentTrans = activeTrans.parent;

        if (activeParentTrans.TryGetComponent(out Tags tagComponent))
        {
            if (tagComponent.HasTag(ToolsTag))
            {
                ToolsSelection(activeTrans);
                return; 
            }
        }

        if (GetTilesParent(activeTrans.gameObject))
        {
            if (editState == EditMode.Delete)
            {
                Debug.Log("ahhhhhhhhhhhhhhhhhh");
                Transform tileCurrent = activeTrans;

                DisconnectWalls(tileCurrent);
                DestroyImmediate(tileCurrent.gameObject);
                return; 
            }

            GetEditorTools().position = GetTilesParent(activeTrans.gameObject).transform.position;
            GetEditorTools().gameObject.SetActive(true);
        }
    }

    private GameObject GetTilesParent(GameObject obj)
    {

        if (obj != null)
        {

            Transform currentTransform = obj.transform;


            while (currentTransform != null)
            {
                Tags tags = currentTransform.GetComponent<Tags>();
                if (tags != null && tags.GetTags()[0] == TilesTag)
                {
                    return currentTransform.gameObject;
                }
                currentTransform = currentTransform.parent;
            }
        }

        return null;
    }

    private Transform GetMapParent()
    {
        return GameObject.Find("Map").transform ?? new GameObject("Map").transform;
    }

    private Transform GetSelectedTile()
    {
        Transform tools = GetEditorTools();
        Transform getter = tools.Find("TileGetter"); 
        Collider[] hitColliders = Physics.OverlapSphere(getter.position, .1f);

        if (hitColliders.Length != 0)
        {
            int index = 0; 
            if (hitColliders[index].gameObject.name == "TileGetter") //why use foreach when you can do this??? xd 
            {
                index = 1; 
            }

            if (hitColliders[index].gameObject.name == "Floor")
            {
                return GetTilesParent(hitColliders[index].gameObject).transform;
            } else
            {
                Debug.Log("not floor :(, is " + hitColliders[index].gameObject.name);
            }
        } else
        {
            Debug.Log("no length :("); 
        }

        return null; 
    }

    //FROM 2.0 (which is why it sucks xd)//
    public void ConnectWalls(Transform tile)
    {
        List<Transform> Walls = new List<Transform>();

        foreach (Transform child in tile.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("Wall") && child.name != "Walsl")
            {
                Walls.Add(child);
            }
        }

        /*        foreach (Transform child in tile.GetComponentsInChildren<Transform>(true))
                {
                    foreach (Transform wall in Walls)
                    {
                        string WallDir = wall.name.Replace("Wall_", string.Empty);
                        string PathDir = child.name;

                        Debug.Log(WallDir);
                        Debug.Log(PathDir);
                        if (WallDir != PathDir) return;

                        child.gameObject.SetActive(!wall.gameObject.activeSelf);
                    }
                }*/

        Transform otherTile = null; 
        foreach (Transform wall in Walls)
        {
            Collider[] hitColliders = Physics.OverlapSphere(wall.position, .1f);
            foreach (Collider collider in hitColliders)
            {
                Debug.Log($"collider: {collider.name}, wall: {wall.name}"); 

                if (collider.gameObject.name.Contains("Wall") && collider.transform != wall && !Walls.Contains(collider.transform) && GetTilesParent(collider.gameObject) != null)
                {
                    TileType typ = tile.GetComponent<TileDescriptor>().type; 
                    if (GetTilesParent(collider.gameObject).GetComponent<TileDescriptor>().type != typ)
                    {
                        continue; 
                    }

                    //im insanely sick and just want this to be done 
                    if (wall.gameObject.GetComponent<WallLink>())
                    {
                        DestroyImmediate(wall.gameObject.GetComponent<WallLink>()); 
                    }
                    if (collider.gameObject.GetComponent<WallLink>())
                    {
                        DestroyImmediate(collider.gameObject.GetComponent<WallLink>());
                    }

                    wall.gameObject.AddComponent<WallLink>().Link = collider.transform;
                    collider.gameObject.AddComponent<WallLink>().Link = wall.transform;
                    collider.gameObject.SetActive(false);
                    wall.gameObject.SetActive(false);

                    otherTile = collider.transform;
                    otherTile = GetTilesParent(otherTile.gameObject).transform;
                    CalculteAIs(otherTile);
                }
            }
        }


        CalculteAIs(tile);
    }

    private void CalculteAIs(Transform tile)
    {
        //I WAS FORCED TO DO THIS
        tile.Find("North").gameObject.SetActive(!tile.Find("Wall_North").gameObject.activeSelf);
        tile.Find("East").gameObject.SetActive(!tile.Find("Wall_East").gameObject.activeSelf);
        tile.Find("South").gameObject.SetActive(!tile.Find("Wall_South").gameObject.activeSelf);
        tile.Find("West").gameObject.SetActive(!tile.Find("Wall_West").gameObject.activeSelf);

        tile.Find("North").GetComponent<AiTagDescriptor>().enabledPath = !tile.Find("Wall_North").gameObject.activeSelf;
        tile.Find("East").GetComponent<AiTagDescriptor>().enabledPath = !tile.Find("Wall_East").gameObject.activeSelf;
        tile.Find("South").GetComponent<AiTagDescriptor>().enabledPath = !tile.Find("Wall_South").gameObject.activeSelf;
        tile.Find("West").GetComponent<AiTagDescriptor>().enabledPath = !tile.Find("Wall_West").gameObject.activeSelf;
        bool doBigOpen = false; 
        if (isMakingRoom && tile.Find("North").gameObject.activeSelf && tile.Find("East").gameObject.activeSelf && tile.Find("South").gameObject.activeSelf && tile.Find("West").gameObject.activeSelf)
        {
            doBigOpen = true; 
        }

        //fuck you hunbnun
        tile.Find("FullTile").GetComponent<AiTagDescriptor>().enabledPath = doBigOpen; 
        tile.Find("FullTile").gameObject.SetActive(doBigOpen);
    }

    public void DisconnectWalls(Transform tile)
    {
        List<Transform> Walls = new List<Transform>();

        foreach (Transform child in tile.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Contains("Wall"))
            {
                Walls.Add(child);
            }
        }

        Transform otherTile = null;
        foreach (Transform wall in Walls)
        {
            if (wall.TryGetComponent(out WallLink linker))
            {
                if (linker.Link != null)
                {
                    linker.Link.gameObject.SetActive(true);
                    wall.gameObject.SetActive(true);

                    otherTile = linker.Link.transform;
                    otherTile = GetTilesParent(otherTile.gameObject).transform;
                    CalculteAIs(otherTile);
                }
            }
        }

        CalculteAIs(tile);
    }
    //

    int attemptCount = 0; 
    private Transform GetEditorTools()
    {
        if (attemptCount == 2)
        {
            attemptCount = 0;
            return null; 
        }

        if (!GameObject.Find("MapEditor"))
        {
            CreateEditTools();
            attemptCount++; 
            return GetEditorTools(); 
        }

        if (!GameObject.Find("MapEditor/EditorTools(Clone)"))
        {
            CreateEditTools();
            attemptCount++;
            return GetEditorTools();
        }

        return GameObject.Find("MapEditor/EditorTools(Clone)").transform; 
    }

    private GameObject CreateTile(Vector3 pos, Transform parent, bool overrideConnection = false)
    {
        GameObject newTile = (GameObject)PrefabUtility.InstantiatePrefab(TilePrefab);
        newTile.transform.parent = parent;
        newTile.transform.position = pos;
        newTile.name = $"Tile_{newTile.transform.localPosition.x}_{newTile.transform.localPosition.y}_{newTile.transform.localPosition.z}";

        Material wallMat = DefaultTextureProfile.DefaultWallTex.mat;
        Material floorMat = DefaultTextureProfile.DefaultFloorTex.mat;
        Material ceilingMat = DefaultTextureProfile.DefaultCeilingTex.mat;

        TileDescriptor descriptor = newTile.AddComponent<TileDescriptor>();
        descriptor.type = TileType.Hall; 

        if (isMakingRoom)
        {
            descriptor.type = TileType.Room; 
            wallMat = DefaultTextureProfile.RoomWallTex.mat; 
            floorMat = DefaultTextureProfile.RoomFloorTex.mat;
            ceilingMat = DefaultTextureProfile.RoomCeilingTex.mat;
        }

        foreach (Transform child in newTile.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Contains("Wall"))
                child.GetComponent<Renderer>().material = wallMat;

            if (child.name.Contains("Floor"))
                child.GetComponent<Renderer>().material = floorMat;

            if (child.name.Contains("Ceiling"))
                child.GetComponent<Renderer>().material = ceilingMat; 
        }

        if (connectTiles && !overrideConnection)
            ConnectWalls(newTile.transform);

        if (overrideConnection && !connectTiles)
            ConnectWalls(newTile.transform); 

        return newTile; 
    }

    private void CreateEditTools()
    {
        GameObject tools = (GameObject)Instantiate(EditorTools);
        GameObject parent = GameObject.Find("MapEditor") ?? new GameObject("MapEditor");

        if (parent.transform.Find("EditorTools(Clone)") && !isEditing)
        {
            DestroyImmediate(parent.transform.Find("EditorTools(Clone)").gameObject); 
        }

        if (!isEditing)
        {
            DestroyImmediate(tools); 
            DestroyImmediate(parent);
            return; 
        } 

        tools.transform.parent = parent.transform; 
    }

    private void CreateStartingTile()
    {
        GameObject tileParent = new GameObject("Map");
        CreateTile(createPos, tileParent.transform, true);
    }

    private void ToolsSelection(Transform obje)
    {
        if (editState == EditMode.Create)
        {
            Vector3 NewTilePos = obje.position;
            NewTilePos.y = 0; //the editor tools is lowered by like .3 t ohide it under the tiles ebcause im too lazy to set activwe (THIS BREAKS Y LEVEL SO BAD FIX)
            GameObject tiel = CreateTile(NewTilePos, GetMapParent());
            GetEditorTools().position = tiel.transform.position;
        }

        //bruteforce asf solution to deselection but ok 
        string originName = Selection.activeGameObject.name; 
        GameObject newObj = Instantiate(Selection.activeGameObject, GetEditorTools());
        newObj.name = originName; 
        DestroyImmediate(Selection.activeGameObject);
    }

    /// <summary>
    /// UI SECTION BEGIN
    /// </summary>
    [System.Obsolete]
    private void OnGUI()
    {
        //i want my basic framework :( 
        switch (state)
        {
            case EditorWindowState.Home:
                RenderHome();
                return;
            case EditorWindowState.Create:
                RenderCreate();
                break;
            case EditorWindowState.Edit:
                RenderEditor(); 
                break;
            case EditorWindowState.Texigen:
                RenderTextures();
                break;
            case EditorWindowState.Tutorial:
                RenderTutorial();
                break;
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Back to Menu"))
        {
            isEditing = false;
            CreateEditTools(); 
            state = EditorWindowState.Home; 
        }
    }

    private void RenderHome()
    {
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Create"))
        {
            state = EditorWindowState.Create; 
        }

        if (GUILayout.Button("Edit"))
        {
            state = EditorWindowState.Edit;
            isEditing = false;
            CreateEditTools();
        }

        EditorGUILayout.Space(); 

        if (GUILayout.Button("Edit Textures"))
        {
            state = EditorWindowState.Texigen; 
        }

        EditorGUILayout.EndVertical(); 

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("What decompile are you using? (if yours is not listed, select PorkyPowers)", MessageType.Info); 
        currDecomp = (Decomp)EditorGUILayout.EnumPopup(currDecomp); 
    }

    private void RenderCreate()
    {
        EditorGUILayout.BeginVertical("box");
        createPos = EditorGUILayout.Vector3Field("Create Position", createPos);

        EditorGUILayout.Space(); 

        if (GUILayout.Button("Create"))
        {
            CreateStartingTile(); 
            state = EditorWindowState.Edit; 
        }

        EditorGUILayout.EndVertical(); 
    }

    [System.Obsolete]
    private void RenderEditor()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Editor Settings", EditorStyles.boldLabel);

        if (GUILayout.Button($"Edit: {(isEditing ? "Active" : "Inactive")}"))
        {
            isEditing = !isEditing;
            CreateEditTools();
        }

        if (!isEditing)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return; 
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Edit Mode", EditorStyles.boldLabel);

        editState = (EditMode)EditorGUILayout.EnumPopup("", editState);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        switch (editState)
        {
            case EditMode.Create:
                RenderEditor_Create();
                break;

            case EditMode.Edit:
                RenderEditor_Edit();
                break;

            case EditMode.Delete:
                RenderEditor_Delete();
                break;
        }
    }

    private void RenderEditor_Create()
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.LabelField("Tile Connection", EditorStyles.boldLabel);
            connectTiles = EditorGUILayout.Toggle("Connect Tiles", connectTiles);

            EditorGUILayout.LabelField("Room Creation", EditorStyles.boldLabel);
            isMakingRoom = EditorGUILayout.Toggle("Create Room", isMakingRoom);
        }
        EditorGUILayout.EndVertical();
    }

    [System.Obsolete]
    private void RenderEditor_Edit()
    {

        EditorGUILayout.HelpBox("Select a tile by clicking to edit\nSelect a wall, ceiling or floor by double clicking to edit", MessageType.Info);
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Toggle AIPaths"))
        {
            bool setState = false;
            bool setTo = false; 

            foreach (Tags tagComponent in FindObjectsOfTypeAll(typeof(Tags))) //support 2019
            {
                if (tagComponent.HasOnlyTag(AITag) && tagComponent.GetComponent<AiTagDescriptor>().enabledPath)
                {
                    if (setState == false)
                    {
                        setTo = !tagComponent.gameObject.activeSelf;
                        setState = true; 
                    }

                    tagComponent.gameObject.SetActive(setTo);
                }
            }
        }
        EditorGUILayout.EndVertical();

        if (!GetTilesParent(Selection.activeGameObject)) return;

        editWindowScrollPosition = EditorGUILayout.BeginScrollView(editWindowScrollPosition);
        currSelectionType = TileObject.Null; 
        Tags tags = Selection.activeGameObject.GetComponent<Tags>();
        if (tags != null && tags.GetTags()[0] == TilesTag)
        {
            currSelectionType = TileObject.Tile; 
        }

        if (Selection.activeGameObject.name.Contains("Wall"))
        {
            currSelectionType = TileObject.Wall;
        }
        if (Selection.activeGameObject.name == "Floor")
        {
            currSelectionType = TileObject.Floor;
        }
        if (Selection.activeGameObject.name == "Ceiling")
        {
            currSelectionType = TileObject.Ceiling;
        }

        if (currSelectionType == TileObject.Null)
        {
            EditorGUILayout.EndScrollView();
            return; 
        }

        //tile editing 
        EditorGUILayout.Space();
        if (currSelectionType == TileObject.Tile)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Connection Controls", EditorStyles.boldLabel);
            if (GUILayout.Button("Disconnect All Walls"))
            {
                DisconnectWalls(Selection.activeTransform); 
            }

            if (GUILayout.Button("Connect Walls"))
            {
                ConnectWalls(Selection.activeTransform);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Manual Connection Edits", EditorStyles.boldLabel);
            string[] wallNames = { "Wall_North", "Wall_East", "Wall_South", "Wall_West" };

            foreach (string wallName in wallNames)
            {
                if (GUILayout.Button("Toggle " + wallName))
                {
                    Transform wall = Selection.activeTransform.Find(wallName);

                    if (wall != null)
                    {
                        WallLink wallLinkComponent = wall.GetComponent<WallLink>();
                        if (wallLinkComponent != null && wallLinkComponent.Link != null)
                        {
                            wallLinkComponent.Link.gameObject.SetActive(!wallLinkComponent.Link.gameObject.activeSelf);
                            CalculteAIs(GetTilesParent(wallLinkComponent.Link.gameObject).transform);
                        }
                        wall.gameObject.SetActive(!wall.gameObject.activeSelf);
                        CalculteAIs(Selection.activeTransform); 
                    }
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            return; 
        }

        if (currSelectionType == TileObject.Wall)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Door Controls", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Door")) //im gonna do the alt ending of bo burnhams inside
            {
                if (currDecomp == Decomp.Simplified) //im too fucking lazy 
                {
                    if (Selection.activeTransform.Find("CJSEditor_Door(Clone)"))
                    {
                        DestroyImmediate(Selection.activeTransform.Find("CJSEditor_Door(Clone)").gameObject);
                    }

                    //Selection.activeTransform.localScale = new Vector3(0.5f, 1, 1);
                    Selection.activeTransform.GetComponent<Collider>().enabled = false;
                    Selection.activeTransform.GetComponent<Renderer>().enabled = false;

                    Vector3 localPos = Vector3.zero;
                    Vector3 doorPos = Vector3.zero;

                    switch (Selection.activeTransform.name)
                    {
                        case "Wall_North":
                            localPos = new Vector3(-5, 5, 2.5f);
                            doorPos = new Vector3(-2.5f, 5, 5);
                            break;
                        case "Wall_West":
                            localPos = new Vector3(-2.5f, 5, -5);
                            doorPos = new Vector3(-2.5f, 5, 5);
                            break;
                        case "Wall_South":
                            localPos = new Vector3(5, 5, -2.5f);
                            doorPos = new Vector3(-2.5f, 5, 5);
                            break;
                        case "Wall_East":
                            localPos = new Vector3(2.5f, 5, 5);
                            doorPos = new Vector3(-2.5f, 5, 5);
                            break;
                    }

                    Selection.activeTransform.localPosition = localPos;
                    if (isMakingRoom) //fuck me 
                    {
                        Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.RoomThinWallTex.mat;
                    }
                    else
                    {
                        Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.DefaultThinWallTex.mat;
                    }

                    GameObject door = (GameObject)Instantiate(DoorPrefab, Selection.activeTransform, false);

                    //the door instantiates weird
                    //door.transform.localPosition = Selection.activeTransform.localPosition;
                    door.transform.localPosition = doorPos;
                    //door.transform.localScale = new Vector3(2, 1, 1);
                    door.transform.localEulerAngles = new Vector3(-90, 0, 0);
                    door.transform.localScale = new Vector3(2, 1, 1);

                    Collider[] hitColliders = Physics.OverlapSphere(Selection.activeTransform.position, .1f);
                    foreach (Collider coll in hitColliders)
                    {
                        if (coll.name.Contains("Wall") && coll.gameObject != Selection.activeGameObject)
                        {
                            coll.gameObject.SetActive(false);
                            door.AddComponent<WallLink>().Link = coll.transform;
                        }
                    }
                } else
                {
                    if (Selection.activeTransform.Find("CJSEditor_Door(Clone)"))
                    {
                        DestroyImmediate(Selection.activeTransform.Find("CJSEditor_Door(Clone)").gameObject);
                    }

                    Selection.activeTransform.localScale = new Vector3(0.5f, 1, 1);
                    Vector3 localPos = Vector3.zero;
                    switch (Selection.activeTransform.name)
                    {
                        case "Wall_North":
                            localPos = new Vector3(-5, 5, 2.5f);
                            break;
                        case "Wall_West":
                            localPos = new Vector3(-2.5f, 5, -5);
                            break;
                        case "Wall_South":
                            localPos = new Vector3(5, 5, -2.5f);
                            break;
                        case "Wall_East":
                            localPos = new Vector3(2.5f, 5, 5);
                            break;
                    }

                    Selection.activeTransform.localPosition = localPos;
                    if (isMakingRoom) //fuck me 
                    {
                        Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.RoomThinWallTex.mat;
                    }
                    else
                    {
                        Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.DefaultThinWallTex.mat;
                    }

                    GameObject BackWallClone = (GameObject)Instantiate(Selection.activeGameObject, Selection.activeTransform);
                    BackWallClone.transform.localEulerAngles = new Vector3(0, 0, -180);
                    BackWallClone.transform.localPosition = Vector3.zero;
                    BackWallClone.transform.localScale = Vector3.one;

                    GameObject door = (GameObject)Instantiate(DoorPrefab, Selection.activeTransform);

                    //the door instantiates weird
                    door.transform.localPosition = new Vector3(5, 5, 5);
                    door.transform.localScale = new Vector3(2, 1, 1);
                    door.transform.localEulerAngles = new Vector3(-90, 0, 0);

                    //set doorscript values 
                    if (currDecomp != Decomp.BASIC) //basic autosets these values (BASIC VER SHOULD REMOVE THIS BECAUSE THEYRE MADE PRIVATE)
                    {
                        DoorScript doorCode = door.GetComponentInChildren<DoorScript>();
                        doorCode.player = GameObject.FindGameObjectWithTag("Player").transform; //2019 support
                        BaldiScript baldi = FindObjectsOfTypeAll(typeof(BaldiScript))[0] as BaldiScript; //2019 support
                        doorCode.baldi = baldi; 
                    }

                    Collider[] hitColliders = Physics.OverlapSphere(BackWallClone.transform.position, .1f);
                    foreach (Collider coll in hitColliders)
                    {
                        if (coll.name.Contains("Wall") && coll.gameObject != BackWallClone && coll.gameObject != Selection.activeGameObject)
                        {
                            coll.gameObject.SetActive(false);
                            door.AddComponent<WallLink>().Link = coll.transform;
                        }
                    }
                }
            }

            if (Selection.activeTransform.Find("CJSEditor_Door(Clone)") && GUILayout.Button("Remove Door"))
            {
                if (Selection.activeTransform.Find("CJSEditor_Door(Clone)").GetComponent<WallLink>() != null)
                {
                    Selection.activeTransform.Find("CJSEditor_Door(Clone)").GetComponent<WallLink>().Link.gameObject.SetActive(true);
                }

                DestroyImmediate(Selection.activeTransform.Find("CJSEditor_Door(Clone)").gameObject);
                Selection.activeTransform.localScale = new Vector3(1f, 1, 1);
                if (isMakingRoom) //fuck me 
                {
                    Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.RoomWallTex.mat;
                }
                else
                {
                    Selection.activeTransform.GetComponent<Renderer>().material = DefaultTextureProfile.DefaultWallTex.mat;
                }

                Vector3 localPos = Vector3.zero;
                switch (Selection.activeTransform.name)
                {
                    case "Wall_North":
                        localPos = new Vector3(-5, 5, 0);
                        break;
                    case "Wall_West":
                        localPos = new Vector3(0, 5, -5);
                        break;
                    case "Wall_South":
                        localPos = new Vector3(5, 5, 0f);
                        break;
                    case "Wall_East":
                        localPos = new Vector3(0, 5, 5);
                        break;
                }

                Selection.activeTransform.localPosition = localPos;
                Selection.activeTransform.GetComponent<Collider>().enabled = true;
                Selection.activeTransform.GetComponent<Renderer>().enabled = true;
                DestroyImmediate(Selection.activeTransform.Find($"{Selection.activeGameObject.name}(Clone)").gameObject);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);

        //special textures
        MapTexture[] texures = null;
        MapTexture currdefaultexture = new MapTexture(); //non nullable type

        //IM TIRED DONT JUDGE ME OK
        if (currSelectionType == TileObject.Wall)
        {
            texures = DefaultTextureProfile.wallTextures;
            currdefaultexture = DefaultTextureProfile.DefaultWallTex;

            if (isMakingRoom)
            {
                currdefaultexture = DefaultTextureProfile.RoomWallTex;
            }
        }
        if (currSelectionType == TileObject.Floor)
        {
            texures = DefaultTextureProfile.floorTextures;
            currdefaultexture = DefaultTextureProfile.DefaultFloorTex;

            if (isMakingRoom)
            {
                currdefaultexture = DefaultTextureProfile.RoomFloorTex;
            }
        }
        if (currSelectionType == TileObject.Ceiling)
        {
            texures = DefaultTextureProfile.ceilingTextures;
            currdefaultexture = DefaultTextureProfile.DefaultCeilingTex;


            if (isMakingRoom)
            {
                currdefaultexture = DefaultTextureProfile.RoomCeilingTex;
            }
        }

        if (GUILayout.Button("Default Texture"))
        {
            Selection.activeGameObject.GetComponent<Renderer>().material = currdefaultexture.mat;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Extra Textures", EditorStyles.boldLabel);

        foreach (MapTexture tex in texures)
        {
            if (GUILayout.Button(tex.name))
            {
                Selection.activeGameObject.GetComponent<Renderer>().material = tex.mat; 
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical(); 
    }
    private void RenderEditor_Delete()
    {
        EditorGUILayout.HelpBox("Click on a tile to delete it", MessageType.Info); 
    }

    private void RenderTextures()
    {
        SerializedObject serializedData = new SerializedObject(DefaultTextureProfile);
        serializedData.Update();

        textureScrollPosition = EditorGUILayout.BeginScrollView(textureScrollPosition);
        EditorGUILayout.BeginVertical("box");
        SerializedProperty property = serializedData.GetIterator();
        bool enterChildren = true;
        if (property.NextVisible(enterChildren))
        {
            enterChildren = false;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(property, true);
            }
        }
        serializedData.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void RenderTutorial()
    {

    }
}
#endif

public enum TileType
{
    Hall, 
    Room
}
