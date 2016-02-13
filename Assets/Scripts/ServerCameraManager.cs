using UnityEngine;

public class ServerCameraManager : MonoBehaviour
{
    [SerializeField] private SQLComm sqlComm = null;

	private Camera cam = null;
    private Transform cameraTransform = null;

    private Vector3 lerpPosition;
    private float lerpViewportSize;

    private Transform myTransform;

    public static ServerCameraManager Instance; // lazy singleton

    private void Start()
    {
        Instance = this;

        cam = Camera.main;
        cameraTransform = cam.GetComponent<Transform>();

        myTransform = GetComponent<Transform>();

        lerpViewportSize = cam.orthographicSize;
        lerpPosition = cameraTransform.position;
    }

    private void Update()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, lerpPosition, Time.deltaTime);
        cam.orthographicSize = FloatLerp(cam.orthographicSize, lerpViewportSize, Time.deltaTime);
    }

    private float FloatLerp(float from, float to, float time)
    {
        return (from + (to - from) * time);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.position.y + 1f > myTransform.position.y)
            return;

        UpdateCameraViewport();
        UpdateCameraLimits();
    }

    public void ForceUpdate()
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
        float size = cam.orthographicSize;

        cameraTransform.position = lerpPosition;
        cam.orthographicSize = lerpViewportSize;

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(0,0,0));
        if (Physics.Raycast(ray, out hit, 1000000))
        {
            float colliderOffset = 0;

            sqlComm.UpdateSpawnLimits( lerpPosition.y * 4 + 2);
            lerpPosition.y += -hit.point.y;
            myTransform.position += new Vector3(0, -hit.point.y * 2 + colliderOffset, 0);

//            leftMargin.position = new Vector3(hit.point.x - colliderOffset, 500000, 0);
//            rightMargin.position = new Vector3(-hit.point.x + colliderOffset , 500000, 0);
        }

        cameraTransform.position = cameraPos;
        cam.orthographicSize = size;
    }
}
