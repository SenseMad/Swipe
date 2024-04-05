using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Sokoban.LevelManagement;

namespace Sokoban.UI
{
  /// <summary>
  /// Пользовательский интерфейс кнопка выбора уровня
  /// </summary>
  public class UILevelSelectButton : MonoBehaviour
  {
    [SerializeField, Tooltip("Текст названия уровня")]
    private TextMeshProUGUI _textLevelName;

    [SerializeField, Tooltip("Image замка")]
    private Image _imageLock;

    [SerializeField, Tooltip("Спрайт стандартный")]
    private Sprite _spriteStandart;
    [SerializeField, Tooltip("Спрайт выделения")]
    private Sprite _spriteSelected;

    [SerializeField] private GameObject _selectImage;

    //--------------------------------------

    private Image imageButton;

    //private Animator animator;

    //======================================

    public Button Button { get; set; }

    /// <summary>
    /// True, если уровень открыт
    /// </summary>
    //public bool IsLevelOpen { get; private set; }

    //======================================

    private void Awake()
    {
      imageButton = GetComponent<Image>();

      //animator = GetComponent<Animator>();
    }

    //======================================

    /// <summary>
    /// Инициализация кнопки выбора локации
    /// </summary>
    public void Initialize(LevelData levelData)
    {
      Button.name = $"{levelData.LevelNumber}";
      _textLevelName.text = $"{levelData.LevelNumber}";
    }

    /// <summary>
    /// Изменить цвет
    /// </summary>
    public void ChangeColor()
    {
      _textLevelName.color = ColorsGame.STANDART_COLOR;
      //imageButton.color = ColorsGame.SELECTED_COLOR_BLACK;

      //IsLevelOpen = true;
      _imageLock.gameObject.SetActive(false);
      _textLevelName.gameObject.SetActive(true);
    }

    public void ChangeSprite(bool parValue)
    {
      imageButton.sprite = parValue ? _spriteSelected : _spriteStandart;

      _selectImage.SetActive(parValue);

      //animator.enabled = parValue;
    }

    //======================================
  }
}