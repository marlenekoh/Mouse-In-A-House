using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    public GameObject[] cats;
    public Transform[] spawnPoints;
    public Transform[] boxSpawnPoints;
    public GameObject gameOverObject;
    public GameObject catBox;
    public bool debugInvincibleMode;
    public int[] catsKilled;

    private GameObject mouse;
    private Utils utils;
    private bool stunModeOn;
    private int[] catCount; // TODO: decide if in screen or total since start of game
    private bool[] spawnIndexTaken = new bool[6];
    private float spawnDelay;

    private const int BASIC_CAT_INDEX = 0;
    private const int JUMPING_CAT_INDEX = 1;
    private const int CHARGING_CAT_INDEX = 2;

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
        utils = gameObject.GetComponent<Utils>();
        startGame();
    }

    private void Update()
    {
        if (stunModeOn)
        {
            StartCoroutine(waitNSeconds(1));
        }
    }

    public static GameManager getInstance()
    {
        return Instance;
    }

    public void startGame()
    {
        stunModeOn = false;
        destroyExistingCats(); // to clear existing cats when restart game
        moveMouse();
        catsKilled = new int[cats.Length];
        catCount = new int[cats.Length];
        spawnDelay = 1.0f;

        spawnCat(BASIC_CAT_INDEX);

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
        GameObject[] existingCatBoxes = GameObject.FindGameObjectsWithTag("CatBox");
        for (int i = 0; i < existingCatBoxes.Length; i++)
        {
            Destroy(existingCatBoxes[i]);
        }
    }

    public void spawnCat()
    {
        spawnCat(chooseCat());
    }

    public void spawnCat(int index)
    {
        StartCoroutine(waitNSeconds(spawnDelay));
        Random.InitState(System.DateTime.Now.Millisecond);

        // to prevent cats from spawning in the same spot
        int spawnIndex = (int) Mathf.Floor(Random.Range(0.0f, 5.9f));
        if (!spawnIndexTaken[spawnIndex])
        {
            spawnIndexTaken[spawnIndex] = true;
        }
        //Debug.Log("spawnIndex " + spawnIndex + " has been taken");
        while (!spawnIndexTaken[spawnIndex])
        {
            spawnIndex = Random.Range(0, 6);
            spawnIndex = 1;
            spawnIndexTaken[spawnIndex] = true;
            //Debug.Log("spawnIndex " + spawnIndex + " has been taken2");

        }

        StartCoroutine(spawnCatBox(index, spawnIndex));
    }

    public void destroyCat(GameObject cat)
    {
        cat.SetActive(false);
        StartCoroutine(waitNSeconds(1)); //don't kill it immediately
        Destroy(cat);
    }

    public void stunAllCats()
    {
        if (!stunModeOn)
        {
            stunModeOn = true;
            GameObject[] existingCats = GameObject.FindGameObjectsWithTag("Cat");

            for (int i = 0; i < existingCats.Length; i++)
            {
                StartCoroutine(stunCat(existingCats[i], i));
            }
        }
    }

    private int chooseCat()
    {
        // TODO: Decide which cat to spawn based on number of cats killed etc etc
        // TODO: Increment cat spawn count
        Random.InitState(System.DateTime.Now.Millisecond);
        return JUMPING_CAT_INDEX;
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

        GameObject tempCatBox = Instantiate(catBox, boxSpawnPoints[spawnIndex].position, boxSpawnPoints[spawnIndex].rotation);
        if (spawnIndex < 3) //should face the right
        {
            newLocalScale = tempCatBox.transform.localScale;
            newLocalScale.x = -newLocalScale.x;
            tempCatBox.transform.localScale = newLocalScale;
            tempCatBox.transform.localScale = newLocalScale;
        }

        yield return new WaitForSeconds(1); // delay 1 second
        Destroy(tempCatBox);
        StartCoroutine(freeSpawnPoint(spawnIndex));

        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex].position, spawnPoints[0].rotation);

        newLocalScale = createdCat.transform.localScale;
        Vector3 pos = createdCat.transform.position;
        if (spawnIndex >= 3) //should face the left
        {
            newLocalScale.x = -newLocalScale.x;
        }
        if (index == CHARGING_CAT_INDEX)
        {
            pos.y -= 0.4f;
            createdCat.transform.position = pos;
        }
        else if (index == JUMPING_CAT_INDEX)
        {
            pos.y += 0.3f;
            createdCat.transform.position = pos;
        }
        createdCat.transform.localScale = newLocalScale;
    }

    IEnumerator waitNSeconds(float n)
    {
        yield return new WaitForSeconds(n); // delay 1 second
        stunModeOn = false;
    }

    IEnumerator freeSpawnPoint(int spawnIndex)
    {
        yield return new WaitForSeconds(2);
        spawnIndexTaken[spawnIndex] = false;
        //Debug.Log("spawnIndex " + spawnIndex + " has been freed");
    }

    IEnumerator stunCat(GameObject cat, int index)
    {
        int catIndex = -1;

        if (cat.GetComponent<ChargeCatMovement>() != null)
        {
            cat.GetComponent<ChargeCatMovement>().setIsStunned(true);
            Debug.Log("stun charge cats");
            catIndex = CHARGING_CAT_INDEX;
        }
        else if (cat.GetComponent<JumpCatMovement>() != null)
        {
            cat.GetComponent<JumpCatMovement>().setIsStunned(true);
            catIndex = JUMPING_CAT_INDEX;
        }
        else if (cat.GetComponent<CatMovement>() != null)
        {
            cat.GetComponent<CatMovement>().setIsStunned(true);
            catIndex = BASIC_CAT_INDEX;
        }

        yield return new WaitForSeconds(1); // delay 1 second

        switch (catIndex)
        {
            case BASIC_CAT_INDEX:
                cat.GetComponent<CatMovement>().setIsStunned(false);
                break;
            case JUMPING_CAT_INDEX:
                cat.GetComponent<JumpCatMovement>().setIsStunned(false);
                break;
            case CHARGING_CAT_INDEX:
                cat.GetComponent<ChargeCatMovement>().setIsStunned(false);
                break;
        }
    }
}
