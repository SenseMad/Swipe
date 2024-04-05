using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Sokoban.GameManagement;
using Sokoban.LevelManagement;

namespace Sokoban.UI
{
  public class UIShop : MenuUI
  {
    [SerializeField] private RectTransform _content;

    [SerializeField] private ShopButton _prefabShopButton;

    [SerializeField] private Panel _panel;

    [SerializeField] private GameObject _arrowLeft;
    [SerializeField] private GameObject _arrowRight;

    [SerializeField] private TextMeshProUGUI _foodCollected;

    //--------------------------------------

    private GameManager gameManager;

    private List<ShopButton> listShopButtons = new();

    //======================================

    protected override void Awake()
    {
      base.Awake();

      gameManager = GameManager.Instance;
    }

    protected override void OnEnable()
    {
      OnAmountFoodCollected(gameManager.ProgressData.AmountFoodCollected);

      indexActiveButton = gameManager.ProgressData.CurrentActiveIndexSkin;

      DisplayButtonsUI();

      base.OnEnable();

      gameManager.ProgressData.OnAmountFoodCollected += OnAmountFoodCollected;
    }

    protected override void OnDisable()
    {
      base.OnDisable();

      ClearButtonsUI();

      gameManager.ProgressData.OnAmountFoodCollected -= OnAmountFoodCollected;
    }

    protected override void Update()
    {
      MoveMenuHorizontally();
    }

    //======================================

    private void DisplayButtonsUI()
    {
      if (_listButtons.Count != 0)
        return;

      foreach (var skinData in ShopData.Instance.SkinDatas)
      {
        ShopButton shopButton = Instantiate(_prefabShopButton, _content);

        if (gameManager.ProgressData.PurchasedSkins.Contains(skinData.IndexSkin))
        {
          shopButton.UnSelect();
          if (gameManager.ProgressData.CurrentActiveIndexSkin == skinData.IndexSkin)
            shopButton.Select();
        }
        else
          shopButton.NotPurchased();

        shopButton.Button.onClick.AddListener(() => SelectSkin(skinData));

        _listButtons.Add(shopButton.Button);
        listShopButtons.Add(shopButton);

        shopButton.Initialize(skinData);
      }

      if (indexActiveButton - 1 < 0)
        _arrowLeft.SetActive(false);
      if (indexActiveButton + 1 > _listButtons.Count - 1)
        _arrowRight.SetActive(false);

      LocationElements();
    }

    public void ClearButtonsUI()
    {
      for (int i = 0; i < _listButtons.Count; i++)
      {
        Destroy(_listButtons[i].gameObject);
      }

      _listButtons = new();
      listShopButtons = new();
    }

    //======================================

    private void SelectSkin(SkinData parSkinData)
    {
      if (gameManager.ProgressData.PurchasedSkins.Contains(parSkinData.IndexSkin))
      {
        foreach (var oldShopButton in listShopButtons)
        {
          if (oldShopButton.IndexSkin != gameManager.ProgressData.CurrentActiveIndexSkin)
            continue;

          oldShopButton.UnSelect();

          gameManager.ProgressData.CurrentActiveIndexSkin = parSkinData.IndexSkin;

          foreach (var newShopButton in listShopButtons)
          {
            if (newShopButton.IndexSkin != gameManager.ProgressData.CurrentActiveIndexSkin)
              continue;

            newShopButton.Select();
            break;
          }

          break;
        }

        LevelManager.Instance.SkinReplace();

        gameManager.SaveData();
        return;
      }

      if (gameManager.ProgressData.AmountFoodCollected < parSkinData.PriceSkin)
        return;

      gameManager.ProgressData.AmountFoodCollected -= parSkinData.PriceSkin;
      gameManager.ProgressData.PurchasedSkins.Add(parSkinData.IndexSkin);

      listShopButtons[parSkinData.IndexSkin].UnSelect();

      gameManager.SaveData();
    }

    //======================================

    private void LocationElements()
    {
      for (int i = 0; i < _listButtons.Count; i++)
      {
        float scale = (i == indexActiveButton) ? 1.5f : 0.7f;

        _listButtons[i].transform.localScale = new Vector3(scale, scale, 1f);

        float xPosition = (i - indexActiveButton) * 400;

        _listButtons[i].transform.localPosition = new Vector3(xPosition, 0f, 0f);
      }
    }

    //======================================

    protected override void MoveMenuHorizontally()
    {
      if (_listButtons.Count == 0)
        return;

      if (Time.time > nextTimeMoveNextValue)
      {
        nextTimeMoveNextValue = Time.time + timeMoveNextValue;

        if (inputHandler.GetChangingValuesInput() > 0)
        {
          if (indexActiveButton + 1 > _listButtons.Count - 1)
            return;

          _arrowLeft.SetActive(true);
          IsSelectedButton = false;

          indexActiveButton++;
          LocationElements();

          Sound();
          IsSelectedButton = true;

          if (indexActiveButton + 1 > _listButtons.Count - 1)
            _arrowRight.SetActive(false);
        }

        if (inputHandler.GetChangingValuesInput() < 0)
        {
          if (indexActiveButton - 1 < 0)
          {
            return;
          }

          _arrowRight.SetActive(true);
          IsSelectedButton = false;

          indexActiveButton--;
          LocationElements();

          Sound();
          IsSelectedButton = true;

          if (indexActiveButton - 1 < 0)
            _arrowLeft.SetActive(false);
        }
      }

      if (inputHandler.GetChangingValuesInput() == 0)
      {
        nextTimeMoveNextValue = Time.time;
      }
    }

    //======================================

    private void OnAmountFoodCollected(int parValue)
    {
      _foodCollected.text = $"{parValue}";
    }

    //======================================
  }
}