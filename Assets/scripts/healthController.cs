using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class healthController : MonoBehaviour
{
    [SerializeField] float maxhealth = 100f;
    public float currentHealth;
    private bool isDead = false;
    [Tooltip("lenth of animation")]
    [SerializeField] float dealay = 1f;
    Animator animator;
    private void Start()
    {
        currentHealth = maxhealth;
        TryGetComponent<Animator>(out  animator);
    }
    public void ApplyDamage(float damage)
    {
        if(isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
            Destroy(gameObject, dealay);
        }
        Destroy(gameObject);

    }
}
