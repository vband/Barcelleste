using UnityEngine;

namespace Barcelleste
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingCharacter : MonoBehaviour
    {
        [Tooltip("Max velocity in units per second.")]
        [SerializeField] private float moveVelocity = 0;
        [Tooltip("Velocity curve for acceleration phase. Also used for deceleration phase.")]
        [SerializeField] private AnimationCurve accelerationCurve = new AnimationCurve();
        [Tooltip("The time in seconds this character spends in acceleration phase.")]
        [SerializeField] private float accelerationTime = 0f;
        [Tooltip("The time in seconds this character spends in deceleration phase.")]
        [SerializeField] private float decelerationTime = 0f;
        [Tooltip("The time in seconds it takes this character to completely reverse the direction of her movement.")]
        [SerializeField] private float reverseDirectionTime = 0f;

        private float moveIntention = 0;
        /// <summary>
        /// Represents this character's intention to move. You should update this every frame. Positive if she wants to move to the right, negative if she wants to move to the left. Zero if there is no intention to move.
        /// </summary>
        public float MoveIntention { get { return moveIntention; } set { moveIntention = Mathf.Clamp(value, -1, 1); } }

        // Events
        public delegate void StartedMoving();
        public event StartedMoving startedMoving;
        public delegate void StoppedMoving();
        public event StoppedMoving stoppedMoving;
        

        private new Rigidbody2D rigidbody;
        private float t = 0;
        private bool isTryingToReverseMovementDirection = false;
        private float moveIntentionAtTheLastUpdate = 0;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckIfTryingToReverseMovementDirection();
            RaiseEvents();
        }

        private void RaiseEvents()
        {
            RaiseStartedMovingEvent();
            RaiseStoppedMovingEvent();
        }

        private void RaiseStoppedMovingEvent()
        {
            if (moveIntentionAtTheLastUpdate != 0 && MoveIntention == 0)
            {
                stoppedMoving.Invoke();
            }
        }

        private void RaiseStartedMovingEvent()
        {
            if (moveIntentionAtTheLastUpdate == 0 && MoveIntention != 0)
            {
                startedMoving.Invoke();
            }
        }

        private void FixedUpdate()
        {
            Run();
        }

        private void LateUpdate()
        {
            UpdateLastTimeMetrics();
        }

        private void UpdateLastTimeMetrics()
        {
            moveIntentionAtTheLastUpdate = MoveIntention;
        }

        private void CheckIfTryingToReverseMovementDirection()
        {
            float horizontalVelocity = rigidbody.velocity.x;

            if (horizontalVelocity != 0 && Mathf.Sign(MoveIntention) != Mathf.Sign(horizontalVelocity))
            {
                isTryingToReverseMovementDirection = true;
            }

            else if (Mathf.Abs(horizontalVelocity) <= 0.1f)
            {
                isTryingToReverseMovementDirection = false;
            }
        }

        private void Run()
        {
            // Accelerating
            if (MoveIntention != 0f)
            {
                if (!isTryingToReverseMovementDirection)
                {
                    t += Time.fixedDeltaTime / accelerationTime;
                    Accelerate(Mathf.Clamp01(t));
                }
                else
                {
                    t -= Time.fixedDeltaTime / reverseDirectionTime;
                    Decelerate(t = Mathf.Clamp01(t));
                }
            }

            // Decelerating
            else if (MoveIntention == 0f)
            {
                t -= Time.fixedDeltaTime / decelerationTime;
                Decelerate(t = Mathf.Clamp01(t));
            }
        }

        private void Accelerate(float t)
        {
            var speed = accelerationCurve.Evaluate(t) * moveVelocity;
            rigidbody.velocity = new Vector2(speed * MoveIntention, rigidbody.velocity.y);
        }

        private void Decelerate(float t)
        {
            var speed = accelerationCurve.Evaluate(t) * moveVelocity;
            rigidbody.velocity = new Vector2(speed * Mathf.Sign(rigidbody.velocity.x), rigidbody.velocity.y);
        }
    }
}
