using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float zoom;
    public float desiredZoom;
    private float rotationX;
    private float rotationY;

    private Vector3 desiredPosition;

    [SerializeField]
    private float mouseSensitivity;

    [SerializeField]
    private float interpolationSpeed;

    [SerializeField]
    private float cameraIntSpeed;

    [SerializeField]
    private float cameraSpeed;

    public static CameraController Instance { get; private set; }

    //private Vector3 startMousePos;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        zoom = 35f;
        Vector3 pos = transform.localPosition;
        pos.y = zoom;
        transform.localPosition = pos;

        desiredZoom = 17.5f;

        transform.parent.parent.position = new Vector3(50,0,50);

        desiredPosition = transform.parent.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
        Rotate();
        Move();
    }

    void Zoom()
    {
        if (!GameManager.Instance.paused)
        {
            desiredZoom -= Input.mouseScrollDelta.y;
            desiredZoom = Mathf.Clamp(desiredZoom, 1, 12);
        }

        zoom = Mathf.Lerp(zoom, 1 + Mathf.Pow(desiredZoom / 3f, 2), Time.unscaledDeltaTime * interpolationSpeed);

        Vector3 pos = transform.localPosition;
        pos.y = zoom;
        transform.localPosition = pos;
    }

    void Rotate()
    {
        if (GameManager.Instance.paused) return;
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationX += mouseX;
            rotationY -= mouseY;

            rotationY = Mathf.Clamp(rotationY, -65, -30);

            transform.parent.parent.localRotation = Quaternion.Euler(new Vector3(0, rotationX, 0));
            transform.parent.localRotation = Quaternion.Euler(new Vector3(rotationY, 0, 0));
        }
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Move()
    {
        if(!GameManager.Instance.paused)
        {
            float sSpeed = Mathf.Sqrt(zoom) * 2;

            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            move.Normalize();

            move = transform.parent.parent.rotation * move;

            desiredPosition += move * cameraSpeed * sSpeed * Time.deltaTime;
        }
        transform.parent.parent.position = Vector3.Lerp(transform.parent.parent.position, desiredPosition, Time.unscaledDeltaTime * cameraIntSpeed);
    }
}
