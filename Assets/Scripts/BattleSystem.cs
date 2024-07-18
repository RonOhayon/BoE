using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState {Start, Selection, Battle, Won, Lost, Run }

    [Header("battle State")]
    [SerializeField] private BattleState state;

    [Header("Spawm Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattler = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> HeroesBattlers = new List<BattleEntities>();


    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;


    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentHero;

    private const string ACTION_MESSAGE = "'s Action:";
    private const string WIN_MESSAGE = "The ememys are defeated you won";
    private const string LOST_MESSAGE = "You defeated";
    private const string SUCCESFULLY_RAN_AWAY = " Your party ran away";
    private const string UNSUCCESFULLY_RAN = " Your party failed to run away";
    private const string OVERWORLD_SCENE = "OverWorld";
    private const int RUN_CHANCE = 50;
    private const int TURN_DURATION = 2;
    

    void Start()
    {
       partyManager = GameObject.FindFirstObjectByType<PartyManager>();
       enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
      
    }
    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);                   // enemy selection menu disable
        state = BattleState.Battle;                            // change our state to the battle state
        bottomTextPopUp.SetActive(true);                       // enable our bottom popUP text                                   
        for(int i = 0; i < allBattler.Count; i++)              // loop throgh all our battlers
        {
            if (state == BattleState.Battle && allBattler[i].CurrHealth >= 0) 
            {
                switch (allBattler[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
                        // do the attack
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        // run 
                        yield return StartCoroutine(RunRoutine());
                        break;
                    default:
                        Debug.Log("Error - incorrect battle action");
                        break;
                }
            }
        }
        RemoveDeadBattlers();
        // if we havent won or lost, repeat the loop
        // by opening the battle menu
        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentHero = 0;
            ShowBattleMenu();
        }                                               
        
        yield return null;

    }
    private IEnumerator AttackRoutine(int i) 
    {
        
        // player turn
        if (allBattler[i].IsPlayer == true)
        {
            BattleEntities currAttacker = allBattler[i];
                if (allBattler[currAttacker.Target].CurrHealth <= 0)
                {
                currAttacker.SetTarget(GetRandomEnemy());
                }
            BattleEntities currTarget = allBattler[currAttacker.Target];
            
            AttackAction(currAttacker,currTarget);                     // attack selected enemy (attack action)
            yield return new WaitForSeconds(TURN_DURATION);            // wait a few seconds

            // kill the enemy                                                           
            if (currTarget.CurrHealth <= 0)       
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                
                yield return new WaitForSeconds(TURN_DURATION);        // wait a few seconds
               
                enemyBattlers.Remove(currTarget);
                if(enemyBattlers.Count <= 0)                           // if no enemies remain -> we won the battle 
                {
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);    // wait a few seconds
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                   
                }
            }

           
        }

        // enemies turn 
        if (allBattler[i].IsPlayer == false)
        {
            BattleEntities currAttacker = allBattler[i];
            currAttacker.SetTarget(GetRandomPartyMember());            // get random party member(target)
            BattleEntities currTarget = allBattler[currAttacker.Target];

            AttackAction(currAttacker, currTarget);                    // attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION);            // wait a few seconds


            if (currTarget.CurrHealth <= 0)                            // kill the party member 
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);        // wait a few seconds
                HeroesBattlers.Remove(currTarget);
                
              
                if(HeroesBattlers.Count <= 0)
                {
                    // if no party members remain  -> we lost the battle
                    state = BattleState.Lost;
                    bottomText.text = LOST_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);     // wait a few seconds
                    Debug.Log("Game Over");
                }

            }


        }
    }
    private void RemoveDeadBattlers()
    {
        for (int i = 0;i<allBattler.Count;i++) 
        {
            if (allBattler[i].CurrHealth <= 0)
            {
                allBattler.RemoveAt(i);
            }
        }
    }
    private IEnumerator RunRoutine()
    {
        if(state == BattleState.Battle)
        {
            if(Random.Range(1,101) >= RUN_CHANCE )
            {
                // we have run away
                bottomText.text = SUCCESFULLY_RAN_AWAY;               // set our bottom text that tell us we run away
                state = BattleState.Run;
                allBattler.Clear();                                   // clear all our battler list
                yield return new WaitForSeconds(TURN_DURATION);       // wait a few seconds
                SceneManager.LoadScene(OVERWORLD_SCENE);              // load the overworld scene 
                yield break;
            }
            else
            {
                // we failed to run away
                bottomText.text = UNSUCCESFULLY_RAN;                   // set our bottom text to say we faild 
                yield return new WaitForSeconds(TURN_DURATION);        // wait a few seconds
            }
        }
        
    }
    private void CreatePartyEntities()
    {
        // get current party
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();

        // create battle entities

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            // assign the values
            tempEntity.SetEntityValues(currentParty[i].MemberName , currentParty[i].CurrHealth,
                currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength
                ,currentParty[i].Level,true);
            
            
            // spawning the visuals
            BattleVisaulsManager tempBattleVisauls =
                Instantiate(currentParty[i].BattlePrefab,
                partySpawnPoints[i].position,Quaternion.identity).GetComponent<BattleVisaulsManager>();

            // set visuals starting values
            tempBattleVisauls.SetStartingValues(currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Level);

            // assign it to the battle entity
            tempEntity.BattleVisaulsManager = tempBattleVisauls;


            allBattler.Add(tempEntity);
            HeroesBattlers.Add(tempEntity);

        }
        
    }
    private void CreateEnemyEntities()
    {

        // get current enemies
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrentEnemies();
        // create battle entities
        for (int i = 0; i <currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            // assign the values
            tempEntity.SetEntityValues(currentEnemies[i].EnemyName,currentEnemies[i].CurrHealth, 
                currentEnemies[i].MaxHealth,currentEnemies[i].Initiative, 
                currentEnemies[i].Strength, currentEnemies[i].Level, false);

            // spawning the visuals
            BattleVisaulsManager tempBattleVisauls =
                Instantiate(currentEnemies[i].EnemyVisualPrefab,
                enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisaulsManager>();

            // set visuals starting values
            tempBattleVisauls.SetStartingValues(currentEnemies[i].MaxHealth, 
                currentEnemies[i].MaxHealth, currentEnemies[i].Level);


            // assign it to the battle entity
            tempEntity.BattleVisaulsManager = tempBattleVisauls;


            allBattler.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }
    public void ShowBattleMenu()
    {
        // whos action it is 
        actionText.text = HeroesBattlers[currentHero].Name + ACTION_MESSAGE;
        // enabling our battle menu
        battleMenu.SetActive(true);
    }
    public void ShowEnemySelectedMenu()
    {
        // disable battle menu
        battleMenu.SetActive(false);
        // set our enemy selection buttons
        SetEnemySelectionButtons();
        // enable our selection menu
        enemySelectionMenu.SetActive(true);
    }
    private void SetEnemySelectionButtons()
    {
        // disable all of our buttons
        for(int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);  
        }
        // enable buttons for each enemy
        for(int j = 0;j < enemyBattlers.Count; j++)
        {
            enemySelectionButtons[j].SetActive(true);
           // change the buttons text
            enemySelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name;
        }
        
    }
    public void SelectEnemy(int currentEnemy)
    {
        // setting the current members target
        BattleEntities currentHeroEntity = HeroesBattlers[currentHero];
        currentHeroEntity.SetTarget(allBattler.IndexOf(enemyBattlers[currentEnemy]));

        // tell the battle system this member intend to attack
        currentHeroEntity.BattleAction = BattleEntities.Action.Attack;
        // increment through our party members
        currentHero++;

        // if all players have selected an action 
        if (currentHero>=HeroesBattlers.Count)
        {
            // start the battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            // show the battle menu for the next player
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }

    }
    private void AttackAction(BattleEntities currAttacker,BattleEntities currTarget)
    {
        // get Damage
        int damage = currAttacker.Strength;                     // think of a better way to calculate damage
        currAttacker.BattleVisaulsManager.PlayAttackAnimation();// play the attack animation
        currTarget.CurrHealth -= damage;                        // dealing the damage
        currTarget.BattleVisaulsManager.PlayHitAnimation();     // play hit animation
        currTarget.UpdateUI();                                  // update the UI
        bottomText.text = string.Format("{0} attacks {1} for {2} damage",currAttacker.Name,currTarget.Name,damage);
        SaveHealth();
       
    }
    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>();          
        // find all party member -> add to list 
        for (int i = 0; i< allBattler.Count; i++)
        {
            if (allBattler[i].IsPlayer )
            {
                partyMembers.Add(i);
            }
        }

        return partyMembers[Random.Range(0,partyMembers.Count)];    // return random party member 
    }
    private int GetRandomEnemy()
    {
        List <int> enemies = new List<int>();
        for(int i = 0;  i< allBattler.Count;i++)
        {
            if (!allBattler[i].IsPlayer)
            {
                enemies.Add(i);
            }
        }
        return enemies[Random.Range(0,enemies.Count)];
    }
    private void SaveHealth()
    {
        for (int i = 0; i < allBattler.Count; i++)
        {
            if (allBattler[i].IsPlayer)
            {
                partyManager.SaveHealth(i, HeroesBattlers[i].CurrHealth);
            }
        }
    }
    public void SelectRunAction()
    {
        state = BattleState.Selection;

        BattleEntities currentHeroEntity = HeroesBattlers[currentHero];   // setting the current members target

        currentHeroEntity.BattleAction = BattleEntities.Action.Run;       // tell the battle system we intend to run

        battleMenu.SetActive(false);

        currentHero++;                                                    // increment through our party members

        if (currentHero >= HeroesBattlers.Count)                          // if all players have selected an action 
        {
           
            StartCoroutine(BattleRoutine());                               // start the battle
        }
        else
        {                          
            enemySelectionMenu.SetActive(false);                           // show the battle menu for the next player
            ShowBattleMenu();
        }
    }
}

[System.Serializable]
public class BattleEntities
{
    public enum Action {Attack , Run }
    public Action BattleAction;

    public string Name;
    public int CurrHealth;
    public int MaxLevel;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisaulsManager BattleVisaulsManager;
    public int Target;



    public void SetEntityValues(string name,int currHealth,int maxHealth,int initiative,int strength,int level,bool isPlayer)
    {
        Name = name;
        CurrHealth = currHealth;    
        MaxLevel = maxHealth;   
        Initiative = initiative;
        Strength = strength;
        Level = level;
        IsPlayer = isPlayer;

    }
    public void SetTarget(int target)
    {
        Target = target;
    }
    public void UpdateUI()
    {
        BattleVisaulsManager.ChangeHealthBar(CurrHealth);
    }
}
