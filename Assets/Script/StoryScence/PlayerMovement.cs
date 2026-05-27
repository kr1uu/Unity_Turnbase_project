using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    [HideInInspector]
    public bool canMove = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;

            animator.SetBool(
                "isRunning",
                false
            );

            return;
        }

        movement = Vector2.zero;

        if (Keyboard.current != null)
        {
            movement.x =
                Keyboard.current.aKey.isPressed ? -1 :
                Keyboard.current.dKey.isPressed ? 1 : 0;

            movement.y =
                Keyboard.current.sKey.isPressed ? -1 :
                Keyboard.current.wKey.isPressed ? 1 : 0;
        }

        bool isMoving =
            movement.sqrMagnitude > 0.01f;

        animator.SetBool(
            "isRunning",
            isMoving
        );

        animator.SetFloat(
            "inputx",
            movement.x
        );

        animator.SetFloat(
            "inputy",
            movement.y
        );

        if (isMoving)
        {
            animator.SetFloat(
                "LastInputX",
                movement.x
            );

            animator.SetFloat(
                "LastInputY",
                movement.y
            );
        }
    }
    void FixedUpdate()
    {
        if (!canMove) return;                
        if (movement == Vector2.zero) return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            var encounter = collision.gameObject.GetComponent<BattleTrigger>();
            if (encounter != null && BattleEncounterData.Instance != null)
            {
                // Truy?n c? danh sách ID và danh sách rank
                BattleEncounterData.Instance.SetEnemies(encounter.selectedEnemyIDs, encounter.enemyRanks);
            }

            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene("BattleScene");
            }
            else
            {
                Debug.LogWarning("SceneFader chua setup trong scene!");
            }
        }
        if (collision.collider.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero; // D?ng l?i
            Debug.Log("Wall get impact");
        }

    }
    public void SetControl(bool value)
    {
        canMove = value;

        if (!value)
        {
            movement = Vector2.zero;

            animator.SetBool(
                "isRunning",
                false
            );
        }
    }
    //public void Move(InputAction.CallbackContext context)
    //{
    //    animator.SetBool("isRunning",true);

    //    if (context.canceled)
    //    {
    //        animator.SetBool("isRunning", false);
    //        animator.SetFloat("LastinPutx", movement.x);
    //        animator.SetFloat("LastinPuty", movement.y);
    //    }
    //    movement = context.ReadValue<Vector2>();
    //    animator.SetFloat("inPutx", movement.x);
    //    animator.SetFloat("inPuty", movement.y);
    //}
}