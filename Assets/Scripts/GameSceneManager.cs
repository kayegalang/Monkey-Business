
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{ 
    
   public enum State {Start, Battle, Pause, Results}

   public State state;
   
   public static GameSceneManager Instance { get; private set; }

   [Header("Data References")]
   [SerializeField] private BananaWallet wallet;
   [SerializeField] private UpgradeStat[] allUpgrades;

   private GameObject pauseShadePanel;
   
   private void Awake()
   {
       if (Instance != null && Instance != this)
       {
           Destroy(gameObject);
           return;
       }

       Instance = this;
       DontDestroyOnLoad(gameObject);

       SceneManager.sceneLoaded += OnSceneLoaded;
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
               {
                   ResetAllData();
                   SceneManager.LoadScene("BattleScene");
               }
               else
               {
                   SetPausePanel(false);
               }
               break;
           case State.Pause:
               SetPausePanel(true);
               break;
           case State.Results:
               break;
       }
   }
   
   private void OnDestroy()
   {
       SceneManager.sceneLoaded -= OnSceneLoaded;
   }
   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
       switch (scene.name)
       {
           case "BattleScene":
               pauseShadePanel = GameObject.Find("PauseShadePanel");
               // delay hiding so UIButtonManager can find buttons inside it first
               Invoke(nameof(HidePausePanel), 0.1f);
               break;

           case "StartScene":
               pauseShadePanel = null;
               break;
       }
   }

   private void HidePausePanel()
   {
       if (pauseShadePanel != null)
           pauseShadePanel.SetActive(false);
   }
 
 private void ResetAllData()
 {
     wallet.Reset();
 
     foreach (var upgrade in allUpgrades)
     {
         upgrade.Reset();
     }
 
     Debug.Log("All data reset!");
 }
 
 private void SetPausePanel(bool active)
 {
     if (pauseShadePanel == null)
     {
         Debug.LogWarning("PauseShadePanel not found!");
         return;
     }
 
     pauseShadePanel.SetActive(active);
 }

}
