using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Camera cam;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float lookSensitivity = 0.5f;

    private float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>().normalized;

        Vector3 movement = (transform.right * v.x + transform.forward * v.y) * speed;

        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    void OnJump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }

    void OnLook(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        transform.Rotate(Vector3.up * (v.x * lookSensitivity));
        xRotation -= (v.y * lookSensitivity);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}