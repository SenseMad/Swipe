using System.IO;
using UnityEngine;

using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.SaveLoad;
using Alekrus.UnivarsalPlatform.Utilities;

namespace Assets.Scripts.Save
{
  public class PC_UserSaveLoad : IUserSaveLoad
  {
    public static string PATH = $"{Application.persistentDataPath}/SaveData.dat";

    //======================================

    public int SaveDataMaxSize => int.MaxValue;

    public SaveState State { get; private set; }

    public ILocalUserId TargetUserId { get; }

    public GameSlotDetails CurrentSlotDetails { get; private set; }

    public byte[] CurrentData { get; private set; }

    public bool IsInitialized { get; private set; }

    public ISaveLoad ParentInterface { get; private set; }

    //======================================

    public event GameSavedEventHandler GameSaved;
    public event GameLoadedEventHandler GameLoaded;
    public event GameDeletedEventHandler GameDeleted;
    public event CanceledEventHandler Canceled;
    public event InitializedEventHandler Initialized;
    public event ShutdownedEventHandler Shutdowned;

    //======================================

    internal PC_UserSaveLoad(ILocalUserId parILocalUserId)
    {
      TargetUserId = parILocalUserId;
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

    public bool Save(byte[] parData, GameSlotDetails parDetails)
    {
      State = SaveState.SaveFile;

      File.WriteAllBytes(PATH, parData);

      State = SaveState.None;

      GameSaved?.Invoke(this, new SaveLoadReceivedArgs(TargetUserId, new PC_Result(true)));

      return true;
    }

    public bool CanSave(byte[] parData, GameSlotDetails parDetails)
    {
      return IsInitialized && parData.Length <= SaveDataMaxSize && State == SaveState.None;
    }

    public bool Load()
    {
      if (!File.Exists(PATH))
        return false;

      State = SaveState.LoadFile;

      CurrentData = File.ReadAllBytes(PATH);

      State = SaveState.None;

      GameLoaded?.Invoke(this, new SaveLoadReceivedArgs(TargetUserId, new PC_Result(true)));

      return true;
    }

    public bool CanLoad()
    {
      return IsInitialized && State == SaveState.None;
    }

    public bool Delete()
    {
      if (!IsInitialized)
        return false;
      if (State != SaveState.None)
        return false;
      if (!Exists())
        return false;

      State = SaveState.DeleteFile;

      PlatformDebugging.Log(GetType(), $"Game Delete : UserId = {TargetUserId}");

      File.Delete(PATH);

      State = SaveState.None;

      GameDeleted?.Invoke(this, new SaveLoadReceivedArgs(TargetUserId, new PC_Result(true)));

      return true;
    }

    public bool CanDelete()
    {
      return IsInitialized && State == SaveState.None;
    }

    public bool Exists()
    {
      if (!IsInitialized)
        return false;

      return File.Exists(PATH);
    }

    public bool GetDetails(out GameSlotDetails outDetails)
    {
      outDetails = CurrentSlotDetails;
      return false;
    }

    //======================================
  }
}