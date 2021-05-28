using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        /* The "m_" in front of a variable is a coding convention that helps you seperate
     * Global and Local variables at a glance. It stands for "member variable" and
     * Unity automatically removes the "m_" in the Editor so that they don't look weird
     * in the Inspector!
     */

        // Handles
        [Header("Handles")]
        [SerializeField]
        private Camera m_Camera;
        [SerializeField]
        private CharacterController characterController;

        // Movement speeds
        [Header("Speed Variables")]
        [SerializeField]
        private float moveSpeed = 5.0f;
        [SerializeField]
        private float jumpForce = 5.0f;
        [SerializeField]
        private float gravityForce = 9.807f;

        // Look sensitivity variable
        [Range(0.0f, 5.0f)]
        public float lookSensitivity = 1.0f;

        [Header("Debugging Variables")]
        [SerializeField]
        private float mouseX;
        [SerializeField]
        private float mouseY;

        [SerializeField]
        private Vector3 moveDirection;

        private void Start()
        {
            characterController = characterController ? characterController : null;
            m_Camera = m_Camera ? m_Camera : Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Rotate();
            Movement();
        }

        private void Rotate()
        {
            // Receive mouse input and modifies
            mouseX += Input.GetAxisRaw("Mouse X") * lookSensitivity;
            mouseY += Input.GetAxisRaw("Mouse Y") * lookSensitivity;

            // Keep mouseY between -90 and +90
            mouseY = Mathf.Clamp(mouseY, -90.0f, 90.0f);

            // Rotate the player object around the y-axis
            transform.localRotation = Quaternion.Euler(Vector3.up * mouseX);
            // Rotate the camera around the x-axis
            m_Camera.transform.localRotation = Quaternion.Euler(Vector3.left * mouseY);
        }

        private void Movement()
        {
            var yVelocity = moveDirection.y;
            var forwardMovement = transform.forward * Input.GetAxisRaw("Vertical");
            var strafeMovement = transform.right * Input.GetAxisRaw("Horizontal");

            moveDirection = (forwardMovement + strafeMovement).normalized * moveSpeed;
            moveDirection.y = yVelocity;

            if (characterController.isGrounded)
            {
                moveDirection.y = 0f;

                if (Input.GetKey(KeyCode.Space)) 
                {
                    moveDirection.y = jumpForce;
                }
            } else {
                moveDirection.y -= gravityForce * Time.deltaTime;
                moveDirection.y = Mathf.Clamp(moveDirection.y, Physics.gravity.y, -Physics.gravity.y);
            }

            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
}
