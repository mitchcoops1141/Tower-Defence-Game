using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTower : MonoBehaviour
{
    Color radiusMat;
    public Renderer radius;
    public LayerMask hexLayer;

    GameObject towerToPlace = null;

    private void Start()
    {
        radiusMat = radius.material.color;
    }

    public void SetTower(GameObject tower)
    {
        towerToPlace = tower;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer))
        {
            Hex hex = hit.transform.gameObject.GetComponentInParent<Hex>();

            //if its allowed to be placed on these hexes
            if(hex.hexType != HexType.DefaultUnplaceable && hex.hexType != HexType.Path)
            {
                //if there is no tower on this hex
                if (hex.tower == null)
                {
                    //if radius is red
                    if (radius.material.color != radiusMat)
                        //set radius to blue
                        radius.material.color = radiusMat;

                    //if click the mouse destroy the gameobject and level manager will spawn the tower
                    if (Input.GetMouseButtonDown(0) && LevelManager.instance.GetMoney() >= towerToPlace.GetComponent<Tower>().cost)
                    {
                        //instantiate the tower
                        GameObject cacheObj = Instantiate(towerToPlace, hex.transform);
                        //add the tower to the hex
                        hex.tower = cacheObj.GetComponent<Tower>();

                        switch (hex.hexType)
                        {
                            case HexType.Fire:
                                hex.tower.damage *= hex.fireMultiplier;
                                hex.tower.originalDamage = hex.tower.damage;
                                break;
                            case HexType.Wind:
                                hex.tower.firerate /= hex.windMultiplier;
                                hex.tower.originalFirerate = hex.tower.firerate;
                                break;
                            case HexType.Ice:
                                hex.tower.slowMultiplier = hex.iceMultiplier;
                                break;
                            default:
                                break;
                        }

                        LevelManager.instance.UpdateMoney(-towerToPlace.GetComponent<Tower>().cost);

                        //destroy ghost tower
                        Destroy(gameObject);
                    }
                }
                else
                {
                    //RED RADIUS
                    //if radius is blue
                    if (radius.material.color == radiusMat)
                        radius.material.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.35f);
                }
            }
            else if (hex.hexType == HexType.DefaultUnplaceable || hex.hexType == HexType.Path)
            {
                //RED RADIUS
                //if radius is blue
                if (radius.material.color == radiusMat)
                        radius.material.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.35f);
            }

            //set position
            transform.position = hex.transform.position;
            Debug.DrawRay(Camera.main.transform.position, (hit.point - Camera.main.transform.position) * 100f, Color.red);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
        }
    }
}
