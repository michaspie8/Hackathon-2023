using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public string Name = "Enemy";
    public Image image;
    public float health = 5;
    public int Damage = 1;
    public int Armor = 0;

    
}
