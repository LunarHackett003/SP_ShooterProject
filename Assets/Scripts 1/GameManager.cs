using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player playerRef;

    public static GameManager instance;
    public GameObject pauseCanvas;
    public bool paused;

    public TextMeshProUGUI ammoDisplayText;


    public Volume damageVolume;
    public Slider healthbar;
    
    public void PauseGame(bool newPause)
    {
        //pauses the game
        //I don't much like the timescale method but there's not much else I can think to do other than disabling most components and that might have a wonky effect
        //this is but a humble game so its okay :)
        paused = newPause;
        Time.timeScale = paused ? 0 : 1;
        pauseCanvas.SetActive(paused);
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Awake()
    {
        //Initialise singleton if one doesn't already exist, or destroy this object if it does
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //Check if the main camera in the scene does or doesn't have a Cinemachine Brain,
        //Allows it to work with the player
        if (!Camera.main.GetComponent<CinemachineBrain>())
        {
            Camera.main.gameObject.AddComponent<CinemachineBrain>();
        }
        //Get the player 
        if (!playerRef)
            playerRef = FindFirstObjectByType<Player>();
        //Force the game to be unpaused
        PauseGame(false);
    }


    // Update is called once per frame
    private void FixedUpdate()
    {

        if (playerRef.weaponManager.CurrentWeapon)
        {
            (int max, int current) = playerRef.weaponManager.CurrentWeapon.Ammo;
            ammoDisplayText.text = $"{current}\n/{max}";
        }
    }


}

