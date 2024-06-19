using System;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

using Sokoban.GameManagement;
using Sokoban.GridEditor;
using Sokoban.UI;
using NUnit.Framework.Internal;

namespace Sokoban.LevelManagement
{
  public sealed class LevelManager : SingletonInSceneNoInstance<LevelManager>
  {
    [SerializeField] private LevelData _currentLevelData;
    [SerializeField] private LevelProgressData _currentLevelProgressData;

    [Space(10)]
    [SerializeField] private GridLevel _gridLevel;

    [Space(10)]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _levelCompleteMenu;
    [SerializeField] private GameObject _levelFailedMenu;
    [SerializeField] private Panel _menuPanel;

    [Space(10)]
    [SerializeField] private TypeDifficultyGame _currentTypeDifficultyGame;

    //--------------------------------------

    private GameManager gameManager;

    private CinemachineCamera cinemachineCamera;

    private bool isCameraRotation;

    private AudioManager audioManager;

    private LevelSounds levelSounds;

    private int tempAmountFoodCollected = 0;

    private DifficultyGame difficultyGame;

    private bool isLevelMenu = false;

    //======================================

    public bool LevelCompleted { get; set; }

    public bool LevelFailed { get; set; }

    public bool IsPause { get; set; }

    public bool IsLevelMenu { get; private set; }
    
    public GridLevel GridLevel => _gridLevel;

    public CinemachineCamera CinemachineCamera
    {
      get => cinemachineCamera;
      set => cinemachineCamera = value;
    }

    public bool IsLevelRunning { get; set; } = false;

    //======================================

    public UnityEvent<float> ChangeTimeOnLevel { get; } = new UnityEvent<float>();

    public UnityEvent<int> ChangeNumberMoves { get; } = new UnityEvent<int>();

    public event Action ChangeDifficultyGame;

    public UnityEvent<bool> OnPauseEvent { get; } = new UnityEvent<bool>();

    public event Action OnLevelCompleted;

    public UnityEvent IsNextLevel { get; } = new UnityEvent();

    public UnityEvent<LevelData> IsNextLevelData { get; } = new UnityEvent<LevelData>();

    public UnityEvent IsReloadLevel { get; } = new UnityEvent();

    public UnityEvent IsMenu { get; } = new UnityEvent();

    //======================================

    public LevelData GetCurrentLevelData() => _currentLevelData;

    public LevelProgressData GetCurrentLevelProgressData() => _currentLevelProgressData;

    //======================================

    public float TimeOnLevel
    {
      get => _currentLevelProgressData.TimeOnLevel;
      private set
      {
        _currentLevelProgressData.TimeOnLevel = value;
        ChangeTimeOnLevel?.Invoke(value);
      }
    }

    public int NumberMoves
    {
      get => _currentLevelProgressData.NumberMoves;
      set
      {
        _currentLevelProgressData.NumberMoves = value;
        ChangeNumberMoves?.Invoke(value);
      }
    }

    public TypeDifficultyGame CurrentTypeDifficultyGame { get => _currentTypeDifficultyGame; set => _currentTypeDifficultyGame = value; }

    //======================================

    private new void Awake()
    {
      gameManager = GameManager.Instance;

      cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();

      audioManager = AudioManager.Instance;

      levelSounds = GetComponent<LevelSounds>();

      difficultyGame = new DifficultyGame();
    }

    private void Start()
    {
      MenuLevel();
      //_currentLevelData = Levels.CurrentSelectedLevelData;

      //ReloadLevel();
    }

    private void LateUpdate()
    {
      CameraRotationLoop();

      ResetCameraRotation();

      if (_currentLevelData == null)
        return;

      if (IsPause)
        return;

      if (LevelCompleted)
        return;

      if (!_gridLevel.IsLevelCreated)
        return;
      
      TimeOnLevel += Time.deltaTime;

      /*if (_currentTypeDifficultyGame == TypeDifficultyGame.Easy)
        return;

      difficultyGame.IsLevelFailedTime();*/
    }

