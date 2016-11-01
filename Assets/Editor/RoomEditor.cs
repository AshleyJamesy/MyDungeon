using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GridView : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 _position01 = new Vector3();
        Vector3 _position02 = new Vector3();

        Gizmos.color = MyDungeon.Editors.RoomWindow.GridColor();

        int _SizeX = MyDungeon.Editors.RoomWindow.GridXSize();
        int _SizeY = MyDungeon.Editors.RoomWindow.GridYSize();

        Vector2 _CellSize = MyDungeon.Editors.RoomWindow.CellSize();

        for (int i = 0; i < _SizeX + 1; i++)
        {
            _position01.x = +(i * _CellSize.x) * 0.01f;
            _position01.y = 0;
            _position02.x = +(i * _CellSize.x) * 0.01f;
            _position02.y = -(_SizeY * _CellSize.y) * 0.01f;

            Gizmos.DrawLine(_position01, _position02);
        }

        for (int i = 0; i < _SizeY + 1; i++)
        {
            _position01.x = 0;
            _position01.y = -(i * _CellSize.y) * 0.01f;
            _position02.x = +(_SizeX * _CellSize.x) * 0.01f;
            _position02.y = -(i * _CellSize.y) * 0.01f;

            Gizmos.DrawLine(_position01, _position02);
        }
    }

}

public class RoomFile : ScriptableObject
{
    public Dictionary<GridCoordinates, int> m_Room = new Dictionary<GridCoordinates, int>();
}

namespace MyDungeon.Editors
{
    [CustomEditor(typeof(Room))]
    public class RoomInspector : Editor
    {
        Room m_Room;

        void OnEnabled()
        {
            m_Room = (Room)target;
        }

        public override void OnInspectorGUI()
        {
            if (!m_Room)
                m_Room = (Room)target;

            Room.m_MaxSize = 
                EditorGUILayout.IntField("MaxSize", Room.m_MaxSize);
            
            Room.m_MaxSize = Mathf.Clamp(Room.m_MaxSize, 2, int.MaxValue);

            Room.m_UseGuides = EditorGUILayout.Toggle("Use Guides", Room.m_UseGuides);

            if (Room.m_GridColor == Color.clear)
                Room.m_GridColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            if (Room.m_RoomColor == Color.clear)
                Room.m_RoomColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            Room.m_RoomColor = EditorGUILayout.ColorField("Room Color", Room.m_RoomColor);
            Room.m_GridColor = EditorGUILayout.ColorField("Grid Color", Room.m_GridColor);

            m_Room.m_SizeX = Mathf.Clamp(m_Room.m_SizeX, 2, Room.m_MaxSize);
            m_Room.m_SizeY = Mathf.Clamp(m_Room.m_SizeY, 2, Room.m_MaxSize);
            
            EditorGUILayout.Separator();

            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Tiles"))
                m_Room.GenerateTiles();
        }

    }

    public class RoomWindow : EditorWindow
    {
        static TileSet m_TileSet;

        static int m_Type = 0;
        static GameObject m_Prefab;

        static int m_GridXSize = 10;
        static int m_GridYSize = 10;

        static Vector2 m_CellSize = 
            new Vector2(128, 128);
        
        static Color m_GridColor = 
            new Color(1.0f, 1.0f, 1.0f, 0.5f);

        bool m_MouseDown = false;

        Vector2 m_MousePos = Vector2.zero;

        static GameObject m_Grid;

        static Dictionary<GridCoordinates, GameObject> m_Room = new Dictionary<GridCoordinates, GameObject>();

        public enum RoomEditMode
        {
            NoMode,
            EditMode
        }

        static RoomEditMode m_Mode = RoomEditMode.NoMode;

        [MenuItem("Window/My Window")]
        public static void ShowWindow()
        {
            GetWindow(typeof(RoomWindow));

            if (!m_Grid)
                m_Grid = new GameObject("Grid");

            m_Grid.AddComponent<GridView>();

            m_Grid.layer = LayerMask.NameToLayer("UI");

            m_Grid.hideFlags = 
                HideFlags.HideInInspector | HideFlags.DontSave;
        }
        
        [MenuItem("Assets/Create/TileSet")]
        public static void CreateTileSet()
        {
            TileSet _Asset = 
                CreateInstance<TileSet>();

            string _Path = 
                AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(_Path))
                _Path = "Assets/Room Editor/TileSets";
            else if (Path.GetExtension(_Path) != "")
                _Path.Replace(Path.GetFileName(_Path), "");
            else
                _Path += "/";

