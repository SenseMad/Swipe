using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using Sokoban.LevelManagement;
using Sokoban.Achievement;
using Sokoban.Save;

namespace Sokoban.GameManagement
{
  public sealed class GameManager : SingletonInGame<GameManager>
  {
    private TransitionBetweenScenes transitionBetweenScenes;

    //======================================

    public PlatformManager PlatformManager { get; private set; }

    public Achievements Achievements { get; private set; }

    public SaveLoadManager SaveLoadManager { get; private set; }

    public ProgressData ProgressData { get; set; }

    public SettingsData SettingsData { get; set; }

    //======================================

    protected override void Awake()
    {
      base.Awake();

      transitionBetweenScenes = FindAnyObjectByType<TransitionBetweenScenes>();

      StartCoroutine(Init());
    }

    private void Start()
    {
      Levels.GetFullNumberLevelsLocation();

#if UNITY_PS4
      Screen.fullScreen = true;
#else
      SettingsData.CreateResolutions();

      SettingsData.ApplyResolution();
#endif
    }

#if UNITY_PS4
    private void Update()
    {
      PlatformManager.Main.Update();
      PlatformManager.SaveLoad.Update();
    }
#endif

    private void OnApplicationQuit()
    {
#if UNITY_PS4
      PlatformManager.Main.Shutdown();
#else
      SaveData();
#endif
    }

#if !UNITY_PS4
    private void OnApplicationPause(bool pause)
    {
      if (pause)
        SaveData();
      else
        LoadData();
    }
#endif

    //======================================

    private IEnumerator Init()
    {
      bool initScene = SceneManager.GetActiveScene().name == "InitScene";

      SaveLoadManager = new SaveLoadManager();

      ProgressData = new();
      SettingsData = new();

      SettingsData.CurrentLanguage = Language.English;

      PlatformManager = new PlatformManager();

      PlatformManager.Initialize();

      yield return new WaitUntil(() => PlatformManager.IsInitialized);

      InstallingSystemLanguage();

      SaveLoadManager.Initialize();

      LoadData();

      yield return new WaitUntil(() => PlatformManager.SaveLoad.IsInitialized);

      Achievements = Achievements.Instance;

      if (initScene)
      {
        transitionBetweenScenes.StartSceneChange("GameScene");
      }
    }

    private void InstallingSystemLanguage()
    {
      switch (PlatformManager.Main.SystemLanguage)
      {
        case Alekrus.UnivarsalPlatform.SystemLanguage.JAPANESE:
          SettingsData.CurrentLanguage = Language.Japan;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.FRENCH:
        case Alekrus.UnivarsalPlatform.SystemLanguage.FRENCH_CA:
          SettingsData.CurrentLanguage = Language.French;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.SPANISH:
        case Alekrus.UnivarsalPlatform.SystemLanguage.SPANISH_LA:
          SettingsData.CurrentLanguage = Language.Spanish;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.GERMAN:
          SettingsData.CurrentLanguage = Language.German;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.RUSSIAN:
          SettingsData.CurrentLanguage = Language.Russian;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.CHINESE_T:
        case Alekrus.UnivarsalPlatform.SystemLanguage.CHINESE_S:
          SettingsData.CurrentLanguage = Language.Chinese;
          break;

        case Alekrus.UnivarsalPlatform.SystemLanguage.PORTUGUESE_PT:
        case Alekrus.UnivarsalPlatform.SystemLanguage.PORTUGUESE_BR:
          SettingsData.CurrentLanguage = Language.Portuguese;
          break;

        default:
          SettingsData.CurrentLanguage = Language.English;
          break;
      }
    }

    //======================================

    public void SaveData()
    {
      SaveLoadManager.SaveData();
    }

    public void ResetAndSaveFile()
    {
      SaveLoadManager.ResetAndSaveFile();
    }

    private void LoadData()
    {
      SaveLoadManager.LoadData();
    }

    //======================================
  }
}