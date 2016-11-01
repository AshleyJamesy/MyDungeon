using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MyDungeon.Editors
{
    [CustomEditor(typeof(TileSet))]
    public class TileSetInspector : Editor
    {
        TileSet m_TileSet;

        void OnEnabled()
        {
            m_TileSet = target as TileSet;
        }

        public override void OnInspectorGUI()
        {
            if (!m_TileSet)
                m_TileSet = target as TileSet;

            base.OnInspectorGUI();
            
            Texture2D _texture = null;

            if(m_TileSet)
            {
                foreach (Object _object in m_TileSet.m_Tiles)
                {
                    _texture = AssetPreview.GetAssetPreview(_object);

                    if (_texture)
                        GUILayout.Button(_texture, GUILayout.Width(64), GUILayout.Height(64));
                }
            }
        }

    }
}
