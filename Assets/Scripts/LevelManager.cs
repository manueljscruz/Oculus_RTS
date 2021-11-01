using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public AudioClip BtnClickSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSceneToDemo()
    {
        AudioSource.PlayClipAtPoint(BtnClickSound, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
        SceneManager.LoadScene("RTS_DEMO", LoadSceneMode.Single);
    }

    public void ChangeSceneToMenu()
    {
        
        SceneManager.LoadScene("RTS_MENU", LoadSceneMode.Single);
    }
}
