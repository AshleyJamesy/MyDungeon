using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    static public GameObject m_DefaultTile;

    static public int m_MaxSize = 16;

    [HideInInspector]
    static public bool m_UseGuides = true;
    [HideInInspector]
    static public Color m_RoomColor;
    [HideInInspector]
    static public Color m_GridColor;

    [SerializeField]
    public int m_TileSize;

    [SerializeField]
    public int m_SizeX;
    [SerializeField]
    public int m_SizeY;

    public Vector3 Size
    {
        get
        {
            m_Size.x = m_SizeX * m_TileSize * 0.01f;
            m_Size.y = m_SizeY * m_TileSize * 0.01f;
            m_Size.z = 0;

            return m_Size;
        }
    }
    Vector3 m_Size;

    public void GenerateTiles()
    {
        Tile[] _tiles = GetComponentsInChildren<Tile>();

        foreach (Tile _tile in _tiles)
            DestroyImmediate(_tile.gameObject);

        if(m_DefaultTile)
        {
            Vector3 _position = new Vector3(0, 0, 0);

            for (int i = 0; i < m_SizeX; i++)
                for (int j = 0; j < m_SizeY; j++)
                {
                    GameObject _gameObject = Instantiate(m_DefaultTile, transform) as GameObject;

                    _position.x = (i * m_TileSize * 0.01f) - ((m_SizeX -1) * m_TileSize * 0.5f * 0.01f);
                    _position.y = (j * m_TileSize * 0.01f) - ((m_SizeY -1) * m_TileSize * 0.5f * 0.01f);

                    _gameObject.transform.position = transform.position + _position;

                    Tile _tile = _gameObject.GetComponent<Tile>();

                    if (_tile)
                    {
                        //_tile.SetCoordinates(i, j);
                        //_tile.SetType(0);
                    }
                }
        }
    }

    void OnDrawGizmos()
    {
        if(m_UseGuides)
        {
            //Gizmos.color = m_RoomColor;
            //Gizmos.DrawCube(transform.position, Size);

            Vector3 _position01 = new Vector3();
            Vector3 _position02 = new Vector3();

            Gizmos.color = m_GridColor;

            for (int i = 0; i < m_SizeX + 1; i++)
            {
                _position01.x = +(i * m_TileSize - (m_SizeX * m_TileSize) * 0.5f) * 0.01f;
                _position01.y = -((m_SizeY * m_TileSize) * 0.5f) * 0.01f;

                _position02.x = +(i * m_TileSize - (m_SizeX * m_TileSize) * 0.5f) * 0.01f;
                _position02.y = +((m_SizeY * m_TileSize) * 0.5f) * 0.01f;

                Gizmos.DrawLine(transform.position + _position01, transform.position + _position02);
            }

            for (int i = 0; i < m_SizeY + 1; i++)
            {
                _position01.x = -((m_SizeX * m_TileSize) * 0.5f) * 0.01f;
                _position01.y = +(i * m_TileSize - (m_SizeY * m_TileSize) * 0.5f) * 0.01f;

                _position02.x = +((m_SizeX * m_TileSize) * 0.5f) * 0.01f;
                _position02.y = +(i * m_TileSize - (m_SizeY * m_TileSize) * 0.5f) * 0.01f;

                Gizmos.DrawLine(transform.position + _position01, transform.position + _position02);
            }
        }
    }
    
}
