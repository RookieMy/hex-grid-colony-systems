using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float dragSpeed = 2f;
    public float edgeSize = 10f;
    private Vector3 dragOrigin;
    private float targetZoom;

    public Vector2 minBounds; // (minX, minZ)
    public Vector2 maxBounds; // (maxX, maxZ)

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom=cam.orthographicSize;
    }

    void Update()
    {
        // Camera movement
        HandleMovement();
        // Camera zoom
        HandleZoom();
        HandleMouseDrag();
        HandleEdgeMovement();
        ClampPosition();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.right * moveX + transform.forward * moveZ);
        moveDir.y = 0f;

        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }


    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 5f);
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            Vector3 moveDir = (transform.right * -pos.x + transform.forward * -pos.y);
            moveDir.y = 0f;

            transform.position += moveDir * dragSpeed;
        }
    }


    void HandleEdgeMovement()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.mousePosition.x < edgeSize) moveDir -= transform.right;
        if (Input.mousePosition.x > Screen.width - edgeSize) moveDir += transform.right;
        if (Input.mousePosition.y < edgeSize) moveDir -= transform.forward;
        if (Input.mousePosition.y > Screen.height - edgeSize) moveDir += transform.forward;

        moveDir.y = 0f;
        if (moveDir != Vector3.zero)
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);
        transform.position = pos;
    }
}
