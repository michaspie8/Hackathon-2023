using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public int id;
    public int level;
    public float range;
    public int damage;
    new public string name;
    // model
}