    private void OnEnable()
    {
      OnLevelCompleted += LevelManager_OnLevelCompleted;
      difficultyGame.OnLevelFailed += DifficultyGame_OnLevelFailed;

      //InputHandler.Instance.AI_Player.Player.Test.performed += Test_performed;
    }

    private void OnDisable()
    {
      OnLevelCompleted -= LevelManager_OnLevelCompleted;
      difficultyGame.OnLevelFailed -= DifficultyGame_OnLevelFailed;

      //InputHandler.Instance.AI_Player.Player.Test.performed += Test_performed;
    }

    //======================================

    private void CameraRotationLoop()
    {
      if (!IsLevelMenu)
        return;

      float yRotation = cinemachineCamera.transform.rotation.eulerAngles.y + Time.deltaTime * 5f;
      Quaternion rotation = Quaternion.Euler(48f, yRotation, 0f);
      cinemachineCamera.transform.rotation = rotation;
    }

    private void ResetCameraRotation()
    {
      if (!isCameraRotation)
        return;

      Quaternion currentRotation = cinemachineCamera.transform.rotation;
      Quaternion targetQuaternion = Quaternion.Euler(48.0f, 0.0f, 0.0f);
      Quaternion newRotation = Quaternion.Slerp(currentRotation, targetQuaternion, 3f * Time.deltaTime);

      cinemachineCamera.transform.rotation = newRotation;

      if (Quaternion.Angle(currentRotation, targetQuaternion) < 0.01f)
      {
        isCameraRotation = false;
        cinemachineCamera.transform.rotation = targetQuaternion;
      }
    }

    private void LevelComplete()
    {
      if (LevelCompleted)
        return;

      OnLevelCompleted?.Invoke();
    }

    public bool IsLevelComplete()
    {
      var groundObjects = _gridLevel.GetListGroundObjects();
      foreach (var groundObject in groundObjects)
      {
        if (!groundObject.IsBlockActive)
          return false;
      }

      LevelComplete();
      return true;
    }

    public void IsFoodCollected()
    {
      tempAmountFoodCollected++;
    }

    public void SetPause(bool parValue)
    {
      IsPause = parValue;
      OnPauseEvent?.Invoke(parValue);
    }

    //======================================

    private void LevelManager_OnLevelCompleted()
    {
      LevelCompleted = true;
      /*LevelFailed = false;

      if (LevelFailed)
      {
        tempAmountFoodCollected = 0;
        return;
      }*/

      GameManager.Instance.ProgressData.AmountFoodCollected += tempAmountFoodCollected;
      GameManager.Instance.ProgressData.TotalFoodCollected += tempAmountFoodCollected;
      tempAmountFoodCollected = 0;

      audioManager.OnPlaySound?.Invoke(levelSounds.LevelComplete);

      OpenNextLevel();
    }

    private void DifficultyGame_OnLevelFailed()
    {
      LevelFailed = true;

      //OnLevelCompleted?.Invoke();
    }

    //======================================

