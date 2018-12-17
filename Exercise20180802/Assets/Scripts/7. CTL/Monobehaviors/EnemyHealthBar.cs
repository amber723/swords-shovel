using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    Vector3 localScale;
    [SerializeField] private SpriteRenderer healthBar;
    CharacterStats enemyStats;

    void Awake()
    {
        enemyStats = transform.parent.GetComponent<CharacterStats>();
    }

    void Start()
    {
        localScale = healthBar.transform.localScale;
    }

    void FixedUpdate()
    {
        if (enemyStats != null)
        {
            localScale.x = (float) enemyStats.characterDefinition.currentHealth /
                enemyStats.characterDefinition.maxHealth;

            healthBar.transform.localScale = localScale;
        }

        //ensure the healthBar is always facing the camera.
        transform.LookAt(Camera.main.transform);
    }
}
