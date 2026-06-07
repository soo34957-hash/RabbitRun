using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private Vector3 startPosition;
    private bool isMovingRight = false;

    [Header("Animation Controller")]
    public RuntimeAnimatorController idleController;
    public RuntimeAnimatorController jumpController;
    public RuntimeAnimatorController runController;
    public RuntimeAnimatorController crouchController;

    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = idleController;
    }

    void Update()
    {
        Vector2 moveDirection = Vector2.zero;

        // 1. 입력 감지 (이동 방향 세팅)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x -= 1f;
            isMovingRight = false; // 왼쪽 이동
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x += 1f;
            isMovingRight = true; // 오른쪽 이동
        }

        // 2. 점프 입력 및 로직 처리
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartJump();
        }

        if (isJumping)
        {
            UpdateJump();
        }
        else
        {
            // 점프 중이 아닐 때만 애니메이션 상태 변경
            if (Input.GetKey(KeyCode.DownArrow))
            {
                // 아래 방향키를 누르고 있으면 엎드리기
                if (animator.runtimeAnimatorController != crouchController)
                    animator.runtimeAnimatorController = crouchController;
            }
            else if (moveDirection.x != 0f)
            {
                // 좌우 이동 키가 눌려있다면 걷기(달리기) 애니메이션 적용
                if (animator.runtimeAnimatorController != runController)
                    animator.runtimeAnimatorController = runController;
            }
            else
            {
                // 아무것도 안 누르면 가만히 있기
                if (animator.runtimeAnimatorController != idleController)
                    animator.runtimeAnimatorController = idleController;
            }
        }

        // 3. 방향키에 맞춰 스프라이트 뒤집기
        if (moveDirection.x > 0f)
        {
            spriteRenderer.flipX = false; // 오른쪽 바라봄
        }
        else if (moveDirection.x < 0f)
        {
            spriteRenderer.flipX = true; // 왼쪽 바라봄
        }

        // 4. 실제 물리 이동 처리
        moveDirection = moveDirection.normalized;

    
        if (isJumping)
        {
            // 점프 중일 때는 X축 위치만 이동 방향에 맞춰 직접 더해줍니다.
            startPosition.x += moveDirection.x * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 평지일 때는 기존대로 이동
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void StartJump()
    {
        isJumping = true;
        jumpTimer = 0f;
      
        startPosition = transform.position;

        animator.runtimeAnimatorController = jumpController;
    }

    void UpdateJump()
    {
        jumpTimer += Time.deltaTime;           
        float progress = jumpTimer / jumpDuration;

        if (progress >= 1f)
        {
            // 점프 종료 시, 누적된 startPosition의 x를 반영하여 착지시킵니다.
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
            isJumping = false;

            // 점프가 끝났을 때 멈춰있는지 움직이는지 체크하여 애니메이션 전환
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                animator.runtimeAnimatorController = runController;
            else
                animator.runtimeAnimatorController = idleController;
        }
        else
        {
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
       
            transform.position = new Vector3(startPosition.x, startPosition.y + height, transform.position.z);
        }
    }
}