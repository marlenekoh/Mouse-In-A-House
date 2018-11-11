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
    public GameObject explosion;
    public GameObject catBox;
    public GameObject stunTimer;
    public bool debugInvincibleMode;
    public int[] catsKilled;
    public int[] catsToSpawn;
    public int sameLocationSpawnDelay;
    public float spawnDelay;
    public int numCatsEscaped;
    public float TimeModifier;
    public int NumCatsKillForIncrement;
    public int NumCatsSiameded;
    public int LevelDampener;
    public int LevelCap;
    public int NumBasicCatKills;
    public int NumJumpCatKills;
    public int NumChargeCatKills;

    // for adaptive difficulty
    public int level = 1;

    public int totalCats = 1;
    public int totalCatsMinusDiff = 1;
    private float gameStartTime;
    private float gameStartSpawnTime;
    private int catCounter = 0; // if counter is 1, spawn cat at mouse level
    private float[] catProb;
    private int[] catProportion;
    private int killCounter;

    public int difficultyMod;
    public int difficultyModMin;

    private GameObject mouse;
    private Utils utils;
    private bool stunModeOn;
    private int[] catCount; // TODO: decide if in screen or total since start of game, do we really need this?
    public List<Transform> spawnIndexTakenList;

    private const int BASIC_CAT_INDEX = 0;
    private const int JUMPING_CAT_INDEX = 1;
    private const int CHARGING_CAT_INDEX = 2;

    private void Start()
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
        mouse.GetComponent<SpriteRenderer>().enabled = true;
        utils = gameObject.GetComponent<Utils>();
        startGame();
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

        catProb = new float[cats.Length];
        catProb[BASIC_CAT_INDEX] = 2.0f / 3.0f;
        catProb[JUMPING_CAT_INDEX] = 1.0f / 6.0f;
        catProb[CHARGING_CAT_INDEX] = 1.0f / 6.0f;
        difficultyMod = 0;
        numCatsEscaped = 0;
        killCounter = 0;

        //TEMP = 0.2f;
        //TEMP2 = 3;
        //TEMP3 = 5;

        spawnIndexTakenList = new List<Transform>();
        level = 1;
        totalCats = 1;
        totalCatsMinusDiff = 1;
        InvokeRepeating("increaseLevel", spawnDelay, spawnDelay);

        spawnCatsAfterN(1.0f, spawnDelay);
        gameStartTime = Time.time;
        gameStartSpawnTime = Time.time;

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
        float currTime = Time.time;

        if (numCatsEscaped / NumCatsSiameded >= 1)
        {
            if (difficultyMod >= difficultyModMin)
            {
                difficultyMod--;
            }
            
            if (difficultyMod > 5)
            {
                difficultyMod--;
            }
            numCatsEscaped = 0;
        }
        difficultyMod = (difficultyMod > LevelCap) ? LevelCap : Mathf.Max(difficultyModMin, difficultyMod);

        int prevCats = totalCatsMinusDiff;
        totalCatsMinusDiff  = (int)Mathf.Floor(0.3f * Mathf.Round((currTime - gameStartTime) / 60.0f) + 1);
        totalCats = Mathf.Min(4,(totalCatsMinusDiff + (difficultyMod / NumCatsKillForIncrement))); // for every TEMP2 cats

        if (prevCats != totalCatsMinusDiff) // when I increase number of cats spawning per wave, increase back spawndelay
        {
            gameStartSpawnTime = Time.time;
        }

        //Debug.Log("level " + level);
        //Debug.Log("Difficulty mod " + difficultyMod);
        //Debug.Log("numCatsEscaped " + numCatsEscaped);


        catProportion = new int[12];
        for (int i = 0; i < 12; i++)
        {
            int multiplier = (int) Mathf.Min(Mathf.Round((currTime - gameStartTime) / 60.0f), 3);

            if (i < 8 - 2 * multiplier)
            {
                catProportion[i] = BASIC_CAT_INDEX;
            }
            else if (i >= 8 - 2 * multiplier && i < 10 - multiplier)
            {
                catProportion[i] = JUMPING_CAT_INDEX;
            }
            else
            {
                catProportion[i] = CHARGING_CAT_INDEX;
            }
        }
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
                int numCatsLeft = totalCats;
                //Debug.Log("level " + level);

                float currTime = Time.time;

                Random.InitState(System.DateTime.Now.Millisecond);
                catsToSpawn = new int [cats.Length];

                for (int i=0; i<totalCats; i++)
                {
                    int currCatToSpawn = Random.Range(0, catProportion.Length);
                    catsToSpawn[catProportion[currCatToSpawn]]++;
                }
                break;
        }
        //Debug.Log(mouse.GetComponent<Transform>().position);

        for (int i = 0; i < catsToSpawn.Length; i++) // for each type of cat
        {
            for (int j = 0; j < catsToSpawn[i]; j++) // for number of times of each cat
            {
                // to prevent cats from spawning in the same spot
                int spawnIndex;
                if (catCounter == 1)
                {
                    // spawn cat on same level as mouse
                    if (mouse.GetComponent<Transform>().position.y >= 1.6)
                    {
                        // spawn at top
                        spawnIndex = 0;
                        if (mouse.GetComponent<Transform>().position.x >= -2.1)
                        {
                            spawnIndex = 3;
                        }
                    }
                    else if (mouse.GetComponent<Transform>().position.y >= -0.7)
                    {
                        // spawn at middle
                        spawnIndex = 1;
                        if (mouse.GetComponent<Transform>().position.x >= -2.1)
                        {
                            spawnIndex = 4;
                        }
                    }
                    else
                    {
                        // spawn at bottom
                        spawnIndex = 2;
                        if (mouse.GetComponent<Transform>().position.x >= -2.1)
                        {
                            spawnIndex = 5;
                        }
                    }
                    catCounter = 0;
                }
                else
                {
                    spawnIndex = (int)Mathf.Floor(Random.Range(0.0f, 5.9f));
                    catCounter++;
                }

                while (spawnIndexTakenList.Count < 6 && spawnIndexTakenList.Contains(spawnPoints[spawnIndex]))
                {
                    spawnIndex = (int)Mathf.Floor(Random.Range(0, 5.9f));
                }
                spawnIndexTakenList.Add(spawnPoints[spawnIndex]);

                StartCoroutine(spawnCatBox(i, spawnIndex));
                StartCoroutine(freeSpawnPoint(spawnIndex));
            }
        }
    }

    public void onSuccessfulKill(GameObject cat)
    {
        int catIndex = -1;
        if (cat.GetComponent<ChargeCatMovement>() != null)
        {
            catIndex = CHARGING_CAT_INDEX;
            cat.GetComponent<ChargeCatMovement>().onCatDeath();
        }
        else if (cat.GetComponent<JumpCatMovement>() != null)
        {
            catIndex = JUMPING_CAT_INDEX;
            cat.GetComponent<JumpCatMovement>().onCatDeath();
        }
        else if (cat.GetComponent<CatMovement>() != null)
        {
            catIndex = BASIC_CAT_INDEX;
            cat.GetComponent<CatMovement>().onCatDeath();
        }
        catsKilled[catIndex]++;

        if ((catsKilled[BASIC_CAT_INDEX] == NumBasicCatKills) || (catsKilled[JUMPING_CAT_INDEX] == NumJumpCatKills) || (catsKilled[CHARGING_CAT_INDEX] == NumChargeCatKills))
        {
            difficultyMod++;
            resetCounters();
        }

        //if (killCounter == NumCatKills)
        //{
        //    //difficultyMod++;
        //    killCounter = 0;
        //}
        //else
        //{
        //    killCounter++;
        //}

        if (difficultyMod < LevelDampener)
        {
            numCatsEscaped = 0;
        }

        difficultyMod = (difficultyMod > LevelCap) ? LevelCap : Mathf.Max(difficultyModMin, difficultyMod);


        SfxManager.PlaySound("catDeathCry");
        destroyCat(cat);
        stunAllCats(cat);

    }

    private void resetCounters()
    {
        for (int i = 0; i < cats.Length; i++)
        {
            catsKilled[i] = 0;
        }
    }

    public void destroyCat(GameObject cat)
    {
        StartCoroutine(playCatDeathAnim(cat));
        //StartCoroutine(waitNSeconds(2)); //don't kill it immediately
        //Destroy(cat);
    }

    IEnumerator playCatDeathAnim(GameObject cat)
    {
        yield return new WaitForSeconds(1); // delay 1 second
        Destroy(cat);
    }

    public void stunAllCats(GameObject catToIgnore)
    {
        if (!stunModeOn)
        {
            stunModeOn = true;
            StartCoroutine(waitForStunModeOff());

            GameObject[] allGameObjectsWithCatTag = GameObject.FindGameObjectsWithTag("CatBack");

            List<GameObject> existingCats = new List<GameObject>();
            for (int i = 0; i < allGameObjectsWithCatTag.Length; i++)
            {
                GameObject cat = allGameObjectsWithCatTag[i].transform.parent.gameObject;
                if (!existingCats.Contains(cat))
                {
                    existingCats.Add(cat);
                }
            }
            Debug.Log("existingCats " + existingCats.Count);
            if (existingCats.Count > 1)
            {
                stunTimer.SetActive(true);
                stunTimer.GetComponentInChildren<Animator>().SetTrigger("stunCountdown");
                StartCoroutine(waitForStunTimer());
            }

            for (int i = 0; i < existingCats.Count; i++)
            {
                if (existingCats[i] != catToIgnore)
                {
                    StartCoroutine(stunCat(existingCats[i], i));
                }
            }
        }
    }

    public void gameOver()
    {
        if (!debugInvincibleMode)
        {
            mouse.GetComponent<SpriteRenderer>().enabled = false;
            gameOverObject.SetActive(true);
            stunTimer.SetActive(false);
            utils.pauseGame(true);
        }
    }

    public void updateSpawnDelay()
    {
        if (spawnDelay > 1)
        {
            //Debug.Log("spawn delay " + spawnDelay);
            spawnDelay = -1.0f / 16 * Mathf.Pow(Mathf.Round((Time.time - gameStartSpawnTime) / 60.0f - 2), 3) + 3 - TimeModifier * difficultyMod;
        }
        else
        {
            spawnDelay = 1;
        }
       
        if (!stunModeOn)
        {
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
        StartCoroutine(spawnExplosion(index, spawnIndex));
        StartCoroutine(spawnCat(index, spawnIndex));
    }

    IEnumerator spawnExplosion(int index, int spawnIndex)
    {
        GameObject tempExplosion = Instantiate(explosion, boxSpawnPoints[spawnIndex].position, boxSpawnPoints[spawnIndex].rotation);
        yield return new WaitForSeconds(0.7f);
        Destroy(tempExplosion);

    }

    IEnumerator spawnCat(int index, int spawnIndex)
    {
        SfxManager.PlaySound("catSpawn");
        Vector3 newLocalScale;
        yield return new WaitForSeconds(0.1f);
        GameObject createdCat = Instantiate(cats[index], spawnPoints[spawnIndex].position, spawnPoints[0].rotation);

        newLocalScale = createdCat.transform.localScale;
        Vector3 pos = createdCat.transform.position;
        if (spawnIndex >= 3) //should face the left
        {
            newLocalScale.x = -newLocalScale.x;
        }
        createdCat.transform.localScale = newLocalScale;

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

        yield return new WaitForSeconds(3.0f); // delay 3 seconds

        if (cat != null)
        {
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

    IEnumerator waitForStunTimer()
    {
        yield return new WaitForSeconds(4.0f);
        stunTimer.SetActive(false);
    }

    IEnumerator waitForStunModeOff()
    {
        yield return new WaitForSeconds(3.0f);
        stunModeOn = false;
    }
}
