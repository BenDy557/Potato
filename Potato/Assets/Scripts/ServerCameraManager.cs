using UnityEngine;

public class ServerCameraManager : MonoBehaviour
{
    [SerializeField] private SocketComm serverSocket = null;
    [SerializeField] private Transform leftMargin = null;
    [SerializeField] private Transform rightMargin = null;

    private Camera camera = null;
    private Transform cameraTransform = null;

    private Vector3 lerpPosition;
    private float lerpViewportSize;

    private Transform myTransform;

    private void Start()
    {
        camera = Camera.main;
        cameraTransform = camera.GetComponent<Transform>();

        myTransform = GetComponent<Transform>();

        lerpViewportSize = camera.orthographicSize;
        lerpPosition = cameraTransform.position;
    }

    private void Update()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, lerpPosition, Time.deltaTime);
        camera.orthographicSize = FloatLerp(camera.orthographicSize, lerpViewportSize, Time.deltaTime);
    }

    private float FloatLerp(float from, float to, float time)
    {
        return (from + (to - from) * time);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        UpdateCameraViewport();
        UpdateCameraLimits();
    }

    private void UpdateCameraViewport()
    {
        lerpViewportSize *= 2;
    }
    private void UpdateCameraLimits()
    {
        // Save Info Before Modifying
        Vector3 cameraPos = cameraTransform.position;
        float size = camera.orthographicSize;

        cameraTransform.position = lerpPosition;
        camera.orthographicSize = lerpViewportSize;

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(new Vector3(0,0,0));
        if (Physics.Raycast(ray, out hit, 1000000))
        {
            float colliderOffset = 0.5f;

            serverSocket.UpdateSpawnLimits(hit.point.x, -hit.point.x);
            lerpPosition.y += -hit.point.y;
            myTransform.position += new Vector3(0, -hit.point.y * 2 + colliderOffset * 2, 0);

            leftMargin.position = new Vector3(hit.point.x - colliderOffset, 500000, 0);
            rightMargin.position = new Vector3(-hit.point.x + colliderOffset , 500000, 0);
        }

        cameraTransform.position = cameraPos;
        camera.orthographicSize = size;
    }
}
