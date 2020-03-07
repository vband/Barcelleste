using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Barcelleste
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Killer : MonoBehaviour
    {
        [SerializeField] private float verticalImpulseVelocity = 0;
        [SerializeField] private float knockbackVelocity = 0;
        [SerializeField] private float stunDuration = 0;

        private new Rigidbody2D rigidbody;
        private new Collider2D collider;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var killable = collision.gameObject.GetComponent<Killable>();
            if (killable)
            {
                if (IsBelow(collision))
                {
                    killable.Die();
                    ApplyVerticalImpulse();
                }
                else
                {
                    ApplyKnockback(collision);
                }
            }
        }

        private bool IsBelow(Collision2D collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).point.y < collider.bounds.min.y)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyVerticalImpulse()
        {
            rigidbody.velocity = new Vector2
            {
                x = rigidbody.velocity.x,
                y = verticalImpulseVelocity
            };
        }

        private void ApplyKnockback(Collision2D collision)
        {
            var direction = (transform.position - collision.transform.position).normalized;
            rigidbody.velocity = direction * knockbackVelocity;
            StartCoroutine(DisableMovementFor(seconds: stunDuration));
        }

        private IEnumerator DisableMovementFor(float seconds)
        {
            var moveScript = GetComponent<MovingCharacter>();
            var jumpScript = GetComponent<JumpingCharacter>();

            if (moveScript) moveScript.enabled = false;
            if (jumpScript) jumpScript.enabled = false;
            yield return new WaitForSeconds(seconds);
            if (moveScript) moveScript.enabled = true;
            if (jumpScript) jumpScript.enabled = true;
        }
    }
}
