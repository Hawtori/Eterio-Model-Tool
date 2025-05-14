using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Toggles", menuName = "ScriptableObjects/Toggles", order = 1)]
public class Toggles : ScriptableObject
{
    public bool snapToEdge = false;
}
