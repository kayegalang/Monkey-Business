
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{ 
   // [Header("Script References")] 
    
    
   // [Header("Scene References")]
    
    
    
  
   public enum State {Start, Battle, Pause, Results}

   public State state;
   
   private void Awake()
   {
       DontDestroyOnLoad(gameObject);
   }

   void Start() => ChangeState(State.Start);
       

   public void ChangeState(State newState)
   {
      state = newState; switch (newState)
      {
         case State.Start:
             SceneManager.LoadScene("StartScene");
            break;
         case State.Battle:
             //check if pause screen active
                //if true, resume game mechanics
             SceneManager.LoadScene("BattleScene");
            break;
         
         case State.Pause:
             //set active pause screen
             //StopGameMechanics();
             break;
         
         case State.Results:
             //check if win or lost
             
            break;
      }
   }
   
   public void StopGameMechanics()
   {
   }

   //move to its own menu buttons script
    public void OnStartButtonClick()
    {
       ChangeState(State.Battle);
   }
    
    public void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        Debug.Log("Application Quit!");
        #endif
    }
    

   

}
