using Sokoban.GridEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : SingletonInGame<InputHandler>
{
  public AI_Player AI_Player { get; private set; }

#if UNITY_PS4
  public bool EnterButtonCross { get; private set; }
#endif

  //======================================

  protected override void Awake()
  {
    base.Awake();

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    AI_Player = new AI_Player();

#if UNITY_PS4
    // 0 means circle, 1 means cross.  See https://ps4.siedev.net/resources/documents/SDK/10.000/SystemService-Reference/0001.html
    EnterButtonCross = (Application.isEditor ? 1 : UnityEngine.PS4.Utility.GetSystemServiceParam(UnityEngine.PS4.Utility.SystemServiceParamId.EnterButtonAssign)) == 1;
    //string fullPath = "<PS4DualShockGamepad>/" + buttonName;
    if (!EnterButtonCross)
    {
      Debug.Log($"Enter button param is 0, adding binding (Select = <Gamepad>/buttonEast, Back = <Gamepad>/buttonSouth)");
      //Удаляем прошлый InputAction и добавляем новый
      AI_Player.UI.Select.ChangeBindingWithPath("<Gamepad>/buttonSouth").Erase();
      AI_Player.UI.Select.AddBinding("<Gamepad>/buttonEast");

      AI_Player.UI.Pause.ChangeBindingWithPath("<Gamepad>/buttonEast").Erase();
      AI_Player.UI.Pause.AddBinding("<Gamepad>/buttonSouth");
    }
#endif
  }

  private void OnEnable()
  {
    AI_Player.Enable();
  }

  private void OnDisable()
  {
    AI_Player.Disable();
  }

  //======================================

  /// <summary>
  /// True, если можно использовать ввод данных
  /// </summary>
  public bool CanProcessInput()
  {
    return AI_Player != null;
  }

  //======================================

  /// <summary>
  /// Получит движение
  /// </summary>
  public Vector2 GetMove()
  {
    return CanProcessInput() ? AI_Player.Player.Move.ReadValue<Vector2>() : Vector2.zero;
  }

  /// <summary>
  /// Получить поворот
  /// </summary>
  public Vector2 GetLook()
  {
    return CanProcessInput() ? AI_Player.Camera.Look.ReadValue<Vector2>() : Vector2.zero;
  }

  #region Камера

  /// <summary>
  /// Получить кнопку меделеного полета камеры
  /// </summary>
  public bool GetButtonSlowCamera()
  {
    return CanProcessInput() && AI_Player.Camera.SlowCameraSpeed.ReadValue<float>() == 1;
  }

  /// <summary>
  /// Получить кнопку быстрого полета камеры
  /// </summary>
  public bool GetButtonFastCamera()
  {
    return CanProcessInput() && AI_Player.Camera.FastCameraSpeed.ReadValue<float>() == 1;
  }

  #endregion

  #region UI

  /// <summary>
  /// Получить кнопки навигации (По вертикали)
  /// </summary>
  public float GetNavigationInput()
  {
    return AI_Player != null ? AI_Player.Player.Move.ReadValue<Vector2>().y : 0f;
  }

  /// <summary>
  /// Получить кнопки изменения значений (По горизонтали)
  /// </summary>
  public float GetChangingValuesInput()
  {
    return AI_Player != null ? AI_Player.Player.Move.ReadValue<Vector2>().x : 0f;
  }

  #endregion

  //======================================
}