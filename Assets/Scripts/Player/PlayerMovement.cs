using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb = null;
    [SerializeField] private Animator animator = null;

    private Vector2 movement;

    private void Start()
    {
        StartCoroutine("FootstepSound");
    }

    private IEnumerator FootstepSound()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (movement.sqrMagnitude > 0.01)
            {
                Sound sound = SoundManager.GetSound("footstep2");

                SoundManager.OneShotSound(sound.clip, gameObject.transform.position, sound.volume, 
                    Random.Range(sound.pitch - 0.5f, sound.pitch + 0.5f), sound.stereoPan, sound.spatialBlend, sound.priority);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void Update()
    {
        if (OpenWorldManager.GameStatusIsOpenWorld() && Time.timeScale > 0)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            movement.x = 0;
            movement.y = 0;
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        if (OpenWorldManager.GameStatusIsOpenWorld()) 
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
