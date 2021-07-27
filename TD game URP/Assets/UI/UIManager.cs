using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public GameObject[] towers;
    public GameObject[] ghostTowers;
    public int towerIndex = 0;
    [HideInInspector] public bool placeTower = false;

    [Header("UI CONNECTIONS")]
    public GameObject upgradesMenu;
    public GameObject towersMenu;
    public GameObject playButton;

    [Header("UI TEXTS")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI WinLossText;
    public TextMeshProUGUI towerText;

    [Header("UPGRADES CONNECTIONS")]
    public Button topButton;
    public TextMeshProUGUI topButtonName;
    public TextMeshProUGUI topButtonDesc;
    public TextMeshProUGUI topButtonCost;

    public Button botButton;
    public TextMeshProUGUI botButtonName;
    public TextMeshProUGUI botButtonDesc;
    public TextMeshProUGUI botButtonCost;

    public Button sellButton;

    [Header("PAUSED")]
    public GameObject pausedPannel;
    public GameObject pausedMenu;
    public float startBlur = 0f;
    public float endBlur = 20f;
    public float timeToTransition = 3f;
    float waitTime = 0f;

    [Header("UPGRADE INDEXES")]
    public Image[] topIndexes;
    public Image[] botIndexes;
    Tower currentTower = null;

    [Header("TOWER BUTTONS")]
    public GameObject[] towerButtons;

    public Color grayColor;

    [Header("ROMAN NUMERALS")]
    private int[] arabic = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
    private string[] roman = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

    GameObject cacheObj = null;

    public GameObject endScreen;

    private void Start()
    {
        UpdateMoneyText();
        UpdateWaveText();
        UpdateHealthText();
        pausedPannel.GetComponent<Image>().material.SetFloat("_Radius", startBlur);
        pausedMenu.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() => LevelManager.instance.QuitGame());
        pausedMenu.transform.Find("Cont").GetComponent<Button>().onClick.AddListener(() => LevelManager.instance.PauseGame());
    }

    public void ShowUpgradesMenu(Upgrades[] topUpgrade, Upgrades[] botUpgrade, Tower tower)
    {
        StartCoroutine(ShowUpgradesMenuIE(topUpgrade, botUpgrade, tower));
    }

    IEnumerator ShowUpgradesMenuIE(Upgrades[] topUpgrade, Upgrades[] botUpgrade, Tower tower)
    {
        yield return new WaitForSeconds(0.02f);

        towerText.text = tower.towerType.ToString() + " Tower";

        if (tower.topPathMaxed)
        {
            ShowUpgradeIndex(true, 3, true);
            //show upgrades menu
            topUpgrade[tower.TopPathUpgrades.Length - 1].CheckTopUpgrades();
            topButtonName.text = topUpgrade[tower.TopPathUpgrades.Length - 1].name;
            topButtonDesc.text = topUpgrade[tower.TopPathUpgrades.Length - 1].description;
            topButtonCost.text = "$" + topUpgrade[tower.TopPathUpgrades.Length - 1].cost;
        }
        else
        {
            ShowUpgradeIndex(true, tower.topPathIndex, false);
            //show upgrades menu
            topUpgrade[tower.topPathIndex].CheckTopUpgrades();
            topButtonName.text = topUpgrade[tower.topPathIndex].name;
            topButtonDesc.text = topUpgrade[tower.topPathIndex].description;
            topButtonCost.text = "$" + topUpgrade[tower.topPathIndex].cost;
            topButton.onClick.AddListener(topUpgrade[tower.topPathIndex].ApplyUpgradeTop);
        }

        if (tower.botPathMaxed)
        {
            ShowUpgradeIndex(false, 3, true);
            botUpgrade[tower.BottomPathUpgrades.Length - 1].CheckBotUpgrades();
            botButtonName.text = botUpgrade[tower.BottomPathUpgrades.Length - 1].name;
            botButtonDesc.text = botUpgrade[tower.BottomPathUpgrades.Length - 1].description;
            botButtonCost.text = "$" + botUpgrade[tower.BottomPathUpgrades.Length - 1].cost;
        }
        else
        {
            ShowUpgradeIndex(false, tower.botPathIndex, false);
            botUpgrade[tower.botPathIndex].CheckBotUpgrades();
            botButtonName.text = botUpgrade[tower.botPathIndex].name;
            botButtonDesc.text = botUpgrade[tower.botPathIndex].description;
            botButtonCost.text = "$" + botUpgrade[tower.botPathIndex].cost;
            botButton.onClick.AddListener(botUpgrade[tower.botPathIndex].ApplyUpgradeBottom);
        }

        currentTower = tower;

        upgradesMenu.SetActive(true);
    }

    public void ShowUpgradeIndex(bool isTopUpgrade, int index, bool isMaxed)
    {
        //top path indexing
        if (isTopUpgrade)
        {
            if (isMaxed)
            {
                foreach (Image i in topIndexes)
                {
                    i.color = Color.white;
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    topIndexes[i].color = Color.white;
                }
            }
        }
        //bot path indexing
        else
        {
            if (isMaxed)
            {
                foreach(Image i in botIndexes)
                {
                    i.color = Color.white;
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    botIndexes[i].color = Color.white;
                }
            }
        }
    }

    public void ShowTowersMenu()
    {
        //show tower menu
        towersMenu.SetActive(true);
    }
    public void HideTowersMenu()
    {
        //hide tower menu
        towersMenu.SetActive(false);
    }

    public void HideUpgradesMenu()
    {
        //hide upgrades menu
        StartCoroutine("HideUpgradesMenuIE");
    }

    IEnumerator HideUpgradesMenuIE()
    {
        yield return new WaitForSeconds(0.02f);

        //remove button listeners
        topButton.onClick.RemoveAllListeners();
        botButton.onClick.RemoveAllListeners();

        //make all the images gray
        foreach(Image i in botIndexes)
        {
            i.color = grayColor;
        }
        foreach (Image i in topIndexes)
        {
            i.color = grayColor;
        }

        //reset the text
        towerText.text = "Select a Tower";

        currentTower = null;

        //disable the menu
        upgradesMenu.SetActive(false);
    }

    public void ButtonHoverEnter(int index)
    {
        //increase scale
        towerButtons[index].transform.localScale += new Vector3(0.1f, 0.1f);
        //set text
        towerText.text = towers[index].GetComponent<Tower>().towerType.ToString() + " Tower";
    }

    public void ButtonHoverExit(int index)
    {
        //reset scale
        towerButtons[index].transform.localScale -= new Vector3(0.1f, 0.1f);
        //reset text
        towerText.text = "Select a Tower";
    }

    public void T_GatlingTower()
    {
        cacheObj = Instantiate(ghostTowers[0]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[0]);
    }

    public void T_SniperTower()
    {
        cacheObj = Instantiate(ghostTowers[1]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[1]);
    }

    public void T_CannonTower()
    {
        cacheObj = Instantiate(ghostTowers[2]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[2]);
    }

    public void T_AuraTower()
    {
        cacheObj = Instantiate(ghostTowers[3]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[3]);
    }

    public void T_RaderTower()
    {
        cacheObj = Instantiate(ghostTowers[4]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[4]);
    }

    public void T_Flamethrower()
    {
        cacheObj = Instantiate(ghostTowers[5]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[5]);
    }

    public void T_RocketLauncher()
    {
        cacheObj = Instantiate(ghostTowers[6]);
        cacheObj.GetComponent<GhostTower>().SetTower(towers[6]);
    }

    public void ResetPlacingTower()
    {
        Destroy(cacheObj);

        placeTower = !placeTower;
    }

    public bool CheckTower()
    {
        if(Time.timeScale == 1)
        {
            return placeTower;
        }
        else
        {
            return false;
        }
    }

    public void UpdateMoneyText()
    {
        moneyText.text = LevelManager.instance.GetMoney().ToString();
    }

    public void UpdateWaveText()
    {
        waveText.text = "Wave: " + IntToRoman(WaveManager_CS.instance.waveNumber + 1);
    }

    public void UpdateHealthText()
    {
        healthText.text = LevelManager.instance.GetHealth() + " / 100";
    }

    public void SellTower()
    {
        if (currentTower != null)
        {
            currentTower.gameObject.GetComponentInParent<Hex>().tower = null;
            LevelManager.instance.UpdateMoney(currentTower.cost / 2);
            HideUpgradesMenu();
            ShowTowersMenu();

            //connected to radar
            if (currentTower.isConnectedToRadarTower != null)
            {
                currentTower.isConnectedToRadarTower.towers.Remove(currentTower);
                currentTower.isConnectedToRadarTower = null;
            }
            if (currentTower.towerType == towerType.Radar)
            {
                currentTower.GetComponentInChildren<RadarRadius>().SoldThisTower();
            }

            Destroy(currentTower.gameObject);
        }
    }

    public string IntToRoman(int value)
    {
        string output = "";

        for (int i = 0; i < 13; i++)
        {
            while(value >= arabic[i])
            {
                output += roman[i].ToString();
                value -= arabic[i];
            }
        }
        return output;
    }

    IEnumerator BlurScreen()
    {
        StopCoroutine("UnblurScreen");
        waitTime = 0f;
        pausedMenu.gameObject.SetActive(true);
        while (waitTime < timeToTransition)
        {
            pausedPannel.GetComponent<Image>().material.SetFloat("_Radius", Mathf.Lerp(pausedPannel.GetComponent<Image>().material.GetFloat("_Radius"), endBlur, (waitTime / timeToTransition)));
            waitTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        pausedPannel.GetComponent<Image>().material.SetFloat("_Radius", endBlur);
        yield return null;
    }

    IEnumerator UnblurScreen()
    {
        StopCoroutine("BlurScreen");
        waitTime = 0f;
        pausedMenu.gameObject.SetActive(false);
        while (waitTime < timeToTransition)
        {
            pausedPannel.GetComponent<Image>().material.SetFloat("_Radius", Mathf.Lerp(pausedPannel.GetComponent<Image>().material.GetFloat("_Radius"), startBlur, (waitTime / timeToTransition)));
            waitTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        pausedPannel.GetComponent<Image>().material.SetFloat("_Radius", startBlur);
        yield return null;
    }

}