    public string UpdateTextTimeLevel()
    {
      int hours = Mathf.FloorToInt(TimeOnLevel / 3600f);
      int minutes = Mathf.FloorToInt((TimeOnLevel % 3600f) / 60f);
      int seconds = Mathf.FloorToInt(TimeOnLevel % 60f);

      if (hours > 0)
        return $"{hours:00}:{minutes:00}:{seconds:00}";
      else if (minutes > 0)
        return $"{minutes:00}:{seconds:00}";
      else
        return $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Загрузить новый уровень
    /// </summary>
    public void UploadNewLevel()
    {
      IsNextLevel?.Invoke();

      var levelData = Levels.GetNextLevelData(_currentLevelData.Location, _currentLevelData.LevelNumber);
      if (levelData != null)
        _currentLevelData = levelData;

      gameManager.ProgressData.LocationLastLevelPlayed = levelData.Location;
      gameManager.ProgressData.IndexLastLevelPlayed = levelData.LevelNumber;

      IsNextLevelData?.Invoke(_currentLevelData);

      isCameraRotation = true;

      ReloadLevel(_currentLevelData);
    }

    public void ReloadLevelDeath()
    {
      IsReloadLevel?.Invoke();
      _gridLevel.ReloadLevel();

      TimeOnLevel = 0;
      NumberMoves = 0;
      LevelCompleted = false;

      tempAmountFoodCollected = 0;
    }

    public void ReloadLevel()
    {
      isCameraRotation = true;
      IsReloadLevel?.Invoke();
      _gridLevel.CreatingLevelGrid();

      TimeOnLevel = 0;
      NumberMoves = 0;
      LevelCompleted = false;

      tempAmountFoodCollected = 0;
    }

    public void ReloadLevel(LevelData levelData)
    {
      gameManager.ProgressData.IndexLastLevelPlayed = levelData.LevelNumber;

      _currentLevelData = levelData;

      IsNextLevelData?.Invoke(_currentLevelData);

      isCameraRotation = true;

      IsReloadLevel?.Invoke();
      _gridLevel.CreatingLevelGrid();

      TimeOnLevel = 0;
      NumberMoves = 0;
      LevelCompleted = false;
      IsLevelMenu = false;

      _pauseMenu.SetActive(true);
      _levelCompleteMenu.SetActive(true);

      tempAmountFoodCollected = 0;
    }

    public void MenuLevel()
    {
      GetLastLevelData();

      _gridLevel.CreatingLevelGrid();
      LevelCompleted = true;
      IsLevelMenu = true;
    }

    private void GetLastLevelData()
    {
      ProgressData progress = gameManager.ProgressData;
      LevelData oldLevelData = _currentLevelData;

      _currentLevelData = Levels.GetLevelData(progress.LocationLastLevelPlayed, progress.IndexLastLevelPlayed);
      _currentLevelData ??= oldLevelData;
    }

    public void SkinReplace()
    {
      _gridLevel.SkinReplace();
    }

    public void ExitMenu()
    {
      if (IsLevelMenu)
        return;

      GetLastLevelData();
      _gridLevel.CreatingLevelGrid();
      IsLevelMenu = true;

      _pauseMenu.SetActive(false);
      _levelCompleteMenu.SetActive(false);

      isCameraRotation = true;

      IsMenu?.Invoke();

      PanelController.Instance.CloseAllPanels();
      PanelController.Instance.SetActivePanel(_menuPanel);

      IsLevelRunning = false;
    }

    private void OpenNextLevel()
    {
      gameManager.ProgressData.SaveProgressLevelData(_currentLevelProgressData, _currentLevelData.Location, _currentLevelData.LevelNumber);

      gameManager.ProgressData.OpenNextLevel(_currentLevelData.Location, _currentLevelData.LevelNumber);

      if (gameManager.Achievements != null)
      {
        gameManager.Achievements.UpdateAchivementLevels();
        gameManager.Achievements.UpdateAchivementChapter();
      }

      //Test();

      gameManager.SaveData();
    }

    //======================================

    /*public int test = 1;
    public Location loc;

    private void Test_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
      gameManager.ProgressData.OpenNextLevel(loc, test);

      test++;

      Test();
    }

    private void Test()
    {
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 24)
        Debug.Log($"4-24");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 23)
        Debug.Log($"4-23");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 18)
        Debug.Log($"4-18");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 13)
        Debug.Log($"4-13");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 8)
        Debug.Log($"4-8");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 3)
        Debug.Log($"4-3");

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 22)
        Debug.Log($"3-22");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 17)
        Debug.Log($"3-17");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 12)
        Debug.Log($"3-12");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 7)
        Debug.Log($"3-7");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 2)
        Debug.Log($"3-2");

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 21)
        Debug.Log($"2-21");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 16)
        Debug.Log($"2-16");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 11)
        Debug.Log($"2-11");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 6)
        Debug.Log($"2-6");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 1)
        Debug.Log($"2-1");

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 20)
        Debug.Log($"1-20");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 15)
        Debug.Log($"1-15");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 10)
        Debug.Log($"1-10");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 5)
        Debug.Log($"1-5");
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 1)
        Debug.Log($"1-1");
    }*/
  }
}