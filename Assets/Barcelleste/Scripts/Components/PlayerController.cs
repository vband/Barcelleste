using UnityEngine;
using UnityEngine.InputSystem;

namespace Barcelleste
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(JumpingCharacter))]
    [RequireComponent(typeof(MovingCharacter))]
    public class PlayerController : MonoBehaviour
    {
        [Tooltip("Should the player be allowed to jump constantly without letting go of the jump button?")]
        [SerializeField] private bool allowConstantJump = false;

        private new Rigidbody2D rigidbody;
        private JumpingCharacter jumpScript;
        private MovingCharacter moveScript;
        private PlayerInput inputActions;
        private float moveInput = 0;
        private float jumpInput = 0;

        private bool hasJumped = false;
        private bool hasLanded = false;
        private bool hasStartedFalling = false;
        private bool hasLetGoOfJumpButtonAfterLanding = false;
        private bool hasLandedWithoutPressingJumpButton = false;

        private void Awake()
        {
            Initialize();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            inputActions.Gameplay.Move.performed += OnMoveActionPerformed;
            inputActions.Gameplay.Move.canceled += OnMoveActionCanceled;
            inputActions.Gameplay.Jump.performed += OnJumpActionPerformed;
            inputActions.Gameplay.Jump.canceled += OnJumpActionCanceled;

            jumpScript.jumped += OnCharacterJumped;
            jumpScript.landed += OnCharacterLanded;
            jumpScript.startedFalling += OnCharacterStartedFalling;
        }

        private void Initialize()
        {
            inputActions = new PlayerInput();
            rigidbody = GetComponent<Rigidbody2D>();
            jumpScript = GetComponent<JumpingCharacter>();
            moveScript = GetComponent<MovingCharacter>();
        }

        private void Update()
        {
            TransmitMoveIntention();
            TransmitJumpIntention();
        }

        private void TransmitMoveIntention()
        {
            moveScript.MoveIntention = moveInput;
        }

        private void TransmitJumpIntention()
        {
            if (allowConstantJump)
            {
                jumpScript.JumpIntention = jumpInput == 1 ? true : false;
            }
            else
            {
                jumpScript.JumpIntention = jumpInput == 1 && CanJump();
            }
        }

        private bool CanJump()
        {
            return hasLetGoOfJumpButtonAfterLanding || hasLandedWithoutPressingJumpButton || (hasJumped && !hasStartedFalling);
        }

        private void OnCharacterJumped()
        {
            hasJumped = true;
            hasLanded = false;
            hasStartedFalling = false;
            hasLetGoOfJumpButtonAfterLanding = false;
            hasLandedWithoutPressingJumpButton = false;
        }

        private void OnCharacterLanded()
        {
            hasJumped = false;
            hasLanded = true;
            hasStartedFalling = false;
            if (jumpInput == 0)
                hasLandedWithoutPressingJumpButton = true;
        }

        private void OnCharacterStartedFalling()
        {
            hasStartedFalling = true;
            hasJumped = false;
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext callbackContext)
        {
            moveInput = callbackContext.ReadValue<float>();
        }

        private void OnMoveActionCanceled(InputAction.CallbackContext callbackContext)
        {
            moveInput = callbackContext.ReadValue<float>();
        }

        private void OnJumpActionPerformed(InputAction.CallbackContext callbackContext)
        {
            jumpInput = callbackContext.ReadValue<float>();
        }

        private void OnJumpActionCanceled(InputAction.CallbackContext callbackContext)
        {
            jumpInput = callbackContext.ReadValue<float>();

            if (hasLanded)
                hasLetGoOfJumpButtonAfterLanding = true;
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }
    }
}
