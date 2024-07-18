using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> CurrentParty;

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private Vector3 playerPosition;
    private static GameObject instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
            AddMemberToPartyByName(defaultPartyMember.MemberName);
            AddMemberToPartyByName(defaultPartyMember.MemberName);
        }
        DontDestroyOnLoad(gameObject);
       
    }
    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++) 
        {
            if (allMembers[i].name == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMembers[i].MemberName;
                newPartyMember.Level = allMembers[i].StartingLevel;
                newPartyMember.CurrHealth = allMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMembers[i].BaseStr;
                newPartyMember.Initiative = allMembers[i].BaseInitiative;
                newPartyMember.BattlePrefab = allMembers[i].MemberBattlePrefab;
                newPartyMember.OverworldPrefab = allMembers[i].MemberOverworldPrefab;

                CurrentParty.Add(newPartyMember);

            }
        }
    }
    public List<PartyMember> GetCurrentParty()
    { 
        List<PartyMember> aliveParty = new List<PartyMember>();
        aliveParty = CurrentParty;
        for (int i = 0;i<aliveParty.Count; i++)
        {
            if (aliveParty[i].CurrHealth <= 0)
            {
                aliveParty.RemoveAt(i);
            }
        }
        return aliveParty;
    }
    public void SaveHealth(int partyMember , int health)
    {
        CurrentParty[partyMember].CurrHealth = health;
    }
    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }
    public Vector3 GetPosition()
    {
        return playerPosition;
    }
}

[System.Serializable]
public class PartyMember 
{
    public string MemberName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurrExp;
    public int MaxExp;
    public GameObject BattlePrefab;
    public GameObject OverworldPrefab;
}
