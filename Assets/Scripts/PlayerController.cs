using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private Vector2 moveDir;
    [SerializeField] private float speed;


    [Header("Camera")]
    [SerializeField] private Transform cam;
    [SerializeField] private float sensitivity;
    private Vector2 mouseDelta;   

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float rotY = -mouseDelta.y * sensitivity * Time.deltaTime;
        float rotX = mouseDelta.x * sensitivity * Time.deltaTime;

        if(!Input.GetKey(KeyCode.LeftShift))
        {
            cam.Rotate(rotY, 0, 0);
            transform.Rotate(0, rotX, 0);
        }

        Vector3 moveVector = new Vector3(moveDir.x, 0, moveDir.y);

        moveVector = transform.TransformDirection(moveVector);

        rb.linearVelocity = speed * moveVector + (rb.linearVelocity.y * Vector3.up);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }
}