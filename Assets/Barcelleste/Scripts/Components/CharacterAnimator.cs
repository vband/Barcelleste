using UnityEngine;

namespace Barcelleste
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MovingCharacter))]
    [RequireComponent(typeof(JumpingCharacter))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator animator;
        private new Rigidbody2D rigidbody;
        private MovingCharacter moveScript;
        private JumpingCharacter jumpScript;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            Initialize();
            SubscribeToEvents();
        }

        private void Initialize()
        {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody2D>();
            moveScript = GetComponent<MovingCharacter>();
            jumpScript = GetComponent<JumpingCharacter>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void SubscribeToEvents()
        {
            jumpScript.jumped += OnCharacterJumped;
            jumpScript.startedFalling += OnCharacterStartedFalling;
            jumpScript.landed += OnCharacterLanded;

            moveScript.startedMoving += OnCharacterStartedMoving;
            moveScript.stoppedMoving += OnCharacterStoppedMoving;
        }

        private void Update()
        {
            FlipSprite();
        }

        private void OnCharacterJumped()
        {
            animator.SetTrigger(Constants.ANIMATOR_PARAMETER_TRIGGER_JUMPED);
        }

        private void OnCharacterStartedFalling()
        {
            animator.SetBool(Constants.ANIMATOR_PARAMETER_BOOL_IS_FALLING, true);
        }

        private void OnCharacterLanded()
        {
            animator.SetTrigger(Constants.ANIMATOR_PARAMETER_TRIGGER_LANDED);
            animator.SetBool(Constants.ANIMATOR_PARAMETER_BOOL_IS_FALLING, false);
        }

        private void OnCharacterStartedMoving()
        {
            animator.SetBool(Constants.ANIMATOR_PARAMETER_BOOL_IS_MOVING, true);
        }

        private void OnCharacterStoppedMoving()
        {
            animator.SetBool(Constants.ANIMATOR_PARAMETER_BOOL_IS_MOVING, false);
        }

        private void FlipSprite()
        {
            if (moveScript.MoveIntention > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveScript.MoveIntention < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
