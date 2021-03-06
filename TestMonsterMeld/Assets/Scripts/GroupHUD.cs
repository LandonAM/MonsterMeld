﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupHUD : MonoBehaviour
{
    private GameObject infoTemplate;
    private List<MonsterInfoPanel> panels = new List<MonsterInfoPanel>();
    private Transform groupPanel;
    private Text countText;
    private Transform combatPanel;
    private Transform membersPanel;
    private Text enemyText;

    private float baseHeight;
    
    public MonsterGroup group;
    public bool showGroupPanel;
    
    void Start() {
	infoTemplate = Resources.Load<GameObject>("Prefabs/UI/Info Panel");
	groupPanel = transform.Find("GROUP_PANEL");
	
	if(showGroupPanel){
	    groupPanel.gameObject.SetActive(true);

	}else{
	    groupPanel.gameObject.SetActive(false);
	}
	
	Color groupCol = group.groupColor;
	groupCol = new Color(groupCol.r,groupCol.g,groupCol.b,groupPanel.GetComponent<Image>().color.a);
	groupPanel.Find("GROUP_NAME").GetComponent<Text>().text = group.name;
	groupPanel.GetComponent<Image>().color = groupCol;
	countText = groupPanel.Find("GROUP_COUNT").GetComponent<Text>();
	combatPanel = groupPanel.Find("COMBAT_PANEL");
	enemyText = combatPanel.Find("ENEMY_GROUP").GetComponent<Text>();
	membersPanel = groupPanel.Find("MEMBERS_PANEL");

	baseHeight = groupPanel.GetComponent<RectTransform>().sizeDelta.y;
	    
	group.OnAddMonster += MonsterAdded;
	group.OnRemoveMonster += MonsterRemoved;
	foreach(Monster m in group.GetMonsters()){
	    //Debug.Log("HALLO!");
	    AddPanel(m);
	}
    }

    void Update() {
        if(showGroupPanel){
	    countText.text = group.Count.ToString();
	    if(group.InCombat){
		if(!combatPanel.gameObject.activeSelf)
		    combatPanel.gameObject.SetActive(true);
		enemyText.text = group.GetEnemyGroup().name;
	    }else{
		if(combatPanel.gameObject.activeSelf)
		    combatPanel.gameObject.SetActive(false);
	    }
	    groupPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(250,baseHeight+group.Count*50);
	    for(int i = 0;i<membersPanel.childCount;i++){
		Transform child = membersPanel.GetChild(i);
		child.GetComponent<RectTransform>().anchoredPosition = new Vector2(50,i*(-50) + 50);
	    }
	}
    }    

    private void AddPanel(Monster m){
	MonsterInfoPanel panel = Instantiate(infoTemplate).GetComponent<MonsterInfoPanel>();
	MonsterInfoPanel sidePanel = Instantiate(infoTemplate).GetComponent<MonsterInfoPanel>();
	
	panel.name = m.name + " " + panel.name;
	sidePanel.name = m.name + " " + sidePanel.name;
	panel.transform.SetParent(this.transform);
	sidePanel.transform.SetParent(membersPanel);
	panel.SetMonster(m);
	sidePanel.SetMonster(m);
	sidePanel.followMonster = false;
	sidePanel.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 50;
	panels.Add(panel);
	panels.Add(sidePanel);
    }

    private void MonsterAdded(Monster m){
	AddPanel(m);
    }

    private void MonsterRemoved(Monster m){
	Debug.Log("MONSTER REMOVED..." + m);
	List<MonsterInfoPanel> panelsToRemove = new List<MonsterInfoPanel>();
	foreach(MonsterInfoPanel p in panels){
	    if(p.Monster == m){
		panelsToRemove.Add(p);
		//Debug.Log("MATCHED MONSTER");
		//MonsterInfoPanel pan = p;
		//panels.Remove(p);
		//Destroy(pan.gameObject);
		//break;
	    }
	}
	foreach(MonsterInfoPanel p in panelsToRemove){
	    panels.Remove(p);
	    Destroy(p.gameObject);
	}
    }


}
