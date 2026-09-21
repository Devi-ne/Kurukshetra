using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    public float speed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Handle Movement Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        animator.SetFloat("VelX", horizontal);
        animator.SetFloat("VelZ", vertical);

        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        if (movement.magnitude > 1f) {
            movement.Normalize();
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.Self);

        // 2. Handle Sword Attack (Left Click)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack"); 
        }

        // 3. Handle Bow Aiming (Hold Right Click)
        // GetMouseButton(1) returns true as long as the button is held down
        bool aiming = Input.GetMouseButton(1);
        animator.SetBool("IsAiming", aiming);
    }
}