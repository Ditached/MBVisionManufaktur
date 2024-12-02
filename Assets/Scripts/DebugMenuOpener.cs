using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugMenuOpener : MonoBehaviour
{
    public float minTime = 2f;
    public float minDistance = 0.04f;

    public Transform a;
    public Transform b;

    public GameObject debugMenu;
    public GameObject[] otherObjects;
    
    [ReadOnly]
    public float currTime;

    private void Awake()
    {
        if (!Application.isEditor)
        {
            debugMenu.SetActive(false);
            
            foreach (var otherObject in otherObjects)
            {
                otherObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Vector3.Distance(a.position, b.position) < minDistance)
        {
            currTime += Time.deltaTime;
        }
        else
        {
            currTime -= Time.deltaTime;
            if(currTime < 0) currTime = 0;
        }
        
        if (currTime > minTime)
        {
            debugMenu.SetActive(!debugMenu.activeInHierarchy);
            
            foreach (var otherObject in otherObjects)
            {
                otherObject.SetActive(debugMenu.activeInHierarchy);
            }
            
            currTime = 0f;
        }
    }
}
