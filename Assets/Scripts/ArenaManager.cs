using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    public Countdown countdownTimer;
    public TMP_Text commandText;
    public TMP_Text seekerScoreText;
    public TMP_Text hiderScoreText;
    public TMP_Text winStateText;
    private string hideStr = "HIDE";
    private string seekStr = "SEEK";
    private string seekerWonText = "S won!\n(press r)";
    private string hiderWonText = "H won!\n(press r)";
    
    public ArenaComponentsManager entityManager;

    public float timeBeforeSeekStarts = 3;
    public float timeForSeek = 20;

    private MaterialController _materialController;
    

    public bool roundComplete = false;
    
    public int hiderScore = 0;
    public int seekerScore = 0;
    
    public bool hiderControlled = false;
    public bool seekerControlled = false;


    public bool immediateRestart = false;
    
    
    public ArenaAnimator _animator;

    private bool playersSetup = false;
    
    private IEnumerator roundCoroutine;

    public bool animateLights = true;
    

    // Start is called before the first frame update
    void Start()
    {
        _materialController = FindObjectOfType<MaterialController>();
        ResetArena();
        
    }


    void ResetArena()
    {
        _animator.ResetAnimations();
        winStateText.text = "";
        roundComplete = false;
        playersSetup = false;
        entityManager.ResetArenaComponents();
        setupPlayers();

        commandText.text = hideStr;
        commandText.alpha = 1;
        roundCoroutine = PlayRound();
        StartCoroutine(roundCoroutine);
    }

    IEnumerator PlayRound()
    {
        
        
        if (seekerControlled)
        {
            UpdateMovement(entityManager.generatedHiders, true);
            commandText.alpha = 1;
            commandText.text = seekStr;
            _animator.FadeCommandText();
            countdownTimer.ResetTimer(timeForSeek);
            UpdateMovement(entityManager.generatedSeekers, true);
            _materialController.sightHidden = true;
                yield return new WaitForSeconds(timeForSeek);
            if (!roundComplete)
                transform.root.BroadcastMessage("HiderWon");
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            UpdateMovement(entityManager.generatedHiders, true);
            countdownTimer.ResetTimer(timeBeforeSeekStarts);
            _animator.FadeCommandText(timeBeforeSeekStarts * 0.5f);
            if (animateLights)
                _animator.DimLight();
            yield return new WaitForSeconds(timeBeforeSeekStarts);
            Debug.Log("Time Before Seek over");
            commandText.alpha = 1;
            commandText.text = seekStr;
            _animator.FadeCommandText();
            countdownTimer.ResetTimer(timeForSeek);
            UpdateMovement(entityManager.generatedSeekers, true);
            if (seekerControlled)
            {
                _materialController.sightHidden = true;
            }
            yield return new WaitForSeconds(timeForSeek);
            if (!roundComplete)
                transform.root.BroadcastMessage("HiderWon");
        }
    }


    void setupPlayers()
    {
        foreach (var hiderGo in entityManager.generatedHiders)
        {
            hiderGo.GetComponent<CharacterController>().SetPlayerCotrollerStatus(hiderControlled);
        }
        foreach (var seekerGo in entityManager.generatedSeekers)
        {
            seekerGo.GetComponent<CharacterController>().SetPlayerCotrollerStatus(seekerControlled);
        }
        playersSetup = true;
        
        
    }



    void UpdateMovement(List<GameObject> playerList, bool movementOn)
    {
        foreach (var go in playerList)
        {
            CharacterController cc = go.GetComponent<CharacterController>();
            if (movementOn)
                cc.turnMovementOn();
            else
                cc.turnMovementOff();
        }
    }

    IEnumerator AutomatedRestart()
    {
        
        yield return new WaitForSeconds(2f);
        ResetArena();
    }


    // Update is called once per frame
    void Update()
    {

        if (roundComplete)
        {
            StopCoroutine(roundCoroutine);

        }

        if (Input.GetKeyDown("r"))
        {
            print("r key was pressed");
            ResetArena();
        }

    }



    void RoundComplete()
    {
        UpdateMovement(entityManager.generatedHiders, false);
        UpdateMovement(entityManager.generatedSeekers, false);
        _materialController.sightHidden = false;
        if (animateLights)
            _animator.ResetLight();
        roundComplete = true;
        if (immediateRestart)
        {
           StartCoroutine(AutomatedRestart());
        }
    }

    public void SeekerWon()
    {
        if (!roundComplete)
        {
            seekerScore += 1;
            winStateText.text = seekerWonText;
            seekerScoreText.text = "S:" +  String.Format("{0:00}", seekerScore);
            RoundComplete();
        }
        
    }
    
    public void HiderWon()
    {
        if (!roundComplete)
        {
            hiderScore += 1;
            winStateText.text = hiderWonText;
            hiderScoreText.text = "H:" +  String.Format("{0:00}", hiderScore);
            RoundComplete();
        }

        
    }
    
    
}