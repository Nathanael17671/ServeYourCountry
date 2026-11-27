using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BakeryLevelBuilder : EditorWindow
{
    private float gridSize = 1f;
    private float floorHeight = 0f;
    private int gridRadius = 10;
    private bool buildingEnabled = false;
    private bool showGrid = true;
    private int selectedPrefab = 0;
    private float rotation = 0f;

    private Transform objectParent;
    private List<GameObject> prefabs = new List<GameObject>();

    [MenuItem("Tools/Bakery Level Builder")]
    public static void OpenWindow()
    {
        GetWindow<BakeryLevelBuilder>("Bakery Builder");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);

        gridSize = Mathf.Max(0.1f,EditorGUILayout.FloatField("Grid Size", gridSize));
            
        floorHeight = EditorGUILayout.FloatField("Floor Height", floorHeight);
           
        gridRadius = EditorGUILayout.IntSlider("Grid Radius", gridRadius, 1, 30);  

        showGrid = EditorGUILayout.Toggle("Show Grid", showGrid);

        objectParent = (Transform)EditorGUILayout.ObjectField("Object Parent",objectParent,typeof(Transform),true);

        EditorGUILayout.Space();

        GUILayout.Label("Prefabs", EditorStyles.boldLabel);

        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Toggle(selectedPrefab == i,(i + 1).ToString(),"Button",GUILayout.Width(30)))

            {
                selectedPrefab = i;
            }

            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
                
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                prefabs.RemoveAt(i);

                selectedPrefab = Mathf.Clamp(selectedPrefab, 0,Mathf.Max(0, prefabs.Count - 1));
                    
                    

                EditorGUILayout.EndHorizontal();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Prefab"))
        {
            prefabs.Add(null);
        }

        EditorGUILayout.Space();

        GUILayout.Label("Placement", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Rotation", rotation + " degrees");
            
        if (GUILayout.Button("Rotate 90 Degrees"))
        {
            RotateObject();
        }

        if (GUILayout.Button(buildingEnabled ? "Disable Building" : "Enable Building"))
            
        {
            buildingEnabled = !buildingEnabled;
        }

        if (objectParent == null)
        {
            EditorGUILayout.HelpBox("Assign an Object Parent before building.",MessageType.Warning);
        }

        EditorGUILayout.HelpBox("Left Click: Place Object\n" + "Right Click: Remove Object\n" + "R: Rotate Object\n" + "Escape: Disable Building\n" + "Ctrl + Z: Undo",MessageType.Info);
            
        SceneView.RepaintAll();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!buildingEnabled || Application.isPlaying) return;
 
        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.KeyDown)
        {
            if (currentEvent.keyCode == KeyCode.Escape)
            {
                buildingEnabled = false;
                currentEvent.Use();
                Repaint();
                return;
            }

            if (currentEvent.keyCode == KeyCode.R)
            {
                RotateObject();
                currentEvent.Use();
                Repaint();
            }
        }

        if (objectParent == null)
            return;

        // Prevent normal Scene selection while building.
        if (currentEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        // Convert the mouse position into a world-space ray.
        Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
          
        // Create a horizontal placement plane.
        Plane groundPlane = new Plane(Vector3.up,new Vector3(0f, floorHeight, 0f));

        if (!groundPlane.Raycast(ray, out float distance))
            return;

        Vector3 hitPoint = ray.GetPoint(distance);

        Vector3 snappedPosition = SnapToGrid(hitPoint);

        if (showGrid)
        {
            DrawGrid(snappedPosition);
        }

        GameObject occupiedObject = FindObject(snappedPosition);

        bool validPrefab = selectedPrefab >= 0 && selectedPrefab < prefabs.Count && prefabs[selectedPrefab] != null && PrefabUtility.IsPartOfPrefabAsset(prefabs[selectedPrefab]);
        
        bool canPlace = occupiedObject == null && validPrefab;

        DrawPreview(snappedPosition, canPlace);

        // Allow Alt + mouse for Scene camera navigation.
        if (!currentEvent.alt && currentEvent.type == EventType.MouseDown)
           
        {
            // Left-click to place.
            if (currentEvent.button == 0)
            {
                if (canPlace)
                {
                    PlaceObject(snappedPosition);
                }

                currentEvent.Use();
            }

            // Right-click to remove.
            else if (currentEvent.button == 1)
            {
                if (occupiedObject != null)
                {
                    Undo.DestroyObjectImmediate(occupiedObject);
                    currentEvent.Use();
                }
            }
        }

        sceneView.Repaint();
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x / gridSize) * gridSize,floorHeight,Mathf.Round(position.z / gridSize) * gridSize);
    }

    private void PlaceObject(Vector3 position)
    {
        GameObject prefab = prefabs[selectedPrefab];

        GameObject newObject = (GameObject) PrefabUtility.InstantiatePrefab(prefab, objectParent);
            
        newObject.transform.position = position;

        newObject.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            
        Undo.RegisterCreatedObjectUndo(newObject, "Place Bakery Object");  
    }

    private GameObject FindObject(Vector3 position)
    {
        foreach (Transform child in objectParent)
        {
            if (Vector3.Distance(child.position, position) < 0.01f)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void RotateObject()
    {
        rotation = (rotation + 90f) % 360f;

        SceneView.RepaintAll();
    }

    private void DrawGrid(Vector3 center)
    {
        Handles.color = new Color(1f, 1f, 1f, 0.25f);

        float half = gridSize / 2f;
        float extent = gridRadius * gridSize + half;
        float height = floorHeight + 0.02f;

        for (int i = -gridRadius; i <= gridRadius + 1; i++)
        {
            float offset = i * gridSize - half;

            Handles.DrawLine(new Vector3(center.x + offset, height, center.z - extent), new Vector3(center.x + offset, height, center.z + extent));

            Handles.DrawLine(new Vector3(center.x - extent, height, center.z + offset), new Vector3(center.x + extent, height, center.z + offset)); 
        }
    }

    private void DrawPreview(Vector3 position, bool canPlace)
    {
        Handles.color = canPlace ? Color.green : Color.red;

        Vector3 previewPosition = position + Vector3.up * 0.03f;
            
        Handles.DrawWireCube(previewPosition, new Vector3(gridSize, 0.02f, gridSize));
        
        // Show which direction the prefab will face.
        Quaternion previewRotation = Quaternion.Euler(0f, rotation, 0f);

        Handles.ArrowHandleCap(0, previewPosition + Vector3.up * 0.1f, previewRotation, gridSize * 0.45f, EventType.Repaint);
    }
}
