using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    public UIManager UI;

    [System.Serializable]
    public class Paths
    {
        public Transform[] paths;
    }

    //paths
    public Paths[] paths;
    [HideInInspector]public int pathIndex = 0;

    [SerializeField]
    private int money = 100;
    public int health = 100;

    [Header("Tower Placement")]
    public LayerMask hexLayer;

    Tower previousTower = null;

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
    }

    private void Start()
    {
        health = 100;
        UI = GameObject.Find("UI").GetComponent<UIManager>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            LevelManager.instance.WinOrLose(false);
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer))
                {
                    //get the hex that was just clicked on
                    Hex hex = hit.transform.GetComponentInParent<Hex>();

                    //if the hex is not a path or unplaceable
                    if (hex.hexType != HexType.DefaultUnplaceable && hex.hexType != HexType.Path)
                    {
                        //if the hex has a tower
                        if (hex.tower != null)
                        {
                            //if there is no previous tower
                            if (previousTower == null)
                                //make this the previous tower
                                previousTower = hex.tower;
                            //if there is a previous tower
                            else
                            {
                                //hide its radius
                                previousTower.ShowRadius(false);
                                //show new radius
                                hex.tower.ShowRadius(true);

                                //show upgrades menu
                                UI.HideTowersMenu();
                                UI.HideUpgradesMenu();
                                UI.ShowUpgradesMenu(hex.tower.TopPathUpgrades, hex.tower.BottomPathUpgrades, hex.tower);

                                previousTower = hex.tower;
                            }
                                
                        }
                        //no tower on the hex
                        else
                        {
                            if (previousTower != null)
                            {
                                previousTower.ShowRadius(false);

                                UI.HideUpgradesMenu();
                                UI.ShowTowersMenu();
                            }       
                        }
                    }
                }
            }
            if (Input.GetButtonDown("Cancel"))
            {
                PauseGame();
            }
        }
    }

    public void HideMenus()
    {
        UI.HideUpgradesMenu();
        UI.ShowTowersMenu();
    }

    public void UpdateHealth(int amountToAdd)
    {
        health += amountToAdd;
        UI.UpdateHealthText();
    }

    public void UpdateMoney(int amountToAdd)
    {
        money += amountToAdd;
        UI.UpdateMoneyText();
    }

    public int GetMoney()
    {
        return money;
    }

    public int GetHealth()
    {
        return health;
    }

    public void PauseGame()
    {
        switch (Time.timeScale)
        {
            case 1:
                Time.timeScale = 0f;
                UI.StartCoroutine("BlurScreen");
                break;
            case 0:
                Time.timeScale = 1f;
                UI.StartCoroutine("UnblurScreen");
                break;
        }
    }

    public void WinOrLose(bool w)
    {
        Time.timeScale = 0;
        if(w)
        {
            UI.WinLossText.text = "You Win!!!";
        }
        else
        {
            UI.WinLossText.text = "You Suck!!!";
        }

        UI.endScreen.SetActive(true);
    }

    public void QuitGame()
    {
        print("QUIT");
        Application.Quit();
    }

}
