using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHelapth : MonoBehaviour
{
    [SerializeField] float maxhealth = 100f;
    public float currentHealth;
    private bool isDead = false;
    [Tooltip("lenth of animation")]
    [SerializeField] float dealay = 1f;
    [SerializeField] int sceneIndex;


    private void Start()
    {
        currentHealth = maxhealth;
    }
    public void ApplyDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        SceneManager.LoadScene(sceneIndex);

    }
}
