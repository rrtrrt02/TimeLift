using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;        // 이동 속도
    public float jumpForce = 16f;       // 점프 힘
    public float jumpCutMultiplier = 0.5f; // 점프 키를 뗐을 때 속도 감소 비율 (낮은 점프 구현)

    [Header("Ground Detection")]
    public Transform groundCheck;       // 발바닥 위치
    public float groundCheckRadius = 0.2f; // 감지 반경
    public LayerMask groundLayer;       // 땅 레이어

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 입력 받기 (Input.GetAxisRaw는 관성 없이 즉시 0, 1, -1을 반환함)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프 입력 (Space 키)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 3. 가변 점프 (키를 살짝 누르면 낮게 점프)
        // 상승 중일 때 점프 키를 떼면 y축 속도를 줄임
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // 4. 캐릭터 방향 뒤집기
        Flip();
    }

    void FixedUpdate()
    {
        // 5. 물리 이동 (좌우 이동)
        // AddForce 대신 velocity를 직접 수정하여 즉각적인 반응(Snappy) 구현
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // 6. 땅 감지 (Ground Layer와 겹치는지 확인)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Jump()
    {
        // y축 속도를 0으로 초기화 후 힘을 가함 (일관된 점프 높이 보장)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    
    // 에디터에서 땅 감지 범위 시각화
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}