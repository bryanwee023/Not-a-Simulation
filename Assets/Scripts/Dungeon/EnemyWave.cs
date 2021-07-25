using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Enemy Wave")]
public class EnemyWave: ScriptableObject
{
    public GameObject[] enemyType;
    public int[] enemyCount;

    public List<GameObject> Get()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < enemyType.Length; i++)
        {
            for (int j = 0; j < enemyCount[i]; j++)
                list.Add(enemyType[i]);
        }
        return list;
    }
}
