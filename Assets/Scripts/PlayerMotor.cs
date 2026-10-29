using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public float acceleration = 10.0f;
    public float jumpForce = 10.0f;
    private Vector2 direction;
    private Rigidbody2D rigidbody2D;

    void Start() { rigidbody2D = GetComponent<Rigidbody2D>(); }
    private void FixedUpdate()
    {
        rigidbody2D.AddForce(new Vector2(direction.x, 0) * acceleration);
    }
    void OnMove(InputValue value) { direction = value.Get<Vector2>(); }
    void OnJump()
    {
        rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
