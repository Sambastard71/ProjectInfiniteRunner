using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject firstStage;
    public GameObject StageParent;

    public GameObject[] prefabStages;
    public float Speed;

    List<GameObject> ActiveStages;

    Vector3 SpeedToSum;
    Vector3 posOfSpawnNextStage = new Vector3(85, 230, -900);
    int r;
    // Start is called before the first frame update
    void Start()
    {
        ActiveStages = new List<GameObject>();

        r = Random.Range(0, prefabStages.Length-1);

        GameObject secondStage = Instantiate(prefabStages[r],StageParent.transform);
        secondStage.transform.localPosition = posOfSpawnNextStage;

        ActiveStages.Add(firstStage);
        ActiveStages.Add(secondStage);

    }

    // Update is called once per frame
    void Update()
    {
        SpeedToSum = new Vector3(Speed,0,0);

        if(ActiveStages[0].transform.localPosition.x <= -2300)
        {
            GameObject go = ActiveStages[0];
            ActiveStages.Remove(go);
            DestroyImmediate(go);
            

       
            r = Random.Range(0, prefabStages.Length - 1);
            GameObject go1 = Instantiate(prefabStages[r],StageParent.transform);
            go1.transform.localPosition = posOfSpawnNextStage;
            ActiveStages.Add(go1);

        }

        foreach (GameObject stage in ActiveStages)
        {
            stage.transform.position -= SpeedToSum;
        }
    }
}
