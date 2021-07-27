using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Wave
{
    public string name;
    public WavePortion[] wavePortions;

    //loops through each portion and calls the portion looping function. then returns the wave list
    public List<GameObject> GetWaveList()
    {
        List<GameObject> waveList = new List<GameObject>();

        foreach (WavePortion wp in wavePortions)
        {
            waveList.AddRange(wp.GetWaveListPortion());
        }

        return waveList;
    }

    public List<float> GetEnemyDelayList()
    {
        List<float> waveList = new List<float>();

        foreach (WavePortion wp in wavePortions)
        {
            waveList.AddRange(wp.GetEnemyDelayListPortion());
        }

        return waveList;
    }
}

[System.Serializable]
public class WavePortion
{
    public string name;
    public GameObject enemy;
    public int quantity;
    public float enemyDelay;

    //loops through the portion and creates a list of all the enemies in this portion
    public List<GameObject> GetWaveListPortion()
    {
        List<GameObject> waveList = new List<GameObject>();
        for (int i = 0; i < quantity; i++)
        {
            waveList.Add(enemy);
        }

        return waveList;
    }

    public List<float> GetEnemyDelayListPortion()
    {
        List<float> waveList = new List<float>();
        for (int i = 0; i < quantity; i++)
        {
            waveList.Add(enemyDelay);
        }

        return waveList;
    }
}

public class WaveManager_CS : MonoBehaviour
{
    public static WaveManager_CS instance = null;

    public Wave[] waves;
    public int waveNumber = 0;
    [SerializeField]
    private GameObject playButton;
    public int moneyWhenWaveFinish;

    //stat collection for enemies killed
    [HideInInspector]
    public int enemiesKilledTotal;
    [HideInInspector]
    public int enemiesKilledInWave = 0;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playButton = LevelManager.instance.UI.playButton;
    }

    //button will use this function
    public void StartWave()
    {
        StartCoroutine("StartWaveIE");
    }

    IEnumerator StartWaveIE()
    {
        //deactivate button
        playButton.SetActive(false);
        //reset wave counter
        enemiesKilledInWave = 0;

        for (int i = 0; i < waves[waveNumber].GetWaveList().Count; i++)
        {
            //instantiate the enemy according to the wave list
            Instantiate(waves[waveNumber].GetWaveList()[i], transform.position, transform.rotation);

            //wait time according to the enemy delay list
            yield return new WaitForSeconds(waves[waveNumber].GetEnemyDelayList()[i]);

            LevelManager.instance.pathIndex++;

            if (LevelManager.instance.pathIndex >= LevelManager.instance.paths.Length)
            {
                LevelManager.instance.pathIndex = 0;
            }
        }    
    }

    public void enemyKilled()
    {
        //increase enemies killed stats
        enemiesKilledInWave++;
        enemiesKilledTotal++;

        //if the amount of enemies in the wave is equal to the amount killed in wave
        if (enemiesKilledInWave == waves[waveNumber].GetWaveList().Count)
        {
            //increase the wave number
            waveNumber++;
            if(waveNumber > waves.Length-1)
            {
                LevelManager.instance.WinOrLose(true);
            }
            GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>().UpdateWaveText();

            //increase money
            LevelManager.instance.UpdateMoney(moneyWhenWaveFinish);

            //reactivate button
            playButton.SetActive(true);
        }
    }
}