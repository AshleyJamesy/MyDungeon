using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCoordinates
{
    public GridCoordinates(int _x, int _y)
    {
        m_X = _x;
        m_Y = _y;
    }

    public int m_X = 0;
    public int m_Y = 0;
}

public class ChunkManager : MonoBehaviour {

    [SerializeField]
    int m_Size = 64;
    [SerializeField]
    int m_TileSize = 64;
    [SerializeField]
    float m_TileOffset = 0;
    [SerializeField]
    int m_SizeX = 3;
    [SerializeField]
    int m_SizeY = 3;

    [SerializeField]
    GameObject m_Chunk;

    List<GameObject> m_Chunks;

    [SerializeField]
    GameObject m_Target;

    void Start()
    {
        m_Chunks = new List<GameObject>();

        Vector3 _position = new Vector3(0, 0, 0);

        for (int i = 0; i < m_SizeX; i++)
            for (int j = 0; j < m_SizeY; j++)
            {
                _position.x = +(j * (m_Size * m_TileSize) / 100.0f) - ((m_Size * m_TileSize)/100.0f);
                _position.y = -(i * (m_Size * m_TileSize) / 100.0f) + ((m_Size * m_TileSize)/100.0f);

                GameObject _gameObject = Instantiate(m_Chunk, _position, Quaternion.identity) as GameObject;

                MyDungeon.Chunk _chunk = 
                    _gameObject.GetComponent<MyDungeon.Chunk>();

                _chunk.m_X          = i;
                _chunk.m_Y          = j;
                _chunk.m_Size       = m_Size;
                _chunk.m_TileSize   = m_TileSize;
                _chunk.m_TileOffset = m_TileOffset;

                _chunk.SetName();
                _chunk.GenerateTiles();

                m_Chunks.Add(_gameObject);
            }
    }
    
}
