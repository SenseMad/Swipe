using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sokoban.GridEditor
{
  public class UIBlockTypeSelectButton : MonoBehaviour
  {
    [SerializeField, Tooltip("Image �������")]
    private Image _imageObject;

    [SerializeField, Tooltip("����� ������� �������")]
    private TextMeshProUGUI _textNameObject;

    //======================================

    public Button Button { get; set; }

    public Image Image { get; set; }

    public TypeObject TypeObject { get; set; }

    //======================================

    private void Awake()
    {
      Image = GetComponent<Image>();
    }

    //======================================

    /// <summary>
    /// ������������� ������
    /// </summary>
    public void InitializeButton(TypeObject parTypeObject, Sprite parSprite, string parText)
    {
      TypeObject = parTypeObject;
      _imageObject.sprite = parSprite;
      _textNameObject.text = parText;
    }

    //======================================

    /// <summary>
    /// �������� ����
    /// </summary>
    public void ChangeColor(bool parValue)
    {
      if (parValue)
        Image.color = ColorsGame.SELECTED_COLOR;
      else
        Image.color = ColorsGame.STANDART_COLOR;
    }

    //======================================
  }
}