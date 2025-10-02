using UnityEngine;
using UnityEngine.InputSystem;

namespace GuidanceLine
{
    /// <summary>
    /// Simple Player controller for testing the GuidanceLine asset
    /// Uses direct Input System calls - no PlayerInput component or InputActions asset needed!
    /// Perfect for quick testing before AR adaptation.
    /// </summary>
    public class PlayerSimple : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 5f;
        public float gravity = -9.81f;
        public float jumpForce = 5f;

        [Header("Look Settings")]
        public float mouseSensitivity = 0.1f;
        public float pitchLimit = 80f;

        private float yVelocity = 0f;
        private bool isGrounded;

        private CharacterController controller;
        private Transform playerCamera;
        private float xRotation = 0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            playerCamera = Camera.main.transform;
            
            // Lock cursor to center of screen for better mouse look
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Debug.Log("PlayerSimple: Using direct Input System API - no InputActions asset required!");
        }

        void Update()
        {
            // Handle cursor unlock with Escape
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            isGrounded = controller.isGrounded;

            // Get movement input directly from Keyboard
            Vector2 moveInput = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
                if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
                if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
                if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
            }

            // Movement
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

            if (isGrounded)
            {
                yVelocity = -0.5f;
            }

            yVelocity += gravity * Time.deltaTime;

            // Jump
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            {
                yVelocity = jumpForce;
            }

            Vector3 velocity = move * speed;
            velocity.y = yVelocity;
            controller.Move(velocity * Time.deltaTime);

            // Mouse look - only when cursor is locked
            if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                
                float mouseX = mouseDelta.x * mouseSensitivity;
                float mouseY = mouseDelta.y * mouseSensitivity;

                transform.Rotate(Vector3.up * mouseX);

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -pitchLimit, pitchLimit);
                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            // Re-lock cursor when returning to the game
            if (hasFocus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
