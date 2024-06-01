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
      if (gameManager.ProgressData.GetNumberLevelsCompleted(Location.Chapter_1) >= 15)
        UpdateAchivement(Achievement.CHAPTER_1);
    }
    
    public void UpdateAchivementLevels()
    {
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
      if (gameManager.ProgressData.TotalNumberMoves >= 2000)
        UpdateAchivement(Achievement.MOVE_2000);
      if (gameManager.ProgressData.TotalNumberMoves >= 1500)
        UpdateAchivement(Achievement.MOVE_1500);
      if (gameManager.ProgressData.TotalNumberMoves >= 1000)
        UpdateAchivement(Achievement.MOVE_1000);
      if (gameManager.ProgressData.TotalNumberMoves >= 750)
        UpdateAchivement(Achievement.MOVE_750);
      if (gameManager.ProgressData.TotalNumberMoves >= 500)
        UpdateAchivement(Achievement.MOVE_500);
      if (gameManager.ProgressData.TotalNumberMoves >= 100)
        UpdateAchivement(Achievement.MOVE_100);
      if (gameManager.ProgressData.TotalNumberMoves >= 25)
        UpdateAchivement(Achievement.MOVE_25);
      if (gameManager.ProgressData.TotalNumberMoves >= 1)
        UpdateAchivement(Achievement.MOVE_1);
    }

    public void UpdateAchivementMoveBox()
    {
      if (gameManager.ProgressData.TotalNumberMovesBox >= 1000)
        UpdateAchivement(Achievement.MOVE_BOX_1000);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 750)
        UpdateAchivement(Achievement.MOVE_BOX_750);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 500)
        UpdateAchivement(Achievement.MOVE_BOX_500);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 400)
        UpdateAchivement(Achievement.MOVE_BOX_400);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 300)
        UpdateAchivement(Achievement.MOVE_BOX_300);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 200)
        UpdateAchivement(Achievement.MOVE_BOX_200);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 100)
        UpdateAchivement(Achievement.MOVE_BOX_100);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 50)
        UpdateAchivement(Achievement.MOVE_BOX_50);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 15)
        UpdateAchivement(Achievement.MOVE_BOX_15);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 5)
        UpdateAchivement(Achievement.MOVE_BOX_5);
      if (gameManager.ProgressData.TotalNumberMovesBox >= 1)
        UpdateAchivement(Achievement.MOVE_BOX_1);
    }

    public void UpdateAchivementBuySkin()
    {
      /*if (gameManager.ProgressData.PurchasedSkins.Count >= 10)
      {
        UpdateAchivement(Achievement.BUY_SKIN_10);
        UpdateAchivement(Achievement.BUY_ALL_SKINS);
      }
      if (gameManager.ProgressData.PurchasedSkins.Count >= 9)
        UpdateAchivement(Achievement.BUY_SKIN_9);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 8)
        UpdateAchivement(Achievement.BUY_SKIN_8);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 7)
        UpdateAchivement(Achievement.BUY_SKIN_7);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 6)
        UpdateAchivement(Achievement.BUY_SKIN_6);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 5)
        UpdateAchivement(Achievement.BUY_SKIN_5);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 4)
        UpdateAchivement(Achievement.BUY_SKIN_4);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 3)
        UpdateAchivement(Achievement.BUY_SKIN_3);
      if (gameManager.ProgressData.PurchasedSkins.Count >= 2)
        UpdateAchivement(Achievement.BUY_SKIN_2);*/

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
      if (gameManager.ProgressData.TotalFoodCollected >= 500)
        UpdateAchivement(Achievement.COLLECT_EAT_500);
      if (gameManager.ProgressData.TotalFoodCollected >= 400)
        UpdateAchivement(Achievement.COLLECT_EAT_400);
      if (gameManager.ProgressData.TotalFoodCollected >= 300)
        UpdateAchivement(Achievement.COLLECT_EAT_300);
      if (gameManager.ProgressData.TotalFoodCollected >= 200)
        UpdateAchivement(Achievement.COLLECT_EAT_200);
      if (gameManager.ProgressData.TotalFoodCollected >= 100)
        UpdateAchivement(Achievement.COLLECT_EAT_100);
      if (gameManager.ProgressData.TotalFoodCollected >= 75)
        UpdateAchivement(Achievement.COLLECT_EAT_75);
      if (gameManager.ProgressData.TotalFoodCollected >= 50)
        UpdateAchivement(Achievement.COLLECT_EAT_50);
      if (gameManager.ProgressData.TotalFoodCollected >= 25)
        UpdateAchivement(Achievement.COLLECT_EAT_25);
      if (gameManager.ProgressData.TotalFoodCollected >= 15)
        UpdateAchivement(Achievement.COLLECT_EAT_15);
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