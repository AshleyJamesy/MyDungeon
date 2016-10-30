using UnityEngine;
using System.Collections;

namespace MyDungeon
{
    public class Chunk : MonoBehaviour
    {
        public int m_X = 0;
        public int m_Y = 0;

        [SerializeField]
        GameObject m_Tile;
        [SerializeField]
        public int m_Size = 64;
        [SerializeField]
        public int m_TileSize = 64;
        [SerializeField]
        public float m_TileOffset = 0;

        GridCoordinates[] m_Entrances;
        GridCoordinates[] m_Exits;

        [SerializeField]
        int m_Seed = 0;

        void Start()
        {
            if(Application.isEditor)
                GenerateTiles();
        }

        public void GenerateTiles()
        {
            DestroyTiles();

            Random.InitState(m_Seed);

            Vector3 _position = new Vector3(0, 0, 0);

            for (int i = 0; i < m_Size; i++)
                for (int j = 0; j < m_Size; j++)
                {
                    _position.x = (+((i * m_TileSize) / 100.0f)) - ((m_Size * m_TileSize) / 100.0f) / 2.0f;
                    _position.y = (-((j * m_TileSize) / 100.0f)) + ((m_Size * m_TileSize) / 100.0f) / 2.0f;

                    GameObject _object =
                        Instantiate(m_Tile, gameObject.transform) as GameObject;

                    _object.transform.position = transform.position + _position;

                    Tile _tile
                        = _object.GetComponent<Tile>();

                    _tile.SetCoordinates(i, j);
                    _tile.SetType(0);
                }
        }

        public void DestroyTiles()
        {
            for (int i = 0; i < transform.childCount; i++)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        public void UpdateTiles()
        {
            Random.InitState(m_Seed);

            Tile[] _tiles = 
                GetComponentsInChildren<Tile>();

            //Read in tile data here

            for (int i = 0; i < _tiles.Length; i++)
                _tiles[i].SetType(0);
        }

        public void SetName()
        {
            gameObject.name = string.Format("{0}x{1}", m_X, m_Y);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawCube(transform.position, new Vector3(((m_Size * m_TileSize) / 100.0f), ((m_Size * m_TileSize) / 100.0f), 0));
        }

    }
}
