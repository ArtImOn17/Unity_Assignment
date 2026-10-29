using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebug : MonoBehaviour
{
    void OnJump() { Debug.Log("Jump"); }
    void OnMove(InputValue value) { Debug.Log(value.Get<Vector2>()); }
}
