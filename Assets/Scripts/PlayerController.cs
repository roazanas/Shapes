using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration; 
    private Rigidbody2D body;
    [HideInInspector] public bool isDashing = false; 
    private float dashTimeLeft;

    private PlayerHDController playerHDController;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerHDController = GetComponent<PlayerHDController>();
    }

    void Update()
    {
        if (playerHDController.isShocked) { return; }
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = (Vector2)transform.position;
        Vector2 direction = (mousePosition - playerPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        body.rotation = angle-90;

        if (Input.GetButtonDown("Fire1") && !isDashing) 
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime; 

            if (dashTimeLeft <= 0) 
            {
                EndDash();
            }
        }
    }

    Vector2 direction;

    private void StartDash()
    {
        if (isDashing) return;

        isDashing = true;
        dashTimeLeft = dashDuration;

        direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        body.velocity = Vector2.zero;
        body.AddForce(direction * dashSpeed, ForceMode2D.Impulse);
    }

    private void EndDash()
    {
        isDashing = false;
        body.totalForce = new Vector2(0.0f, 0.0f);
    }
}
