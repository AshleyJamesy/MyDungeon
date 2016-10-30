using UnityEngine;
using UnityEditor;
using System.Collections;


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
        Vector2 m_MousePos;

        Object m_Prefab;

        [MenuItem("Window/My Window")]
        public static void ShowWindow()
        {
            GetWindow(typeof(RoomWindow));
        }
        
        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += RoomSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= RoomSceneGUI;
        }

        void RoomSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;

            Ray _WorldRays =
                HandleUtility.GUIPointToWorldRay(e.mousePosition);

            m_MousePos = _WorldRays.origin;

            switch (e.type)
            {
                case EventType.mouseDown:
                    {
                        GameObject _activeGameObject = Selection.activeGameObject;

                        if (_activeGameObject)
                        {
                            Room _room = _activeGameObject.GetComponent<Room>();

                            if (!_room)
                                return;
                        }
                    }
                    break;
                case EventType.mouseUp:
                    {

                    }
                    break;
            }
        }

        void OnGUI()
        {
            if (AssetDatabase.IsValidFolder("Assets/Room Editor/Tiles"))
            {
                string[] _folders = new string[] { "Assets/Room Editor/Tiles" };

                string[] _ids = AssetDatabase.FindAssets("tile", _folders);

                EditorGUILayout.Separator();

                Room.m_DefaultTile = EditorGUILayout.ObjectField(Room.m_DefaultTile, typeof(GameObject), false) as GameObject;

                GUILayout.Label("Tile Selection");
                EditorGUILayout.Separator();

                if (m_Prefab)
                {
                    Texture _tile = AssetPreview.GetAssetPreview(m_Prefab);

                    GUILayout.Box(_tile, GUILayout.Width(64), GUILayout.Height(64));
                }
                else
                    GUILayout.Box("", GUILayout.Width(64), GUILayout.Height(64));

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("No Tile", GUILayout.Width(64), GUILayout.Height(64)))
                    m_Prefab = null;

                for (int i = 0; i < _ids.Length; ++i)
                {
                    string _path =
                        AssetDatabase.GUIDToAssetPath(_ids[i]);

                    Object _object =
                        AssetDatabase.LoadAssetAtPath(_path, typeof(GameObject));

                    DrawThumb(_object, 64, 64);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawThumb(Object _object, int _width, int _height)
        {
            if (_object == null)
                return;

            Texture _thumb = AssetPreview.GetAssetPreview(_object);

            if (_thumb)
            {
                if (GUILayout.Button(new GUIContent(_thumb, _object.name), GUILayout.Width(_width), GUILayout.Height(_height)))
                    m_Prefab = _object as GameObject;

                return;
            }

            if (AssetPreview.IsLoadingAssetPreview(_object.GetInstanceID()))
            {
                Repaint();
                GUILayout.Box("...", GUILayout.Width(_width), GUILayout.Height(_height));
            }
            else
                GUILayout.Box("x", GUILayout.Width(_width), GUILayout.Height(_height));
        }

    }
}