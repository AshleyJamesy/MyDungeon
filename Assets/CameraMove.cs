using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    [SerializeField]
    float m_Speed = 1;

    [SerializeField]
    Joystick m_JoystickL;

    void Update() {
        transform.position += new Vector3(m_JoystickL.GetInput().x, m_JoystickL.GetInput().y, 0) * m_JoystickL.GetMagnitude() * m_Speed * Time.deltaTime;
    }
}
