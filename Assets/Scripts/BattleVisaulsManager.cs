using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BattleVisaulsManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;

    private int currHealth;
    private int maxHealth;
    private int level;

    private Animator animator;


    private const string LEVEL_ABB = "LVL: ";
    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";


    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Missing Animator component on the GameObject.");
        }
      
        
    }

   public void SetStartingValues(int currHealth, int maxHealth, int level)
    {
        this.currHealth = currHealth;
        this.maxHealth = maxHealth; 
        this.level = level;
        if (levelText == null)
        {
            Debug.LogError("LevelText is not assigned in the inspector!");
            return;
        }
        levelText.text =  LEVEL_ABB + this.level.ToString();
        UpdateHealthBar();
    }

    public void ChangeHealthBar(int currHealth)
    {
        this.currHealth = currHealth;

        if (currHealth <= 0)
        {
            PlayDeadAnimation();
            Destroy(gameObject, 1f);
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar slider is not assigned in the inspector!");
            return;
        }

        healthBar.maxValue = maxHealth;
        healthBar.value = currHealth;
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(IS_ATTACK_PARAM);
    }
    public void PlayHitAnimation()
    {
        animator.SetTrigger(IS_HIT_PARAM);
    }
    public void PlayDeadAnimation()
    {
        animator.SetTrigger(IS_DEAD_PARAM);
    }
}
