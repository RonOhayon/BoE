using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private static GameObject instance;

    private const float LEVEL_MODIFIER = 0.5F;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
        }

       DontDestroyOnLoad(gameObject);
    }
    private void GenerateEnemyByName(string enemyName , int Level)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if(enemyName == allEnemies[i].EnemyName)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.EnemyName = allEnemies[i].EnemyName;
                newEnemy.Level = Level;
                float levelModifier = ( LEVEL_MODIFIER * newEnemy.Level);
                newEnemy.MaxHealth = Mathf.RoundToInt(allEnemies[i].BaseHealth + (allEnemies[i].BaseHealth* levelModifier));
                newEnemy.CurrHealth = newEnemy.MaxHealth;
                newEnemy.Strength = Mathf.RoundToInt(allEnemies[i].BaseStr + (allEnemies[i].BaseStr * levelModifier) );
                newEnemy.Initiative = Mathf.RoundToInt(allEnemies[i].BaseInitative + (allEnemies[i].BaseInitative * levelModifier));
                newEnemy.EnemyVisualPrefab = allEnemies[i].EnemyBattleVisualPrefeb;

                
                currentEnemies.Add(newEnemy);   

            }
        }
    }
    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }
    public void GenerateEnemyByEncounter(Encounter[]  encounters , int maxNumEnemies )
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(1, maxNumEnemies+1);

        for(int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = encounters[Random.Range(0,encounters.Length)];
            int level = Random.Range(tempEncounter.LevelMin,tempEncounter.LevelMax);
            GenerateEnemyByName(tempEncounter.Enemy.EnemyName, level);
        }
    }
}

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public GameObject EnemyVisualPrefab;
}