using System.IO;
using UnityEngine;

using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.SaveLoad;
using Alekrus.UnivarsalPlatform.Utilities;

namespace Assets.Scripts.Save
{
  public class PC_SaveLoad : ISaveLoad
  {
    public static string PATH = $"{Application.persistentDataPath}/SaveData.dat";

    //======================================

    private SaveState state;

    //======================================

    public int SaveDataMaxSize => int.MaxValue;

    public bool IsInitialized { get; private set; }

    public IMain ParentInterface { get; private set; }

    //======================================

    public event InitializedEventHandler Initialized;
    public event ShutdownedEventHandler Shutdowned;
    public event GameSavedEventHandler GameSaved;
    public event GameLoadedEventHandler GameLoaded;
    public event GameDeletedEventHandler GameDeleted;

    //======================================

    public PC_SaveLoad(IMain parIMain)
    {
      ParentInterface = parIMain;
    }

    //======================================

    public void Update() { }

    public bool Initialize()
    {
      if (IsInitialized) { return false; }

      IsInitialized = true;
      PlatformDebugging.Log(GetType(), "Initialized");
      Initialized?.Invoke();

      return true;
    }

    public bool Shutdown()
    {
      if (!IsInitialized) { return false; }

      IsInitialized = false;
      PlatformDebugging.Log(GetType(), "Shutdowned");
      Shutdowned?.Invoke();

      return true;
    }

    //======================================

    public bool Save(ILocalUserId parUserId, byte[] parData, GameSlotDetails parDetails)
    {
      state = SaveState.SaveFile;
      File.WriteAllBytes(PATH, parData);

      state = SaveState.None;

      GameSaved?.Invoke(new SavedReceivedArgs(parUserId, new PC_Result(true)));

      return true;
    }

    public bool CanSave(ILocalUserId parUserId, byte[] parData, GameSlotDetails parDetails)
    {
      return IsInitialized && parData.Length <= SaveDataMaxSize && state == SaveState.None;
    }

    public bool Load(ILocalUserId parUserId)
    {
      if (!File.Exists(PATH))
        return false;

      state = SaveState.LoadFile;
      var currentData = File.ReadAllBytes(PATH);

      GameSlotDetails gameSlot = default;

      state = SaveState.None;

      GameLoaded?.Invoke(new LoadedReceivedArgs(parUserId, gameSlot, currentData, new PC_Result(true)));

      return true;
    }

    public bool CanLoad(ILocalUserId parUserId)
    {
      return IsInitialized && state == SaveState.None;
    }

    public bool Delete(ILocalUserId parUserId)
    {
      if (!IsInitialized)
        return false;
      if (state != SaveState.None)
        return false;
      if (!Exists(parUserId))
        return false;

      state = SaveState.DeleteFile;

      PlatformDebugging.Log(GetType(), $"Game Delete : UserId = {parUserId}");
      File.Delete(PATH);

      state = SaveState.None;

      GameDeleted?.Invoke(new DeletedReceivedArgs(parUserId, new PC_Result(true)));

      return true;
    }

    public bool CanDelete(ILocalUserId parUserId)
    {
      return IsInitialized && state == SaveState.None;
    }

    public SaveState GetState(ILocalUserId parUserId)
    {
      return state;
    }

    public bool GetDetails(ILocalUserId parUserId, out GameSlotDetails outDetails)
    {
      outDetails = default;
      return false;
    }

    public bool Exists(ILocalUserId parUserId)
    {
      if (!IsInitialized)
        return false;

      return File.Exists(PATH);
    }

    //======================================
  }
}