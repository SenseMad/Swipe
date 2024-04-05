using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.UserProfiles;
using Alekrus.UnivarsalPlatform.Achievements;
using Alekrus.UnivarsalPlatform.SaveLoad;

public class PlatformManager : IInitializable
{
  public IMain Main { get; private set; }

  public ILocalUserProfiles LocalUserProfiles { get; private set; }

  public IAchievements Achievements { get; private set; }

  public ISaveLoad SaveLoad { get; private set; }

  //======================================

  public event InitializedEventHandler Initialized;
  public event ShutdownedEventHandler Shutdowned;

  //======================================

  public bool IsInitialized => Achievements.IsInitialized;

  //======================================

  public bool Initialize()
  {
    Main = MainProvider.Create();

    Main.Initialized += Main_Initialized;

    return Main.Initialize();
  }

  public bool Shutdown() => Main.Shutdown();

  //======================================

  private void Main_Initialized()
  {
    Main.Initialized -= Main_Initialized;

    LocalUserProfiles = Main.GetSubInterface<ILocalUserProfiles>();

    LocalUserProfiles.Initialized += LocalUserProfiles_Initialized;

    LocalUserProfiles.Initialize();
  }

  private void LocalUserProfiles_Initialized()
  {
    LocalUserProfiles.Initialized -= LocalUserProfiles_Initialized;

    Achievements = Main.GetSubInterface<IAchievements>();
    SaveLoad = Main.GetSubInterface<ISaveLoad>();

    Achievements.Initialized += Achievements_Initialized;
    SaveLoad.Initialized += SaveLoad_Initialized;

    Achievements.Initialize();
    SaveLoad.Initialize();
  }

  private void Achievements_Initialized()
  {
    Achievements.Initialized -= Achievements_Initialized;

    Achievements.RequestAchievementsInfo(LocalUserProfiles.GetPrimaryLocalUserId());
  }

  private void SaveLoad_Initialized()
  {
    SaveLoad.Initialized -= SaveLoad_Initialized;
  }

  //======================================
}