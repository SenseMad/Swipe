using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using Sokoban.GameManagement;
using Alekrus.UnivarsalPlatform.SaveLoad;
using Alekrus.UnivarsalPlatform;
using Sony.PS4.SavedGame;
using UnityEngine.SocialPlatforms.Impl;

namespace Sokoban.Save
{
  public sealed class SaveLoadManager
  {
    public static string PATH = $"{Application.persistentDataPath}/SaveData.dat";

    private ISaveLoad saveLoad;

    private ILocalUserId LocalUser => GameManager.Instance.PlatformManager.LocalUserProfiles.GetPrimaryLocalUserId();

    //======================================

    public void Initialize()
    {
      saveLoad = GameManager.Instance.PlatformManager.SaveLoad;
    }

    //======================================

    public void SaveData()
    {
      if (saveLoad == null)
        return;

      GameData data = GameData();

      MemoryStream memoryStream = new();
      BinaryFormatter binaryFormatter = new();
      binaryFormatter.Serialize(memoryStream, data);

      GameSlotDetails gameSlotDetails = new()
      {
        Title = "Sokoban SaveData",
        SubTitle = "Your game progress",
        Detail = "This is where your suffering is stored!"
      };

      saveLoad.Save(LocalUser, memoryStream.GetBuffer(), gameSlotDetails);
    }

    public void LoadData()
    {
      if (saveLoad == null)
        return;

      saveLoad.GameLoaded += UserSaveLoad_GameLoaded;

      saveLoad.Load(LocalUser);
    }

    private void UserSaveLoad_GameLoaded(LoadedReceivedArgs parArgs)
    {
      saveLoad.GameLoaded -= UserSaveLoad_GameLoaded;

      MemoryStream input = new(parArgs.Data);
      BinaryFormatter binaryFormatter = new();
      GameData data = (GameData)binaryFormatter.Deserialize(input);

      SetGameData(data);
    }

    //======================================

    public void ResetAndSaveFile()
    {
      ResetGameData();
      SaveData();
    }

    private void ResetGameData()
    {
      GameManager gameManager = GameManager.Instance;

      SettingsData settingsData = gameManager.SettingsData;

      GameData gameData = new()
      {
        MusicValue = settingsData.MusicValue,
        SoundValue = settingsData.SoundValue,
        FullScreenValue = settingsData.FullScreenValue,
        VSyncValue = settingsData.VSyncValue,
        CurrentLanguage = settingsData.CurrentLanguage
      };

      SetGameData(gameData);
    }

    //======================================

    public GameData GameData()
    {
      GameManager gameManager = GameManager.Instance;

      ProgressData progressData = gameManager.ProgressData;
      SettingsData settingsData = gameManager.SettingsData;

      return new GameData()
      {
        #region Settings

        MusicValue = settingsData.MusicValue,
        SoundValue = settingsData.SoundValue,
        FullScreenValue = settingsData.FullScreenValue,
        VSyncValue = settingsData.VSyncValue,
        CurrentLanguage = settingsData.CurrentLanguage,

        #endregion

        #region ProgressData

        NumberCompletedLevelsLocation = progressData.NumberCompletedLevelsLocation,
        LevelProgressData = progressData.LevelProgressData,

        CurrentActiveIndexSkin = progressData.CurrentActiveIndexSkin,
        LocationLastLevelPlayed = progressData.LocationLastLevelPlayed,
        IndexLastLevelPlayed = progressData.IndexLastLevelPlayed,
        AmountFoodCollected = progressData.AmountFoodCollected,
        TotalFoodCollected = progressData.TotalFoodCollected,
        PurchasedSkins = progressData.PurchasedSkins,
        TotalNumberMoves = progressData.TotalNumberMoves,
        TotalNumberMovesBox = progressData.TotalNumberMovesBox

        #endregion
      };
    }

    public void SetGameData(GameData parData)
    {
      GameManager gameManager = GameManager.Instance;

      ProgressData progressData = gameManager.ProgressData;
      SettingsData settingsData = gameManager.SettingsData;

      #region Settings

      settingsData.MusicValue = parData.MusicValue;
      settingsData.SoundValue = parData.SoundValue;
      settingsData.FullScreenValue = parData.FullScreenValue;
      settingsData.VSyncValue = parData.VSyncValue;
      settingsData.CurrentLanguage = parData.CurrentLanguage;

      #endregion

      #region ProgressData

      progressData.NumberCompletedLevelsLocation = parData.NumberCompletedLevelsLocation;
      progressData.LevelProgressData = parData.LevelProgressData ?? new Dictionary<Location, Dictionary<int, LevelManagement.LevelProgressData>>();
      
      progressData.CurrentActiveIndexSkin = parData.CurrentActiveIndexSkin;
      progressData.LocationLastLevelPlayed = parData.LocationLastLevelPlayed;
      progressData.IndexLastLevelPlayed = parData.IndexLastLevelPlayed;
      progressData.AmountFoodCollected = parData.AmountFoodCollected;
      progressData.TotalFoodCollected = parData.TotalFoodCollected;
      progressData.PurchasedSkins = parData.PurchasedSkins;
      progressData.TotalNumberMoves = parData.TotalNumberMoves;
      progressData.TotalNumberMovesBox = parData.TotalNumberMovesBox;

      #endregion
    }

    //======================================
  }
}