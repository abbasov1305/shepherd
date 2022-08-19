using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int coinCount;
    public Text coinText;

    public GameObject[] prefabs;

    [Header("Wolf")]
    public GameObject wolfPrefab;
    public float spawnRate = 5f;
    public float spawnRadius = 4f;
    private float nextSpawn;

    [Header("Sheep")]
    public int sheepCount;
    public Text sheepText;

    private bool gameStarted = false;

    private void Awake()
    {
        instance = this;

        Cursor.visible = false;
        coinCount = 50;
    }


    private void Start()
    {
        UpdateText();

        nextSpawn = spawnRate;
        sheepCount = 0;

    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        
        if (!gameStarted) return;


        if (nextSpawn <= Time.time)
        {
            SpawnWolf();
            float spawnTime = (spawnRate - (sheepCount / 2f));
            if (spawnTime > 1)
                nextSpawn += spawnTime;
            else
                nextSpawn += 1;
        }
    }

    public void EndDrag(int buttonId)
    {
        SpawnGO(prefabs[buttonId], Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void SpawnGO(GameObject prefab, Vector2 position)
    {
        int price = prefab.GetComponent<AnimalController>() != null ? prefab.GetComponent<AnimalController>().price : prefab.GetComponent<Grass>().price;

        if (price > coinCount) {
            coinText.text = "NOT ENOUGH";
            Invoke("UpdateText", 2f);
            return;
        }

        Instantiate(prefab, position, Quaternion.identity);
        coinCount -= price;

        UpdateText();
    }

    public void UpdateText()
    {
        coinText.text = coinCount.ToString();
        sheepText.text = sheepCount.ToString();
    }

    private void SpawnWolf()
    {
        Instantiate(wolfPrefab, new Vector2(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity);
    }

    public void StartGame()
    {
        gameStarted = true;
    }
}
