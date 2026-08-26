using UnityEngine;
using UnityEngine.InputSystem;

public class Player1Controller : MonoBehaviour
{
    public float speed = 5.0f;
    void FixedUpdate()
    {
        float x = 0f;
        float y = 0f;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.wKey.isPressed) y = 1f;
        if (keyboard.sKey.isPressed) y = -1f;
        if (keyboard.aKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed) x = 1f;

        Vector3 move = new Vector3(x, y, 0f).normalized;
        transform.Translate(move * speed * Time.deltaTime, Space.World);

    }
}