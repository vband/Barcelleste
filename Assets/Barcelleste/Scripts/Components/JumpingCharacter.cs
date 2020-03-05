using System.Collections.Generic;
using UnityEngine;

namespace Barcelleste
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class JumpingCharacter : MonoBehaviour
    {
        [Tooltip("This character's initial vertical velocity in units per second at the moment she jumps.")]
        [SerializeField] private float jumpVelocity = 0;
        [Tooltip("A gravity modifier applied to the character when she is falling. It represents how many times gravity should be increased for her during her falls. Leave at zero if you don't want this effect.")]
        [SerializeField] private float fallMultiplier = 0;
        [Tooltip("A gravity modifier applied to the character when she is performing a low jump. If this is the player character, it happens when the player taps briefly the jump button. It represents how many times gravity should be increased for her during low jumps. Leave at zero if you don't want this effect.")]
        [SerializeField] private float lowJumpMultiplier = 0;

        /// <summary>
        /// Represents this character's intention to jump. You should update this every frame. True if she wants to jump, False otherwise.
        /// </summary>
        public bool JumpIntention { get; set; } = false;

        public bool IsGrounded { get => isGrounded; }

        // Events
        public delegate void Jumped();
        public event Jumped jumped;
        public delegate void StartedFalling();
        public event StartedFalling startedFalling;
        public delegate void Landed();
        public event Landed landed;

        private new Rigidbody2D rigidbody;
        private new Collider2D collider;
        private List<Contact> contacts = new List<Contact>();
        private bool isGrounded = false;
        private float verticalVelocityAtTheLastUpdate = 0;
        private bool wasGroundedAtTheLastUpdate = false;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            RaiseLandedEvent();
            RaiseStartedFallingEvent();
            UpdateLastTimePhysicsMetrics();
        }

        private void FixedUpdate()
        {
            PerformGroundCheck();
            ApplyFallMultiplier();
            RaiseJumpedEvent();
        }

        private void UpdateLastTimePhysicsMetrics()
        {
            verticalVelocityAtTheLastUpdate = rigidbody.velocity.y;
            wasGroundedAtTheLastUpdate = isGrounded;
        }

        private void RaiseJumpedEvent()
        {
            if (JumpIntention && isGrounded)
            {
                Jump();
                jumped.Invoke();
            }
        }

        private void RaiseStartedFallingEvent()
        {
            if (verticalVelocityAtTheLastUpdate >= 0 && rigidbody.velocity.y < 0)
            {
                startedFalling.Invoke();
            }
        }

        private void RaiseLandedEvent()
        {
            if (!wasGroundedAtTheLastUpdate && isGrounded)
            {
                landed.Invoke();
            }
        }

        private void Jump()
        {
            rigidbody.velocity = new Vector2
            {
                x = rigidbody.velocity.x,
                y = jumpVelocity
            };
        }

        private void ApplyFallMultiplier()
        {
            if (rigidbody.velocity.y < 0 && !isGrounded)
                rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - rigidbody.gravityScale) * Time.fixedDeltaTime;
            else if (!JumpIntention && !isGrounded)
                rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - rigidbody.gravityScale) * Time.fixedDeltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                contacts.Add(new Contact(collision.collider));
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            contacts.RemoveAll(contact => contact.Collider == collision.collider);
        }

        private void PerformGroundCheck()
        {
            isGrounded = contacts.Exists(contact => contact.Collider.bounds.max.y < collider.bounds.min.y);
        }

        private class Contact
        {
            public Collider2D Collider { get; }

            public Contact(Collider2D col)
            {
                Collider = col;
            }
        }
    }
}
