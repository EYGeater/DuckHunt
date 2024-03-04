using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform[] spawnPoints;
    public GameObject duckPrefab;
    public SpriteRenderer bg;
    public Color blueColor;

    public GameObject dogMiss, dogHit;
    public SpriteRenderer dogSprite;
    public Sprite[] victorySprites;

    public TextMeshProUGUI roundText;
    int roundNumber = 1;
    int totalTrials = 10;

    public int totalHits;
    int ducksCreated;
    bool isRoundOver;

    public TextMeshProUGUI scoreText, hitsText;
    int score, hits, totalClicks;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            totalClicks++;

            scoreText.text = score.ToString("000");
            hitsText.text = hits.ToString() + "/" + totalClicks;
        }
    }

    public void CallCreateDucks()
    {
        StartCoroutine(CreateDucks(2));
    }

    IEnumerator CreateDucks(int _count)
    {
        yield return new WaitForSeconds(1);
        for (int i = 0;i < _count;i++)
        {
            GameObject duck = Instantiate(duckPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
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
