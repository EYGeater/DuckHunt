using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform[] spawnPoints;
    public GameObject duckPrefab;
    [SerializeField] int ducksToSpawn; // original value = 2
    Duck[] ducks;
    public SpriteRenderer bg;
    public Color blueColor;

    public GameObject dogMiss, dogHit;
    public SpriteRenderer dogSprite;
    public Sprite[] victorySprites;

    public TextMeshProUGUI roundText;

    //int roundNumber = 1; // Removing memory that's not currently being used - Amon
    //int totalTrials = 10;

    public int totalHits;
    int ducksCreated;
    bool isRoundOver;

    public TextMeshProUGUI scoreText, hitsText;
    string scoreTxt, hitsTxt; // caching references to TMP_Text of the above UI to avoid get() set() function calls
    int score, hits, totalClicks;

    // Using Awake() instead of Start() so other classes can call GameManager on Start() or Enable() - Amon
    private void Awake()
    {
        // Checking to see if there's already an instance of GameManager, and destroying this instance if there is
        // *Not really needed here because there's no persistance between scenes
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        scoreTxt = scoreText.text;
        hitsTxt = hitsText.text;

        ducks = new Duck[ducksToSpawn]; // setup array of ducks ahead of time, so we can reference the current ducks easily
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            totalClicks++;

            scoreTxt = score.ToString("000");
            hitsTxt = $"{hits} / {totalClicks}"; // string interpolation over string concatenation
        }
    }

    public void CallCreateDucks()
    {
        StartCoroutine(CreateDucks(ducksToSpawn)); // created ducksToSpawn variable for scaling
    }

    IEnumerator CreateDucks(int _count)
    {
        yield return new WaitForSeconds(1);
        int spwnPts = spawnPoints.Length; // cache reference to spawnPoints.length to avoid function call
        for (int i = 0; i < _count; i++)
        {
            // get references to instantiated ducks here, to avoid having to find them later
            ducks[i] = Instantiate(duckPrefab, spawnPoints[Random.Range(0, spwnPts)].position, Quaternion.identity).GetComponent<Duck>();
        }

        StartCoroutine(Timeup());
    }

    IEnumerator Timeup()
    {
        yield return new WaitForSeconds(10f);
        Duck[] ducks = FindObjectsOfType<Duck>();
        for (int i = 0; i < ducks.Length; i++)
        {
            ducks[i].TimeUp();
        }
        bg.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        bg.color = blueColor;
        if (!isRoundOver)
        {
            StartCoroutine(RoundOver());
        }
    }

    public void HitDuck()
    {
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        totalHits++;
        score += 10;
        hits++;
        bg.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        bg.color = blueColor;
        ducksCreated--;

        if (ducksCreated <= 0)
        {
            if (!isRoundOver)
            {
                StopCoroutine(Timeup());
                StartCoroutine(RoundOver());
            }
        }
    }

    IEnumerator RoundOver()
    {
        isRoundOver = true;
        yield return new WaitForSeconds(1f);

        if (ducksCreated <= 0)
        {
            dogHit.SetActive(true);
            dogSprite.sprite = victorySprites[0];
        }
        else if (ducksCreated == 1)
        {
            dogHit.SetActive(true);
            dogSprite.sprite = victorySprites[1];
        }
        else
        {
            dogMiss.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        dogHit.SetActive(false);
        dogMiss.SetActive(false);
        CallCreateDucks();
        isRoundOver = false;
    }
}
