using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public int m_X = 0;
    public int m_Y = 0;

    SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    Sprite[] m_Types;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCoordinates(int _x, int _y)
    {
        m_X = _x;
        m_Y = _y;

        gameObject.name = string.Format("{0}x{1}", m_X, m_Y);
    }
    
    public void SetType(int _type)
    {
        if (_type > m_Types.Length)
            _type = 0;

        m_SpriteRenderer.sprite = m_Types[_type];
    }
    
}
