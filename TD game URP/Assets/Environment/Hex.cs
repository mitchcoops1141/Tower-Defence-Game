using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum HexType
{
    DefaultPlaceable,
    DefaultUnplaceable,
    Path,
    Fire,
    Wind,
    Ice
}


public class Hex : MonoBehaviour
{
    [EnumToggleButtons]
    [Title("Hex Type")]
    [HideLabel]
    public HexType hexType;

    //Fire Hex
    [Title("Fire")]
    [ShowIf("hexType", HexType.Fire)] public float fireMultiplier;

    //Wind Hex
    [Title("Wind")]
    [ShowIf("hexType", HexType.Wind)] public float windMultiplier;

    //Fire Hex
    [Title("Ice")]
    [ShowIf("hexType", HexType.Ice)] public float iceMultiplier;

    //the tower occupying the hex
    [HideInEditorMode] public Tower tower;

    private void Start()
    {
        tower = null;
    }
}
