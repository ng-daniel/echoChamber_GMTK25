using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventHolder : MonoBehaviour
{
    public delegate void DeathEvent(GameObject victim);
    public static DeathEvent OnDeath;

}
