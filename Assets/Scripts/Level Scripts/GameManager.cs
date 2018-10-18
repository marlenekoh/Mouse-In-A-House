using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] cats;
    public Transform[] spawnPoints;
    public Transform[] boxSpawnPoints;
    public GameObject gameOverObject;
    public GameObject catBox;

    private GameObject mouse;
    private int[] catsKilled;
    private int[] catCount; // TODO: decide if in screen or total since start of game
    private bool[] spawnIndexTaken = new bool[6];

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        mouse = GameObject.Find("Mouse");
        startGame();
    }

    public static GameManager getInstance()
    {
        return Instance;
    }
public void startGame()
    {
        destroyExistingCats();
        moveMouse();
        spawnCat(1);
        spawnCat(0);
        gameOverObject.SetActive(false);
        pauseGame(false); 
    }

    private void moveMouse()
    {
        mouse.transform.position = new Vector3(0, -3, 0);
    }

    private void destroyExistingCats()
    {
        GameObject[] existingCats = GameObject.FindGameObjectsWithTag("Cat");
        for (int i = 0; i < existingCats.Length; i++)
        {
            Destroy(existingCats[i]);
        }
    }

    public void spawnCat()
    {
        spawnCat(chooseCat());
    }

    public void spawnCat(int index)
    {
        Debug.Log("I spawned cat with index " + index);

        Random.InitState(System.DateTime.Now.Millisecond);

        // to prevent cats from spawning in the same spot
        int spawnIndex = Random.Range(0, 5);
        spawnIndexTaken[spawnIndex] = true;

        while (!spawnIndexTaken[spawnIndex])
        {
            spawnIndex = Random.Range(0, 5);
            spawnIndexTaken[spawnIndex] = true;
        }

        StartCoroutine(spawnCatBox(index, spawnIndex));
    }

    private int chooseCat()
    {
        // TODO: Decide which cat to spawn based on number of cats killed etc etc
        // TODO: Increment cat spawn count
        Random.InitState(System.DateTime.Now.Millisecond);
        return 2;
    }
    
    public int getMaxSpeed()
    {
        // TODO: depends on adaptive difficulty level
        return 3;
    }

    public void gameOver()
    {
        gameOverObject.SetActive(true);
        pauseGame(true);
    }

    public static void pauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    IEnumerator spawnCatBox(int index, int spawnIndex)
    {
        Vector3 newLocalScale;

        GameObject tempCatBox = Instantiate(catBox, boxSpawnPoints[spawnIndex]);
        if (spawnIndex < 3) //should face the right
        {
            newLocalScale = tempCatBox.transform.localScale;
            newLocalScale.x = -newLocalScale.x;
            tempCatBox.transform.localScale = newLocalScale;
        }

        yield return new WaitForSeconds(1); // delay 1 second
        Destroy(tempCatBox);
        spawnIndexTaken[spawnIndex] = false;

        //TODO: if cat created is charging cat, spawn lower? unless is flying then don't need
        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex]);
        Transform t = createdCat.GetComponent<Transform>();

        if (index == 1) //jumping cat is smaller
        {
            newLocalScale = new Vector3(0.4f, 0.4f, 1);
        }
        else
        {
            newLocalScale = new Vector3(0.5f, 0.5f, 1);
        }

        if (spawnIndex >= 3) //should face the left
        {
            newLocalScale.x = -newLocalScale.x;
        }

        t.localScale = newLocalScale;
    }
}