            string _AssetAndPath 
                = AssetDatabase.GenerateUniqueAssetPath(_Path + "TileSet.asset");

            AssetDatabase.CreateAsset(_Asset, _AssetAndPath);

            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _Asset;

            _Asset.hideFlags = HideFlags.DontSave;
        }

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += RoomSceneGUI;

            Tools.lockedLayers |= 1 << LayerMask.NameToLayer("UI");

            titleContent.text = "Room Editor";
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= RoomSceneGUI;

            Tools.lockedLayers &= ~(1 << LayerMask.NameToLayer("UI"));

            m_Mode = RoomEditMode.NoMode;

            if (m_Grid)
                DestroyImmediate(m_Grid);
        }
        
        void Update()
        {
            if (m_Mode == RoomEditMode.EditMode)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Repaint();
        }

        void RoomSceneGUI(SceneView sceneView)
        {
            Event _event = Event.current;
            Ray worldRays = HandleUtility.GUIPointToWorldRay(_event.mousePosition);

            m_MousePos = worldRays.origin;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (_event.type == EventType.layout)
                HandleUtility.AddDefaultControl(controlID);

            switch (_event.type)
            {
                case EventType.mouseDown:
                    {
                        if (m_Mode == RoomEditMode.EditMode)
                        {
                            if (_event.button == 0)
                            {
                                m_MouseDown = true;
                                _event.Use();
                            }
                        }
                    }
                    break;
                case EventType.mouseUp:
                    {
                        if(m_Mode == RoomEditMode.EditMode)
                        {
                            if (_event.button == 0)
                            {
                                m_MouseDown = false;
                                _event.Use();
                            }
                        }
                    }
                    break;
            }

            if(m_MouseDown)
            {
                int _GridX = Mathf.FloorToInt((m_MousePos.x / (m_CellSize.x * 0.01f)));
                int _GridY = Mathf.FloorToInt((m_MousePos.y / (m_CellSize.y * 0.01f)));

                GridCoordinates _GridCoordinates = new GridCoordinates(_GridX, -(_GridY + 1));

                if (_GridCoordinates.m_X >= 0 && _GridCoordinates.m_Y >= 0 && _GridCoordinates.m_X <= m_GridXSize - 1 && _GridCoordinates.m_Y <= m_GridYSize - 1)
                {
                    GameObject _gameObject = GetFromMap(_GridCoordinates);

                    if (_gameObject)
                    {
                        if (m_Prefab)
                        {
                            
                        }
                        else
                        {
                            DestroyImmediate(_gameObject);
                        }
                    }
                    else
                        if (m_Prefab)
                            CreateTile(_GridCoordinates);
                }
            }
        }

        void OnGUI()
        {
            m_GridXSize = EditorGUILayout.IntField("Grid Size X", m_GridXSize);
            m_GridXSize = Mathf.Clamp(m_GridXSize, 1, int.MaxValue);

            m_GridYSize = EditorGUILayout.IntField("Grid Size Y", m_GridYSize);
            m_GridYSize = Mathf.Clamp(m_GridYSize, 1, int.MaxValue);

            m_CellSize = EditorGUILayout.Vector2Field("Cell Size", m_CellSize);
            m_CellSize.x = Mathf.Clamp(m_CellSize.x, 1, float.MaxValue);
            m_CellSize.y = Mathf.Clamp(m_CellSize.y, 1, float.MaxValue);

            SceneView.RepaintAll();

            m_TileSet =
                EditorGUILayout.ObjectField("TileSet", m_TileSet, typeof(TileSet), false) as TileSet;

            if (m_Prefab)
                GUILayout.Box(AssetPreview.GetAssetPreview(m_Prefab), GUILayout.Width(64), GUILayout.Height(64));
            else
                GUILayout.Box("No Tile", GUILayout.Width(64), GUILayout.Height(64));

            GUILayout.Label("Type: " + m_Type.ToString());

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("No Tile", GUILayout.Width(64), GUILayout.Height(64)))
            {
                m_Type = 0;
                m_Prefab = null;
            }

            if (m_TileSet)
            {
                Texture2D _texture = null;

                for (int i = 0; i < m_TileSet.m_Tiles.Length; i++)
                {
                    Object _object = m_TileSet.m_Tiles[i];

                    _texture = AssetPreview.GetAssetPreview(_object);

                    if (_texture)
                        if (GUILayout.Button(_texture, GUILayout.Width(64), GUILayout.Height(64)))
                        {
                            m_Type      = i + 1;
                            m_Prefab    = _object as GameObject;
                        }
                }
            }

            GUILayout.EndHorizontal();

            switch (m_Mode)
            {
                case RoomEditMode.NoMode:
                    if (GUILayout.Button("Enable Editor"))
                        m_Mode = RoomEditMode.EditMode;
                    break;
                case RoomEditMode.EditMode:
                    if (GUILayout.Button("Disable Editor"))
                        m_Mode = RoomEditMode.NoMode;
                    break;
                default:
                    break;
            }

            if (GUILayout.Button("Export Room"))
                ExportRoom();

            if (GUILayout.Button("Import Room"))
                ImportRoom();
        }
        
        public static RoomEditMode Mode()
        {
            return m_Mode;
        }

        public static Vector2 CellSize()
        {
            return m_CellSize;
        }

        public static int GridXSize()
        {
            return m_GridXSize;
        }

        public static int GridYSize()
        {
            return m_GridYSize;
        }

        public static Color GridColor()
        {
            return m_GridColor;
        }
        
        public static void CreateTile(GridCoordinates _coordinates)
        {
            foreach (KeyValuePair<GridCoordinates, GameObject> _pair in m_Room)
            {
                if (_pair.Key.m_X == _coordinates.m_X && _pair.Key.m_Y == _coordinates.m_Y)
                {
                    m_Room.Remove(_pair.Key);

                    break;
                }
            }

            GameObject _gameObject = CreateObject(_coordinates);

            if (_gameObject)
            {
                if(m_Prefab)
                {
                    
                }

                Tile _tile = _gameObject.GetComponent<Tile>();

                if(_tile)
                    _tile.m_Type = m_Type;

                m_Room.Add(new GridCoordinates(_coordinates.m_X, _coordinates.m_Y), _gameObject);
            }
        }
        
        static GameObject CreateObject(GridCoordinates _coordinates)
        {
            float _x = (m_CellSize.x * 0.01f * 0.5f) + (_coordinates.m_X * (m_CellSize.x * 0.01f));
            float _y = (m_CellSize.y * 0.01f * 0.5f) + (_coordinates.m_Y * (m_CellSize.y * 0.01f));

            GameObject _gameObject = null;

            if (m_Prefab)
            {
                _gameObject = Instantiate(m_Prefab);

                _gameObject.name =
                string.Format("{0}x{1}", _coordinates.m_X, _coordinates.m_Y);

                _gameObject.transform.position
                                = new Vector3(+_x, -_y, 0);
            }

            return _gameObject;
        }

        static GameObject GetFromMap(GridCoordinates _coordinates)
        {
            foreach (KeyValuePair<GridCoordinates, GameObject> _pair in m_Room)
            {
                if (_pair.Key.m_X == _coordinates.m_X && _pair.Key.m_Y == _coordinates.m_Y)
                    return _pair.Value;
            }

            return null;
        }
        
        void ExportRoom()
        {
            RoomFile _Asset =
                CreateInstance<RoomFile>();

            string _Path =
                AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(_Path))
                _Path = "Assets/Room Editor/Rooms";
            else if (Path.GetExtension(_Path) != "")
                _Path.Replace(Path.GetFileName(_Path), "");
            else
                _Path += "/";

            string _AssetAndPath
                = AssetDatabase.GenerateUniqueAssetPath(_Path + "Room.asset");

            AssetDatabase.CreateAsset(_Asset, _AssetAndPath);

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = _Asset;

            foreach (KeyValuePair<GridCoordinates, GameObject> _pair in m_Room)
            {
                Tile _tile = _pair.Value.GetComponent<Tile>();

                if(_tile)
                    _Asset.m_Room.Add(new GridCoordinates(_pair.Key.m_X, _pair.Key.m_Y), _tile.m_Type);
            }

            _Asset.hideFlags = HideFlags.DontSave;
        }

        void ImportRoom()
        {
            RoomFile _room = Selection.activeObject as RoomFile;

            if (_room == null)
            {
                EditorUtility.DisplayDialog("Select Room", "You must select a Room first!", "OK");
                return;
            }

            foreach (KeyValuePair<GridCoordinates, GameObject> _pair in m_Room)
            {
                if (_pair.Value)
                    DestroyImmediate(_pair.Value);
            }

            m_Room.Clear();

            foreach (KeyValuePair<GridCoordinates, int> _pair in _room.m_Room)
            {
                if((_pair.Value - 1) < m_TileSet.m_Tiles.Length)
                {
                    m_Prefab = m_TileSet.m_Tiles[_pair.Value - 1];

                    CreateTile(_pair.Key);
                }
            }
        }

    }
}