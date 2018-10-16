using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] cats;
    public Transform[] spawnPoints;
    public GameObject gameOverObject;

    private GameObject mouse;
    private int[] catsKilled;
    private int[] catCount; // TODO: decide if in screen or total since start of game

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
        if (Instance == null)
        {
            Instance = this;
        }
        return Instance;
    }
public void startGame()
    {
        destroyExistingCats();
        moveMouse();
        spawnCat(1);
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
        int spawnIndex = Random.Range(0, 5);

        //TODO: if cat created is charging cat, spawn lower? unless is flying then don't need
        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex]);
        Transform t = createdCat.GetComponent<Transform>();

        Vector3 newLocalScale;
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
}
