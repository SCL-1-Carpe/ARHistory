using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField, Tooltip("allow to use item below")] bool AllowUsingToScene = true;

    [SerializeField,Tooltip("the scene you want to load")] string ToSceneName;

    [SerializeField, Tooltip("the time you need to load next scene")] float LoadTime = 0 ;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // you use this method into something like Butten in UI;
    public void ToLoadScene()
    {
        if (AllowUsingToScene)
        {
            Invoke("DoLoadScene", LoadTime);
        }
       
    }


    private void DoLoadScene()
    {
        if (ToSceneName!=null)
        {
            SceneManager.LoadScene(ToSceneName);
        }
        else
        {
            Debug.Log("<color=blue>null for ToSceneName</color>"); 

        }
       
    }


    
    


}
