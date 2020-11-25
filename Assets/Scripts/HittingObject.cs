using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClassManagement;
using UnityEngine.SceneManagement;

//CAP NHAT DIEM KHI TUNG DOI TUONG CHIEN THANG
public class HittingObject : MonoBehaviour
{
    public GameHandler a;
    public Text m;
    public Text s;
    public Text currScore, finalScore, finalScoreAI, finalScoreLosing, finalScoreAILosing;
    public int timeOut = 0;
    public int maxTime;
    public bool isEnd = false;
    public Canvas winning;
    public Canvas losing;
    public int iCount = 0;
    public int scoreOfMatch;

    public bool isPlayerDone = false;
    public bool isAIDone = false;
    void Start() {
        iCount = 0;
        isEnd = false;
        a = new GameHandler(SceneManager.GetActiveScene().buildIndex);
        scoreOfMatch = a.MaxScore;

        m = GameObject.Find("minTxt").GetComponent<Text>();
        s = GameObject.Find("secTxt").GetComponent<Text>();

        currScore = GameObject.Find("scoreTxt").GetComponent<Text>();
        finalScore = GameObject.Find("scoreTxtFinal").GetComponent<Text>();
        finalScoreAI = GameObject.Find("scoreAITxtFinal").GetComponent<Text>();
        finalScoreLosing = GameObject.Find("scoreTxtFinalLosing").GetComponent<Text>();
        finalScoreAILosing = GameObject.Find("scoreAITxtFinalLosing").GetComponent<Text>();
        winning.gameObject.SetActive(false);
        losing.gameObject.SetActive(false);
        maxTime = (int)a.TimeOut;
    }

    void OnTriggerEnter(Collider _col){
        //if(_col.tag == "Player") a.IsWon = true;

        if(_col.tag == "Player" && iCount == 0){
            iCount = 1;
        }else if(_col.tag == "AI" && iCount == 1){
            iCount = 5;
        }

        if(_col.tag == "AI" && iCount == 0){
            iCount = 3;
        }else if(_col.tag == "Player" && iCount == 3){
            iCount = 4;
        }
        
        if(iCount == 5){
            a.IsWon = true;
        }else if(iCount == 4){
            a.IsLost = true;
        }
    }

    void Update()
    {
        ContinueGame();
        if(iCount == 1){
            //Set score for Player First = a.TimeOut * a.MaxScore / a.MaxTime
            if(!isPlayerDone){
                a.UpdateCurrScorePlayer(a.getScore());
                finalScore.text = a.getScore().ToString();
                isPlayerDone = true;
            }
        }

        if(iCount == 5){
            //Set score for AI and stop the game
            if(!isAIDone){
                a.IsWon = true;
                a.UpdateCurrScorePlayerAI(a.getScore());
                finalScoreAI.text = a.getScore().ToString();
                isAIDone = true;
            }
        }

        if(iCount == 3){
            //Set Score for AI first
            if(!isAIDone){
                a.UpdateCurrScorePlayerAI(a.getScore());
                finalScoreAILosing.text = a.getScore().ToString();
                isAIDone = true;
            }
        }

        if(iCount == 4){
            //Set Score for Player and Stop the game
            if(!isPlayerDone){
                a.IsLost = true;
                a.UpdateCurrScorePlayer(a.getScore());
                finalScoreLosing.text = a.getScore().ToString();
                isPlayerDone = true;
            }
        }
        currScore.text = a.getScore().ToString();

        if(a.IsWon){
            winning.gameObject.SetActive(true);
            isEnd = true;
            PauseGame();
        }
        
        if(a.IsLost){
            losing.gameObject.SetActive(true);
            isEnd = true;
            PauseGame();
        }
        a.timeOut();
        timeOut = (int)a.TimeOut;

        updateScoreOfMatch();
        updatePlayingTimeLeft();
    }

    void PauseGame(){
        Time.timeScale = 0;
    }

    void ContinueGame(){
        Time.timeScale = 1;
    }

    void updatePlayingTimeLeft(){
        m.text = ((int)(timeOut/60)).ToString();
        s.text = ((int)(timeOut%60)).ToString();
        a.LosingChecker();
    }

    void updateScoreOfMatch(){
        scoreOfMatch = a.getScore();
        currScore.text = scoreOfMatch.ToString();
    }

    public void Quit(){
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void Replay(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
