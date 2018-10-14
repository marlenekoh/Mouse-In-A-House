using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] cats;
    public Transform[] spawnPoints;

    private int[] catsKilled;

    private void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void spawnCat(int index)
    {
        int spawnIndex = Random.Range(0, 5);
        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex]);
        Transform t = createdCat.GetComponent<Transform>();
        if (spawnIndex >= 3)
        {
            t.localScale = new Vector3(-0.5f, 0.5f, 1);
        }
        else
        {
            t.localScale = new Vector3(0.5f, 0.5f, 1);
        }
    }
}
