
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
       state = newState;
       switch (newState)
       {
           case State.Start:
               if (SceneManager.GetActiveScene().name != "StartScene")
                   SceneManager.LoadScene("StartScene");
               break;
           case State.Battle:
               if (SceneManager.GetActiveScene().name != "BattleScene")
                   SceneManager.LoadScene("BattleScene");
               break;
           case State.Pause:
               
               break;
           case State.Results:
               break;
       }
   }
   

}
