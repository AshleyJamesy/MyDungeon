using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Image m_Joystick;

    public bool m_InvertX;
    public bool m_InvertY;

    public float m_MaxZone;
    public float m_DeadZone;

    Vector2 m_Position;

    private Image m_Zone;

    private void Start()
    {
        m_Zone = GetComponent<Image>();

        Screen.orientation = ScreenOrientation.Landscape;
    }

    public virtual void OnPointerUp(PointerEventData data)
    {
        m_Position = Vector2.zero;

        m_Joystick.rectTransform.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData data)
    {

    }

    public virtual void OnDrag(PointerEventData data)
    {
        if (m_Joystick)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_Zone.rectTransform,
                data.position,
                data.pressEventCamera,
                out m_Position
                )
            )
            {
                //m_Position = m_Position - m_Zone.rectTransform.sizeDelta / 2;
                m_Position = m_Position;

                m_Joystick.rectTransform.anchoredPosition = (m_Position.magnitude < m_MaxZone && m_Position.magnitude > m_DeadZone) ? m_Position : m_Position.normalized * m_MaxZone;

                //m_Joystick.rectTransform.anchoredPosition = m_Position - m_Zone.rectTransform.sizeDelta/2;
            }
        }
    }

    public Vector2 GetInput()
    {
        Vector2 _vector = m_Joystick.rectTransform.anchoredPosition.normalized;

        if (m_InvertX)
            _vector.x = -_vector.x;
        if (m_InvertX)
            _vector.y = -_vector.y;

        return _vector;
    }

    public float GetMagnitude()
    {
        return m_Position.magnitude;
    }

    public Vector2 Position()
    {
        return m_Position;
    }

    public float GetAngle()
    {
        if (m_Position.x < 0)
            return 360 - (Mathf.Atan2(m_Position.x, m_Position.y) * Mathf.Rad2Deg * -1);

        return Mathf.Atan2(m_Position.x, m_Position.y) * Mathf.Rad2Deg;
    }

}
