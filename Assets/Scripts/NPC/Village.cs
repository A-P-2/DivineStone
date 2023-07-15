using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public void ChangeNPCStatus(bool isAggressive)
    {
        foreach (Transform child in transform)
            child.gameObject.GetComponent<NPCInfo>().isAggressive = isAggressive;
    }
}
