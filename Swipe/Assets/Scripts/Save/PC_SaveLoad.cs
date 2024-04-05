using System.Collections.Generic;

using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.SaveLoad;
using Alekrus.UnivarsalPlatform.Utilities;

namespace Assets.Scripts.Save
{
  public class PC_SaveLoad : ISaveLoad
  {
    private readonly Dictionary<ILocalUserId, PC_UserSaveLoad> userSaveLoads = new();

    public bool IsInitialized { get; private set; }

    public IMain ParentInterface { get; }

    //======================================

    public event InitializedEventHandler Initialized;
    public event ShutdownedEventHandler Shutdowned;

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

    public IUserSaveLoad GetUserSaveLoad(ILocalUserId parTargetUserId)
    {
      if (!userSaveLoads.TryGetValue(parTargetUserId, out PC_UserSaveLoad userSaveLoad))
      {
        userSaveLoad = new PC_UserSaveLoad(parTargetUserId);
        userSaveLoads.Add(parTargetUserId, userSaveLoad);
      }

      return userSaveLoad;
    }

    //======================================
  }
}