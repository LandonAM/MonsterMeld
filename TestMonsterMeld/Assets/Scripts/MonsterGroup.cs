﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.IO;


/*
public enum CombatResult {
    FIGHTING,
    ALLY_WIN,
    ENEMY_WIN
}

public struct MonsterBattleInfo {
    public Monster ally;
    public Monster enemy;
    public CombatResult result;

    public MonsterBattleInfo(Monster Ally,Monster Enemy) {
        ally = Ally;
        enemy = Enemy;
        result = CombatResult.FIGHTING;
    }
}
*/

public class MonsterGroup : MonoBehaviour
{
    // Private Vars //
    //private List<MonsterBattleInfo> battles;
    private List<string> nameList = new List<string>();
    private List<Monster> monsters = new List<Monster>();
    private MonsterGroup enemyGroup; // group to fight against

    // Protected Vars //
    protected bool inCombat = false;

    // Public Vars //
    public GameObject monsterPrefab;
    public Vector3 spawnOffset = new Vector3(0,0,2);
    public Color groupColor;

    // Properties //
    public bool InCombat {
	    get { return this.inCombat; }
    }

    /// <summary>
    ///   The amount of monsters inside this group.
    /// </summary>
    public int Count {
	get { return monsters.Count; }
    }

    protected virtual void Start()
    {
        //monsters = new List<Monster>();
        //groupColor = Color.blue;
        LoadNames();

    }

    // Update is called once per frame
    protected void Update()
    {
    }

    private void LoadNames()
    {
        // https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
        string path = "Assets/Resources/MonsterNames.txt";
        StreamReader reader = new StreamReader(path, true);
        while (!reader.EndOfStream)
        {
            nameList.Add(reader.ReadLine());
        }
        reader.Close();
    }

    public void CreateMonster()
    {
        Monster monster = GameObject.Instantiate(monsterPrefab).GetComponent<Monster>();
        monster.transform.position = transform.position + spawnOffset;
        AddMonster(monster);
        // set monster to status quo
    }

    public void Follow(Transform target)
    {
        foreach (Monster m in monsters)
        {
            m.Follow(target);
        }
    }

    public void Attack(Monster enemyMonster) // attack MonsterGroup variant?
    {
        if(enemyGroup == null)
            enemyGroup = enemyMonster.GetGroup();
        inCombat = true;
        /*
        foreach(Monster m in monsters)
        {
            if(m.GetState() != MonsterState.ATTACK) {
                m.AttackMonster(monster);
            }
            //if(m.GetState() != MonsterState.ATTACK)
            //    m.AttackMonster(monster);
        }
        */

	    foreach(Monster m in monsters){
	        if(!m.HasEnemy()){
		        m.ChooseEnemy(enemyGroup.GetMonsters());
	        }
	    }

	    /*
        foreach (Monster e in enemyGroup.GetMonsters()) {
            if (!e.HasEnemy()) {
                bool allOccupied = true;   // if all our monsters are attacking
                foreach (Monster m in monsters) {
                    if (!m.HasEnemy()) {
                        m.AttackMonster(e);
                        allOccupied = false;
                    }
                }
                if (allOccupied)
                    return;
            }
        }*/

        // Attack Monster group
        // Find available enemy in group (doesn't have enemy and is close enough)
        // attack it
    }

    public void AddMonster(Monster monster)
    {
        monster.SetGroup(this);
        monster.SetColor(groupColor);
        monster.name = nameList[Random.Range(0, nameList.Count)];
	    //Debug.Log("MonsterGroup: Monsters:" + monsters + " Monster: " + monster);
        monsters.Add(monster);
        //monster.OnDeath += MonsterDeath;
        monster.OnKillTarget += MonsterKill;
    }

    public void RemoveMonster(Monster monster)
    {
        // remove event subscriptions
        monsters.Remove(monster);
    }

    public ReadOnlyCollection<Monster> GetMonsters() {
        return this.monsters.AsReadOnly();
    }

    protected virtual void MonsterAttacked(Monster monster,Monster monsterEnemey) {

    }


    public virtual void MonsterDeath(Monster monster,Monster monsterEnemy)
    {
        monsters.Remove(monster);
    }

    protected virtual void MonsterKill(Monster monster,Monster monsterEnemy)
    {
        // find next monster to attack
        //monster.Follow(transform); // change this???
        Debug.Log(monster.name + " KILLED ENEMY!!! NEXT TARGET...");

        if (enemyGroup.Count == 0) {
            Debug.Log(monster.name + ": ENEMY COUNT AT 0!!!");

            inCombat = false;
            monster.Follow(transform);
        } else {
            Debug.Log(monster.name + ": FINDING NEXT ENEMY");

            monster.Follow(transform);

            Monster enemyFound = monster.ChooseEnemy(enemyGroup.GetMonsters());
            //if (enemyFound == null)
            //{
            //    UnityEditor.EditorApplication.isPlaying = false;
            //    throw new System.Exception("you dun goofed");
            //}
	    }
    }

    void OnGUI() {
	    if(inCombat){
	        GUI.color = Color.red;
	        GUI.Box(new Rect(10,10,200,50), "In Combat with:\n" + enemyGroup.name);
	    }
        /*
        GUI.color = Color.black;

        GUI.Box(new Rect(10,10,500,20 + 10*monsters.Count),"Monsters");
        GUI.color = Color.white;
        int i = 0;
        foreach (Monster m in monsters) {
            GUI.Label(new Rect(10, 10+(i*10), 1000, 20),m.name+" Health: "+m.GetHealth().ToString()+" State: "+m.GetState().ToString() + "Combat State: "+m.GetCombatState());
            i++;
        }
        */
    }
}
