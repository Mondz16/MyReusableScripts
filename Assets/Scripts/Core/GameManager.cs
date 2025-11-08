using System;
using UnityEngine;
using ReusableScripts.Core;
using ReusableScripts.Event;

namespace ReusableScripts.Core{
    public class GameManager : Singleton<GameManager>{
        [Header("Game Setting")]
        [SerializeField] private bool _pauseOnStart = false;
        [SerializeField] private bool _lockCursorOnStart = false;

        private GameState _currentGameState = GameState.Menu;
        private GameState _previousGameState = GameState.Menu;

        public static event Action<GameState, GameState> OnGameStateChanged;
        public static event Action<bool> OnPauseToggled;

        public GameState CurrentGameState => _currentGameState;
        public GameState PreviousGameState => _previousGameState;
        public bool IsPaused {get; private set;}
        public bool IsGameActive => _currentGameState == GameState.Playing && !IsPaused;

        protected override void Awake(){
            base.Awake();
            InitializeGame();
        }

        private void Start(){
            if(_lockCursorOnStart){
                LockCursor();
            }

            if(_pauseOnStart){
                PauseGame();
            }
        }

        private void OnEnable(){
            Application.focusChanged += OnFocusChanged;
        }

        private void OnDisable(){
            Application.focusChanged -= OnFocusChanged;
        }

        private void InitializeGame(){
            ChangeState(GameState.Menu);
        }

        public void ChangeState(GameState newState){
            
            if(_currentGameState == newState) return;

            _previousGameState = _currentGameState;
            _currentGameState = newState;

            HandleStateChange(_previousGameState, _currentGameState);

            OnGameStateChanged?.Invoke(_previousGameState, _currentGameState);
        }

        private void HandleStateChange(GameState previousState, GameState newState){
            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    UnlockCursor();
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    LockCursor();
                    ResumeGame(); // Ensure game is not paused
                    break;

                case GameState.Paused:
                    // Pause is handled separately
                    break;

                case GameState.GameOver:
                    Time.timeScale = 1f; // Keep time scale normal for UI animations
                    UnlockCursor();
                    break;

                case GameState.Victory:
                    Time.timeScale = 1f;
                    UnlockCursor();
                    break;
            }
        }

        public void PauseGame(){
            if(IsPaused || _currentGameState != GameState.Playing) return;

            IsPaused = true;
            Time.timeScale = 0f;
            UnlockCursor();

            ChangeState(GameState.Paused);
            OnPauseToggled?.Invoke(true);
        }

        public void ResumeGame(){
            if(!IsPaused || _currentGameState != GameState.Paused) return;

            IsPaused = false;
            Time.timeScale = 1f;
            LockCursor();
        }

        public void TogglePause(){
            if(IsPaused){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }

        public void RestartGame(){
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
            IsPaused = false;
        }

        public void QuitGame(){
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void LockCursor(){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor(){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnFocusChanged(bool hasFocus){
            if(!hasFocus && _currentGameState == GameState.Playing){
                PauseGame();
            }
        }
    }

    public enum GameState{
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory,
        Loading
    }
}