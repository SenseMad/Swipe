using UnityEngine;

using Sokoban.GameManagement;

namespace Sokoban.Achievement
{
  public sealed class Achievements : SingletonInGame<Achievements>
  {
    private GameManager gameManager;

    //======================================

    protected override void Awake()
    {
      base.Awake();

      gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
      gameManager.ProgressData.OnTotalNumberMoves += UpdateAchivementMove;

      gameManager.ProgressData.OnTotalNumberMovesBox += UpdateAchivementMoveBox;

      gameManager.ProgressData.OnTotalFoodCollected += UpdateAchivementFood;
    }

    private void OnDisable()
    {
      gameManager.ProgressData.OnTotalNumberMoves -= UpdateAchivementMove;

      gameManager.ProgressData.OnTotalNumberMovesBox -= UpdateAchivementMoveBox;

      gameManager.ProgressData.OnTotalFoodCollected -= UpdateAchivementFood;
    }
    
    //======================================

    public void UpdateAchivementChapter()
    {
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) >= 25)
        UpdateAchivement(Achievement.CHAPTER_4);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) >= 25)
        UpdateAchivement(Achievement.CHAPTER_3);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) >= 25)
        UpdateAchivement(Achievement.CHAPTER_2);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 25)
        UpdateAchivement(Achievement.CHAPTER_1);
    }
    
    public void UpdateAchivementLevels()
    {
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) + 1 >= 25)
        UpdateAchivement(Achievement.LEVEL_100);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) + 1 >= 20)
        UpdateAchivement(Achievement.LEVEL_95);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) + 1 >= 15)
        UpdateAchivement(Achievement.LEVEL_90);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) + 1 >= 10)
        UpdateAchivement(Achievement.LEVEL_85);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_4) + 1 >= 5)
        UpdateAchivement(Achievement.LEVEL_80);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) + 1 >= 25)
        UpdateAchivement(Achievement.LEVEL_75);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) + 1 >= 20)
        UpdateAchivement(Achievement.LEVEL_70);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) + 1 >= 15)
        UpdateAchivement(Achievement.LEVEL_65);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) + 1 >= 10)
        UpdateAchivement(Achievement.LEVEL_60);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_3) + 1 >= 5)
        UpdateAchivement(Achievement.LEVEL_55);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) + 1 >= 25)
        UpdateAchivement(Achievement.LEVEL_50);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) + 1 >= 20)
        UpdateAchivement(Achievement.LEVEL_45);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) + 1 >= 15)
        UpdateAchivement(Achievement.LEVEL_40);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) + 1 >= 10)
        UpdateAchivement(Achievement.LEVEL_35);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_2) + 1 >= 5)
        UpdateAchivement(Achievement.LEVEL_30);

      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) + 1 >= 25)
        UpdateAchivement(Achievement.LEVEL_25);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 20)
        UpdateAchivement(Achievement.LEVEL_20);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 15)
        UpdateAchivement(Achievement.LEVEL_15);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 10)
        UpdateAchivement(Achievement.LEVEL_10);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 5)
        UpdateAchivement(Achievement.LEVEL_5);
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 1)
        UpdateAchivement(Achievement.LEVEL_1);
    }

    public void UpdateAchivementMove()
    {
      if (gameManager.ProgressData.TotalNumberMoves >= 250)
        UpdateAchivement(Achievement.MOVE_250);
      if (gameManager.ProgressData.TotalNumberMoves >= 200)
        UpdateAchivement(Achievement.MOVE_200);
      if (gameManager.ProgressData.TotalNumberMoves >= 150)
        UpdateAchivement(Achievement.MOVE_150);
      if (gameManager.ProgressData.TotalNumberMoves >= 100)
        UpdateAchivement(Achievement.MOVE_100);
      if (gameManager.ProgressData.TotalNumberMoves >= 50)
        UpdateAchivement(Achievement.MOVE_50);
      if (gameManager.ProgressData.TotalNumberMoves >= 25)
        UpdateAchivement(Achievement.MOVE_25);
      if (gameManager.ProgressData.TotalNumberMoves >= 10)
        UpdateAchivement(Achievement.MOVE_10);
      if (gameManager.ProgressData.TotalNumberMoves >= 1)
        UpdateAchivement(Achievement.MOVE_1);
    }

    public void UpdateAchivementMoveBox()
    {

    }

    public void UpdateAchivementBuySkin()
    {
      if (gameManager.ProgressData.PurchasedSkins.Contains(9))
        UpdateAchivement(Achievement.BUY_SKIN_10);
      if (gameManager.ProgressData.PurchasedSkins.Contains(8))
        UpdateAchivement(Achievement.BUY_SKIN_9);
      if (gameManager.ProgressData.PurchasedSkins.Contains(7))
        UpdateAchivement(Achievement.BUY_SKIN_8);
      if (gameManager.ProgressData.PurchasedSkins.Contains(6))
        UpdateAchivement(Achievement.BUY_SKIN_7);
      if (gameManager.ProgressData.PurchasedSkins.Contains(5))
        UpdateAchivement(Achievement.BUY_SKIN_6);
      if (gameManager.ProgressData.PurchasedSkins.Contains(4))
        UpdateAchivement(Achievement.BUY_SKIN_5);
      if (gameManager.ProgressData.PurchasedSkins.Contains(3))
        UpdateAchivement(Achievement.BUY_SKIN_4);
      if (gameManager.ProgressData.PurchasedSkins.Contains(2))
        UpdateAchivement(Achievement.BUY_SKIN_3);
      if (gameManager.ProgressData.PurchasedSkins.Contains(1))
        UpdateAchivement(Achievement.BUY_SKIN_2);
    }

    public void UpdateAchivementFood()
    {
      if (gameManager.ProgressData.TotalFoodCollected >= 85)
        UpdateAchivement(Achievement.COLLECT_EAT_85);
      if (gameManager.ProgressData.TotalFoodCollected >= 75)
        UpdateAchivement(Achievement.COLLECT_EAT_75);
      if (gameManager.ProgressData.TotalFoodCollected >= 65)
        UpdateAchivement(Achievement.COLLECT_EAT_65);
      if (gameManager.ProgressData.TotalFoodCollected >= 55)
        UpdateAchivement(Achievement.COLLECT_EAT_55);
      if (gameManager.ProgressData.TotalFoodCollected >= 45)
        UpdateAchivement(Achievement.COLLECT_EAT_45);
      if (gameManager.ProgressData.TotalFoodCollected >= 35)
        UpdateAchivement(Achievement.COLLECT_EAT_35);
      if (gameManager.ProgressData.TotalFoodCollected >= 25)
        UpdateAchivement(Achievement.COLLECT_EAT_25);
      if (gameManager.ProgressData.TotalFoodCollected >= 15)
        UpdateAchivement(Achievement.COLLECT_EAT_15);
      if (gameManager.ProgressData.TotalFoodCollected >= 10)
        UpdateAchivement(Achievement.COLLECT_EAT_10);
      if (gameManager.ProgressData.TotalFoodCollected >= 5)
        UpdateAchivement(Achievement.COLLECT_EAT_5);
      if (gameManager.ProgressData.TotalFoodCollected >= 1)
        UpdateAchivement(Achievement.COLLECT_EAT_1);
    }

    //======================================

    private void UpdateAchivement(Achievement parAchievement)
    {
      if (gameManager.PlatformManager.Achievements != null &&
        gameManager.PlatformManager.LocalUserProfiles != null)
      {
        var userId = gameManager.PlatformManager.LocalUserProfiles.GetPrimaryLocalUserId();

        var progress = gameManager.PlatformManager.Achievements.GetAchievementProgress(userId, parAchievement);
        if (progress != null)
        {
          if (!progress.IsUnlocked)
          {
            Debug.Log($"UpdateAchivement({parAchievement}) : LocalUserId={userId}");
            gameManager.PlatformManager.Achievements.UnlockAchievement(userId, parAchievement);
          }
          else
          {
            Debug.LogWarning($"UpdateAchivement({parAchievement}) : Achievement already unlocked! LocalUserId={userId}");
          }
        }
        else
        {
          Debug.LogWarning($"UpdateAchivement({parAchievement}) : No progress has been made for the player! LocalUserId={userId}");
        }
      }
      else
      {
        Debug.LogError($"Error UpdateAchivement({parAchievement}): " +
          $"PlatformManager = {gameManager.PlatformManager}, " +
          $"Achievements = {gameManager.PlatformManager?.Achievements}, " +
          $"LocalUserProfiles = {gameManager.PlatformManager?.LocalUserProfiles} ");
      }
    }

    //======================================
  }
}