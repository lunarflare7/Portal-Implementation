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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        float rotY = -mouseDelta.y * sensitivity * Time.deltaTime;
        float rotX = mouseDelta.x * sensitivity * Time.deltaTime;

        cam.Rotate(rotY, 0, 0);
        transform.Rotate(0, rotX, 0);

        Vector3 moveVector = new Vector3(moveDir.x, 0, moveDir.y);

        moveVector = transform.worldToLocalMatrix.MultiplyVector(moveVector);

        rb.linearVelocity = speed * moveVector;
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