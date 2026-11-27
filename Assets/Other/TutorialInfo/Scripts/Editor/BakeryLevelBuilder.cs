using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : EditorWindow
{
    [Serializable]
    private class Entry
    {
        public GameObject prefab;
        public Vector3 offset;
        public string assetGuid;
    }

    private const string SettingsKey = "BakeryLevelBuilder.Settings.v2";
    [SerializeField] private List<Entry> prefabs = new List<Entry>();
    [SerializeField] private float gridSize = 1f, floorHeight = 0f, rotationStep = 90f;
    [SerializeField] private int selectedPrefab;
    [SerializeField] private bool snap = true, checkOccupied = true, showGrid = true, paint = false;
    private bool building;
    private float rotation;
    private Transform parent;
    private Vector2 scroll;
    private Material previewMaterial;
    private bool painting;
    private Vector3? lastPaintPosition;

    [MenuItem("Tools/Bakery Level Builder")]
    public static void OpenWindow() => GetWindow<LevelBuilder>("Bakery Builder");

    private void OnEnable()
    {
        string saved = EditorPrefs.GetString(SettingsKey + Application.dataPath, "");
        if (!string.IsNullOrEmpty(saved)) EditorJsonUtility.FromJsonOverwrite(saved, this);
        foreach (Entry entry in prefabs)
            if (!string.IsNullOrEmpty(entry.assetGuid))
                entry.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(entry.assetGuid));
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
        SaveSettings();
        if (previewMaterial != null) DestroyImmediate(previewMaterial);
    }

    private void SaveSettings()
    {
        foreach (Entry entry in prefabs)
            entry.assetGuid = entry.prefab == null ? "" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(entry.prefab));
        EditorPrefs.SetString(SettingsKey + Application.dataPath, EditorJsonUtility.ToJson(this));
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
        gridSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("Grid Size", gridSize));
        if (GUILayout.Button("1 / 0.5 / 0.25: Cycle Grid"))
            gridSize = gridSize > 0.75f ? 0.5f : gridSize > 0.375f ? 0.25f : 1f;
        floorHeight = EditorGUILayout.FloatField("Floor Height", floorHeight);
        snap = EditorGUILayout.Toggle("Snap to Grid", snap);
        showGrid = EditorGUILayout.Toggle("Show Grid", showGrid);
        checkOccupied = EditorGUILayout.Toggle("Check Same Pivot", checkOccupied);
        paint = EditorGUILayout.Toggle("Drag to Paint", paint);
        rotationStep = Mathf.Clamp(EditorGUILayout.FloatField("Rotation Step", rotationStep), 1f, 360f);
        parent = (Transform)EditorGUILayout.ObjectField("Object Parent", parent, typeof(Transform), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefab Palette", EditorStyles.boldLabel);
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedPrefab == i, (i + 1).ToString(), "Button", GUILayout.Width(30))) selectedPrefab = i;
            prefabs[i].prefab = (GameObject)EditorGUILayout.ObjectField(prefabs[i].prefab, typeof(GameObject), false);
            bool remove = GUILayout.Button("X", GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
            if (remove)
            {
                prefabs.RemoveAt(i);
                selectedPrefab = Mathf.Clamp(selectedPrefab, 0, Mathf.Max(0, prefabs.Count - 1));
                break;
            }
            if (selectedPrefab == i)
            {
                prefabs[i].offset = EditorGUILayout.Vector3Field("World Offset", prefabs[i].offset);
                EditorGUILayout.HelpBox("X/Z shift the snapping lattice in world axes; Y adjusts height. X/Z apply only when snapping.", MessageType.None);
            }
        }
        if (GUILayout.Button("Add Prefab")) { prefabs.Add(new Entry()); selectedPrefab = prefabs.Count - 1; }
        EditorGUILayout.LabelField("Rotation", rotation.ToString("0.#") + " degrees");
        if (GUILayout.Button("Rotate")) Rotate();
        if (GUILayout.Button("Reset Rotation")) rotation = 0f;
        if (GUILayout.Button(building ? "Disable Building" : "Enable Building")) { building = !building; painting = false; }
        if (parent == null) EditorGUILayout.HelpBox("Assign a scene object as parent.", MessageType.Warning);
        EditorGUILayout.HelpBox("Left click: Place | Shift + left click: Remove hovered prefab under parent | R: Rotate | Esc: Stop | Alt / right mouse: Navigate | Ctrl+Z: Undo", MessageType.Info);
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck()) { SaveSettings(); SceneView.RepaintAll(); }
    }

    private void Rotate() { rotation = Mathf.Repeat(rotation + rotationStep, 360f); SceneView.RepaintAll(); Repaint(); }
    private Entry Selected => selectedPrefab >= 0 && selectedPrefab < prefabs.Count ? prefabs[selectedPrefab] : null;

    private void DuringSceneGUI(SceneView view)
    {
        if (!building || EditorApplication.isPlayingOrWillChangePlaymode || parent == null || EditorUtility.IsPersistent(parent)) return;
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) { building = false; painting = false; e.Use(); Repaint(); view.Repaint(); return; }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R && !e.control && !e.command && !e.alt) { Rotate(); e.Use(); }
        if (e.type == EventType.MouseUp && e.button == 0) { painting = false; lastPaintPosition = null; }
        if (e.type == EventType.Layout && !e.alt) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (!new Plane(Vector3.up, new Vector3(0, floorHeight, 0)).Raycast(ray, out float distance)) return;
        Vector3 point = ray.GetPoint(distance);
        Entry entry = Selected;
        Vector3 offset = entry == null ? Vector3.zero : entry.offset;
        Vector3 pos = snap ? new Vector3(
            Mathf.Round((point.x - offset.x) / gridSize) * gridSize + offset.x,
            floorHeight + offset.y,
            Mathf.Round((point.z - offset.z) / gridSize) * gridSize + offset.z)
            : point + Vector3.up * offset.y;
        GameObject prefab = entry?.prefab;
        bool valid = prefab != null && PrefabUtility.IsPartOfPrefabAsset(prefab) && (!checkOccupied || !Occupied(pos));
        Quaternion facing = Quaternion.Euler(0, rotation, 0);
        if (e.type == EventType.Repaint)
        {
            if (showGrid) DrawGrid(point, entry == null ? Vector3.zero : entry.offset);
            Handles.color = valid ? Color.green : Color.red;
            Handles.DrawWireCube(pos + Vector3.up * 0.025f, new Vector3(gridSize, 0.01f, gridSize));
            if (prefab != null) DrawMeshPreview(prefab, pos, facing, valid);
            Handles.ArrowHandleCap(0, pos + Vector3.up * 0.1f, facing, gridSize * 0.45f, EventType.Repaint);
        }
        if (!e.alt && !e.control && !e.command && e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                if (e.shift) RemoveHovered(e.mousePosition);
                else { painting = paint && snap; lastPaintPosition = pos; if (valid) Place(prefab, pos, facing); }
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && painting && !e.shift)
            {
                if (!lastPaintPosition.HasValue || (lastPaintPosition.Value - pos).sqrMagnitude > 0.000001f)
                { if (valid) Place(prefab, pos, facing); lastPaintPosition = pos; }
                e.Use();
            }
        }
        if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag || e.type == EventType.Used) view.Repaint();
    }

    private bool Occupied(Vector3 pos)
    {
        foreach (Transform child in parent) if ((child.position - pos).sqrMagnitude < 0.000001f) return true;
        return false;
    }

    private void Place(GameObject prefab, Vector3 pos, Quaternion facing)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        obj.transform.SetPositionAndRotation(pos, facing);
        Undo.RegisterCreatedObjectUndo(obj, "Place Bakery Object");
    }

    private void RemoveHovered(Vector2 mouse)
    {
        GameObject picked = HandleUtility.PickGameObject(mouse, false);
        if (picked == null || picked.transform == parent || !picked.transform.IsChildOf(parent)) return;
        Transform root = picked.transform;
        while (root.parent != parent) root = root.parent;
        Undo.DestroyObjectImmediate(root.gameObject);
    }

    private void DrawGrid(Vector3 point, Vector3 offset)
    {
        const int radius = 10;
        float x = Mathf.Round((point.x - offset.x) / gridSize) * gridSize + offset.x;
        float z = Mathf.Round((point.z - offset.z) / gridSize) * gridSize + offset.z;
        float y = floorHeight + 0.02f;
        float extent = (radius + 0.5f) * gridSize;
        Handles.color = new Color(1, 1, 1, 0.25f);
        for (int i = -radius; i <= radius + 1; i++)
        {
            float d = (i - 0.5f) * gridSize;
            Handles.DrawLine(new Vector3(x + d, y, z - extent), new Vector3(x + d, y, z + extent));
            Handles.DrawLine(new Vector3(x - extent, y, z + d), new Vector3(x + extent, y, z + d));
        }
    }

    private void DrawMeshPreview(GameObject prefab, Vector3 pos, Quaternion facing, bool valid)
    {
        // Draw meshes only: no preview GameObject or gameplay components are instantiated.
        if (previewMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null) return;
            previewMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetInt("_ZWrite", 0);
            previewMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
        }
        previewMaterial.SetColor("_Color", valid ? new Color(0.2f, 1f, 0.4f, 0.35f) : new Color(1f, 0.2f, 0.2f, 0.35f));
        Matrix4x4 root = Matrix4x4.TRS(pos, facing, prefab.transform.localScale);
        foreach (MeshFilter filter in prefab.GetComponentsInChildren<MeshFilter>())
        {
            if (filter.sharedMesh == null) continue;
            Matrix4x4 matrix = root * prefab.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            if (previewMaterial.SetPass(0)) Graphics.DrawMeshNow(filter.sharedMesh, matrix);
        }
    }
}
