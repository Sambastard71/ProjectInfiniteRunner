using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{ 
    public SceneManagement sceneManagement;
    
    // Start is called before the first frame update
    void Start()
    {
        
        gameObject.SetActive(false);
    }

   

    // Update is called once per frame
    void Update()
    {
            //sceneManagement.StartLoadScene();
            
    }
}
