using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [Header("Camera")]
    public GameObject CamPoint;
    public int RotateScale = 0;

    [Header("Menus")]
    public GameObject[] menus;
    private void Start()
    {
        StartCoroutine("RotateBannana");
    }

    public void CloseGame()
    {
        Application.Quit();
        //TODO: play goodby sound and fade
    }

    public void Hidemenus()
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }

    public void ShowMenu(int index)
    {
        Hidemenus();
        menus[index].SetActive(true);
    }


    IEnumerator RotateBannana()
    {
        Vector3 camPointRotation = CamPoint.transform.eulerAngles;
        while (true)
        {
            camPointRotation.y -= Time.unscaledDeltaTime * RotateScale;
            CamPoint.transform.eulerAngles = camPointRotation;
            yield return new WaitForEndOfFrame();
        }
    }
}
