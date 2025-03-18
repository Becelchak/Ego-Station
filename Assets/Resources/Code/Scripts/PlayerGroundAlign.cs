using UnityEngine;

public class PlayerGroundAlign : MonoBehaviour
{
    [SerializeField] private float alignSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (hit.collider != null)
        {
            float targetY = hit.point.y + GetComponent<Collider2D>().bounds.extents.y;
            float newY = Mathf.Lerp(transform.position.y, targetY, alignSpeed * Time.fixedDeltaTime);
            transform.position = new Vector2(transform.position.x, newY);
        }
    }
}