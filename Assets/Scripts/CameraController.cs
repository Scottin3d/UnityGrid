using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 zoomLimits;
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraSwiftSpeed;
    private float camSpeed;

    public Transform mainCamera;
    Transform zoomObject;
    Vector2 input;
    bool mouseMove = false;
    Vector3 moveInput;

    void Awake()
    {
        //mainCamera = Camera.main.transform;
        transform.LookAt(mainCamera);
        zoomObject = transform.GetChild(0);
        camSpeed = cameraSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            camSpeed = cameraSwiftSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            camSpeed = cameraSpeed;
        }
        moveInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (mouseMove)
        {
            Vector2 mousePos = Input.mousePosition;

            if (mousePos.x > Screen.width * 0.95f && mousePos.x < Screen.width)
                moveInput.x = 1;
            else if (mousePos.x < Screen.width * 0.05f && mousePos.x > 0)
                moveInput.x = -1;

            if (mousePos.y > Screen.height * 0.95f && mousePos.y < Screen.height)
                moveInput.z = 1;
            else if (mousePos.y < Screen.height * 0.05f && mousePos.y > 0)
                moveInput.z = -1;
        }

        Vector3 movementDirection = mainCamera.TransformDirection(moveInput);
        movementDirection.y = 0;
        transform.position += movementDirection.normalized * Time.deltaTime * cameraSpeed;

        zoomObject.localPosition += new Vector3(0,0,-Input.mouseScrollDelta.y) * 10f;
        zoomObject.localPosition = new Vector3(zoomObject.localPosition.x, zoomObject.localPosition.y,Mathf.Clamp(zoomObject.localPosition.z, zoomLimits.x, zoomLimits.y));

    }
}
