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
    public bool debugInvincibleMode;

    private GameObject mouse;
    private Utils utils;
    private int[] catsKilled;
    private int[] catCount; // TODO: decide if in screen or total since start of game
    private bool[] spawnIndexTaken = new bool[6];

    private readonly int BASIC_CAT_INDEX = 0;
    private readonly int JUMPING_CAT_INDEX = 1;
    private readonly int CHARGING_CAT_INDEX = 2;

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
        utils = new Utils();
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
        utils.pauseGame(false); 
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
        //Debug.Log("I spawned cat with index " + index);

        Random.InitState(System.DateTime.Now.Millisecond);

        // to prevent cats from spawning in the same spot
        int spawnIndex = Random.Range(0, 5);
        if (!spawnIndexTaken[spawnIndex])
        {
            spawnIndexTaken[spawnIndex] = true;
        }
        Debug.Log("spawnIndex " + spawnIndex + " has been taken");
        while (!spawnIndexTaken[spawnIndex])
        {
            spawnIndex = Random.Range(0, 5);
            spawnIndexTaken[spawnIndex] = true;
            Debug.Log("spawnIndex " + spawnIndex + " has been taken2");

        }

        StartCoroutine(spawnCatBox(index, spawnIndex));
    }

    private int chooseCat()
    {
        // TODO: Decide which cat to spawn based on number of cats killed etc etc
        // TODO: Increment cat spawn count
        Random.InitState(System.DateTime.Now.Millisecond);
        return 1;
    }
    
    public int getMaxSpeed()
    {
        // TODO: depends on adaptive difficulty level
        return 3;
    }

    public void gameOver()
    {
        if (!debugInvincibleMode)
        {
            gameOverObject.SetActive(true);
            utils.pauseGame(true);
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
        StartCoroutine(freeSpawnPoint(spawnIndex));

        //TODO: if cat created is charging cat, spawn lower? unless is flying then don't need
        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex]);

        newLocalScale = createdCat.transform.localScale;
        if (spawnIndex >= 3) //should face the left
        {
            newLocalScale.x = -newLocalScale.x;
        }
        createdCat.transform.localScale = newLocalScale;
    }

    IEnumerator freeSpawnPoint(int spawnIndex)
    {
        yield return new WaitForSeconds(1); // delay 1 second
        spawnIndexTaken[spawnIndex] = false;
        Debug.Log("spawnIndex " + spawnIndex + " has been freed");
    }
}
