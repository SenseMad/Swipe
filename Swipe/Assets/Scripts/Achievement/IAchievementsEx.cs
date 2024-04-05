using Sokoban.Achievement;

using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.Achievements;

public static class IAchievementsEx
{
  /*public static bool UnlockAchievement(this IAchievements parIAchievements, ILocalUserId parUserId, Achievement parAchievementId)
  {
    return parIAchievements.UnlockAchievement(parUserId, new AchievementId(parAchievementId.ToString()));
  }*/
  
  public static bool UnlockAchievement<TEnum>(this IAchievements parIAchievements, ILocalUserId parUserId, TEnum parAchievementId) where TEnum : struct, System.Enum
  {
    return parIAchievements.UnlockAchievement(parUserId, new AchievementId(parAchievementId.ToString()));
  }
}