using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private float controllerSensetivity = 10f;
    void Awake()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        //     Vector2 position = transform.position;
        //     position.x+= Input.GetAxisRaw("Right Stick X") * sensetivity;
        //     position.y += Input.GetAxisRaw("Right Stick Y") * sensetivity;
        //     transform.position = position;
        transform.position = Input.mousePosition;
        // Vector3.Lerp(transform.position, Input.mousePosition, mouseSensetivity);
    }
}
