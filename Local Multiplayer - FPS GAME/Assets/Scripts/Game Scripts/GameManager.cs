using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager gmInstance { get; private set; }

    private void Awake()
    {
        if (gmInstance != null && gmInstance != this)
        {
            Destroy(this);
        }
        else
        {
            gmInstance = this;
        }
    }

    //GM checks winner
    //GM checks loser
    //GM checks player Deaths
    //GM assits with pause sequence 
    //GM assists with audio queues
    [Header("References")] 
    public PlayerController _player1;
    public PlayerController _player2;
    
    
    public GameObject player1;
    public GameObject player2;
    public Camera PodiumCamera;
    public GameObject Podium;
    public GameObject Canvas;
    
    [Header("LiveGame Checks")]
    [Header("Scenes")]
    public bool isSceneLoading = false; 
    [Header("LiveTimer")]
    [SerializeField]private float timer = 0f; 
    [SerializeField]private int currentTime = 0;
    [Header("Halfway")]
    private bool player1Halfway = false;
    private bool player2Halfway = false;
    [Header("Endgame")]
    public bool hasWon = false;
    public bool hasDied = false;




    
    private void Start()
    {
        currentTime = 0;
        DeactivatePlayersMovement();
        Canvas.SetActive(false);
        AudioManager.Instance.PlaySound("Tiles Rising", 1 , 0.5f, 0f, 1.5f);
        AudioManager.Instance.PlaySound("Announcer - Start", 1 , 0.5f, 5.5f, 1f);

        
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            IncrementTimer(1);
            timer = 0f; 
        }

        if (currentTime >= 10f && !hasWon && !hasDied)
        {
            ActivatePlayersMovement();    
            Canvas.SetActive(true);
        }
    }
    
    public void IncrementTimer(int incrementAmount)
    {
        currentTime += incrementAmount;
    }
    
    //Timed Event functions
    public void ActivatePlayersMovement()
    {
        _player1.enabled = true;
        _player2.enabled = true;
    }
    
    public void DeactivatePlayersMovement()
    {
        _player1.enabled = false;
        _player2.enabled = false;
    }
    
    //win conditions
    public void CheckWinner(GameObject player)
    {
        if (hasWon || isSceneLoading) return; 
        hasWon = true;
        isSceneLoading = true; 

        Canvas.SetActive(false);
        PodiumCamera.enabled = true;
        StartCoroutine(SceneManage.smInstance.WaitBeforeLoading());
        if (player == player1)
        {
       
            player1.transform.position = new Vector3(Podium.transform.position.x, Podium.transform.position.y + 2, Podium.transform.position.z);
            player2.transform.position = new Vector3(Podium.transform.position.x - 1f, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            player2.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            StartCoroutine(_player1.TriggerRumble(0.1f, 0.6f, 0.1f));
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("Chime", 1, 1f, 0f,1f);
                AudioManager.Instance.PlaySound("Player1Wins", 1, 1f, 1.6f,1f);
                AudioManager.Instance.PlaySound("TaDa", 1, 1f, 5f,1f);
            }
            Debug.Log("Loading back to start scene.");
        }
        else if (player == player2)
        {

            player2.transform.position = new Vector3(Podium.transform.position.x, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.position = new Vector3(Podium.transform.position.x - 1f, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            player2.transform.rotation = Quaternion.Euler(0f, 360f, 0f); ;
           StartCoroutine(_player1.TriggerRumble(0.1f, 0.6f, 0.1f));
           if (AudioManager.Instance != null)
           {
               AudioManager.Instance.PlaySound("Chime", 1, 1f, 0f,1f);
               AudioManager.Instance.PlaySound("Player2Wins", 1, 1f, 0.5f,1f);  
               AudioManager.Instance.PlaySound("TaDa", 1, 1f, 5f,1f); 
           }
           Debug.Log("Loading back to start scene.");
    
        }
    }
    
    public void CheckDeath(GameObject player)
    {
        if (hasDied || isSceneLoading) return; 
        hasDied = true;
        
        isSceneLoading = true; 

        Canvas.SetActive(false);
        PodiumCamera.enabled = true;
        StartCoroutine(SceneManage.smInstance.WaitBeforeLoading());
        if (player == player1)
        {
            
            player2.transform.position = new Vector3(Podium.transform.position.x, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.position = new Vector3(Podium.transform.position.x - 1f, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            player2.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("player1touched", 1, 1f, 0.5f,1f);  
            }
            StartCoroutine(_player1.TriggerRumble(0.1f, 0.6f, 0.1f));
        }
        else if (player == player2)
        {
            player1.transform.position = new Vector3(Podium.transform.position.x, Podium.transform.position.y + 2, Podium.transform.position.z);
            player2.transform.position = new Vector3(Podium.transform.position.x - 1f, Podium.transform.position.y + 2, Podium.transform.position.z);
            player1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            player2.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("player2touched", 1, 1f, 0.5f,1f);
            }
            StartCoroutine(_player1.TriggerRumble(0.1f, 0.6f, 0.1f));
        }
    }

    public void CheckHalfWay(GameObject player)
    {
        if (player == player1)
        {
            if (!player1Halfway)  
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound("Player1halfway", 1, 1f, 0.5f, 1f);
                }
                player1Halfway = true; 
            }
        }
        else if (player == player2)
        {
            if (!player2Halfway) 
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound("Player2halfway", 1, 1f, 0.5f, 1f);
                }
                player2Halfway = true; 
            }
        }

        if (player1Halfway && player2Halfway)
        {
            OnBothPlayersHalfway();
        }
    }


    private void OnBothPlayersHalfway()
    {
        Debug.Log("Both players have reached halfway!");

    }
}
   
    

