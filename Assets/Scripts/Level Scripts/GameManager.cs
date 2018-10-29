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
    public int[] catsToSpawn;
    public int sameLocationSpawnDelay;
    public float spawnDelay;

    // for adaptive difficulty
    private int level = 1;
    private int totalCats = 1;
    private float gameStartTime;

    private GameObject mouse;
    private Utils utils;
    private bool stunModeOn;
    private int[] catCount; // TODO: decide if in screen or total since start of game, do we really need this?
    public List<Transform> spawnIndexTakenList;

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
        catsToSpawn = new int[cats.Length];
        spawnIndexTakenList = new List<Transform>();
        level = 1;
        totalCats = 1;
        InvokeRepeating("increaseLevel", spawnDelay, spawnDelay);

        spawnCatsAfterN(0, spawnDelay);

        gameOverObject.SetActive(false);
        utils.pauseGame(false);
    }

    private void spawnCatsAfterN(float delay, float n)
    {
        CancelInvoke();
        //Debug.Log("invoke repeating, spawnDelay " + n);
        InvokeRepeating("spawnCat", delay, n);
        InvokeRepeating("increaseLevel", delay, n);
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

    private void increaseLevel()
    {
        level += 1;
        //Debug.Log("level" + level);
        totalCats = (int)Mathf.Floor(0.5f * Mathf.Floor((Time.time - gameStartTime) / 60.0f) + 1);
        //Debug.Log("totalcats " + totalCats);
    }

    public void spawnCat()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        switch (level)
        {
            // case 1 to 3 is to introduce the 3 different types of cats
            case 1:
                catsToSpawn[BASIC_CAT_INDEX] = 1;
                catsToSpawn[JUMPING_CAT_INDEX] = 0;
                catsToSpawn[CHARGING_CAT_INDEX] = 0;
                break;
            case 2:
                catsToSpawn[BASIC_CAT_INDEX] = 0;
                catsToSpawn[JUMPING_CAT_INDEX] = 1;
                catsToSpawn[CHARGING_CAT_INDEX] = 0;
                break;
            case 3:
                catsToSpawn[BASIC_CAT_INDEX] = 0;
                catsToSpawn[JUMPING_CAT_INDEX] = 0;
                catsToSpawn[CHARGING_CAT_INDEX] = 1;
                break;
            default:
                Random.InitState(System.DateTime.Now.Millisecond);
                int numCatsLeft = totalCats;
                int currNumCats = 0;
                catsToSpawn[JUMPING_CAT_INDEX] = 1;
                //for (int i = 0; i < catsToSpawn.Length - 1; i++)
                //{
                //    currNumCats = Random.Range(0, numCatsLeft + 1);
                //    catsToSpawn[i] = currNumCats;
                //    numCatsLeft -= currNumCats;
                //    catsToSpawn[i] = currNumCats;
                //}
                //catsToSpawn[catsToSpawn.Length - 1] = numCatsLeft;
                break;
        }


        for (int i = 0; i < catsToSpawn.Length; i++) // for each type of cat
        {
            for (int j = 0; j < catsToSpawn[i]; j++) // for number of times of each cat
            {
                // to prevent cats from spawning in the same spot
                int spawnIndex = (int)Mathf.Floor(Random.Range(0.0f, 5.9f));

                while (spawnIndexTakenList.Count < 6 && spawnIndexTakenList.Contains(spawnPoints[spawnIndex]))
                {
                    spawnIndex = (int)Mathf.Floor(Random.Range(0, 5.9f));
                }
                spawnIndexTakenList.Add(spawnPoints[spawnIndex]);

                //while (!spawnIndexTaken[spawnIndex])
                //{
                //    spawnIndex = (int) Mathf.Floor(Random.Range(0, 5.9f));
                //    spawnIndexTaken[spawnIndex] = true;
                //    Debug.Log(spawnIndex);
                //}
                StartCoroutine(spawnCatBox(i, spawnIndex));
            }
        }
    }

    public void onSuccessfulKill(GameObject cat)
    {
        int catIndex = -1;
        if (cat.GetComponent<ChargeCatMovement>() != null)
        {
            catIndex = CHARGING_CAT_INDEX;
        }
        else if (cat.GetComponent<JumpCatMovement>() != null)
        {
            catIndex = JUMPING_CAT_INDEX;
        }
        else if (cat.GetComponent<CatMovement>() != null)
        {
            catIndex = BASIC_CAT_INDEX;
        }
        catsKilled[catIndex]++;
        destroyCat(cat);
        stunAllCats();

    }

    public void destroyCat(GameObject cat)
    {
        cat.SetActive(false);
        StartCoroutine(waitNSeconds(2)); //don't kill it immediately
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
        // TODO: depends on adaptive difficulty level -- maybe not implementing
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

    public void updateSpawnDelay()
    {
        // currently spawndelay is decreasing at rate of 0.15 because 3 cats
        if (spawnDelay > 1)
        {
            spawnDelay = -1.0f / 16 * Mathf.Pow(Mathf.Floor((Time.time - gameStartTime) / 60.0f - 2), 3) + 3;
            //Debug.Log("spawn delay " + spawnDelay);
            spawnCatsAfterN(spawnDelay, spawnDelay);
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
        //StartCoroutine(freeSpawnPoint(spawnIndex));

        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex].position, spawnPoints[0].rotation);

        newLocalScale = createdCat.transform.localScale;
        Vector3 pos = createdCat.transform.position;
        if (spawnIndex >= 3) //should face the left
        {
            newLocalScale.x = -newLocalScale.x;
        }
        if (index == CHARGING_CAT_INDEX)
        {
            pos.y += 0.2f;
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
        yield return new WaitForSeconds(sameLocationSpawnDelay);
        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndexTakenList.Remove(spawnPoint);
    }

    IEnumerator stunCat(GameObject cat, int index)
    {
        int catIndex = -1;

        if (cat.GetComponent<ChargeCatMovement>() != null)
        {
            cat.GetComponent<ChargeCatMovement>().setIsStunned(true);
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

        yield return new WaitForSeconds(1.5f); // delay 1 second

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
