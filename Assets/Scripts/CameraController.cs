using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Collider2D playerCollider;
    private PlayerController playerController;
    private PlayerHDController playerHDController;

    [SerializeField] private float smoothDuration;
    [SerializeField] private float mouseImpact;
    [SerializeField] private Vector2 cameraOffset;

    private Camera cam;
    private float originalCameraSize;
    [SerializeField] private float zoomOutAmount;
    [SerializeField] private float zoomOutDuration;

    private float leftBoundary = -9.75f;
    private float rightBoundary = 9.75f;
    private float bottomBoundary = -4.75f;
    private float topBoundary = 4.75f;

    private Vector3 cachedCameraPos;
    private float cachedZoomLevel;

    private void Start()
    {
        if (player == null) { return; }
        playerCollider = player.GetComponent<Collider2D>();
        playerController = player.GetComponent<PlayerController>();
        playerHDController = player.GetComponent<PlayerHDController>();
        cam = Camera.main;
        originalCameraSize = cam.orthographicSize;

        cachedCameraPos = transform.position;
        cachedZoomLevel = cam.orthographicSize;

        DOTween.SetTweensCapacity(3500, 75);
    }

    private void Update()
    {
        if (playerCollider == null) { return; }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = (Vector2)player.transform.position +
            (mousePosition - (Vector2)player.transform.position) * mouseImpact +
            cameraOffset;

        Vector2 smoothedPosition = CalculateClampedPosition(targetPosition);

        Vector3 newPosition = Vector3.Lerp(cachedCameraPos, smoothedPosition, Time.deltaTime / smoothDuration);
        cachedCameraPos = newPosition;
        transform.position = cachedCameraPos;

        HandleCameraZoom();
    }

    private Vector2 CalculateClampedPosition(Vector2 targetPosition)
    {
        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;

        float clampX = Mathf.Clamp(targetPosition.x, leftBoundary + playerPosX, rightBoundary + playerPosX);
        float clampY = Mathf.Clamp(targetPosition.y, bottomBoundary + playerPosY, topBoundary + playerPosY);

        return new Vector2(clampX, clampY);
    }

    private void HandleCameraZoom()
    {
        float targetSize = originalCameraSize;

        if (playerController != null && playerController.isDashing)
        {
            targetSize = originalCameraSize + zoomOutAmount;
        }
        else if (playerController != null && playerHDController.isShocked)
        {
            targetSize = originalCameraSize + zoomOutAmount * 3f;
        }

        float newZoomLevel = Mathf.Lerp(cachedZoomLevel, targetSize, Time.deltaTime / zoomOutDuration);
        cachedZoomLevel = newZoomLevel;
        cam.orthographicSize = cachedZoomLevel;
    }
}
