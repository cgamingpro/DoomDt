using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHelapth : MonoBehaviour
{
    public float maxhealth = 100f;
    public float currentHealth;
    private bool isDead = false;
    [Tooltip("lenth of animation")]
    [SerializeField] float dealay = 1f;
    [SerializeField] int sceneIndex;
    public  int medPackNumber = 3;
    [SerializeField] int maxMedpackNumber = 5;
    public  float healthPerMedpack = 40;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && medPackNumber != 0)
        {
            HealPlayer();
        }
    }
    void HealPlayer()
    {
        currentHealth += healthPerMedpack;
        if (currentHealth > maxhealth)
        {
            currentHealth = maxhealth;
        }
        medPackNumber--;
    }
}
