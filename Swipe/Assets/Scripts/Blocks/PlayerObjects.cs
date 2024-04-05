using UnityEngine;
using UnityEngine.Events;

using Sokoban.LevelManagement;
using Sokoban.GridEditor;
using Sokoban.GameManagement;
using UnityEngine.XR;
using UnityEngine.UIElements;

public class PlayerObjects : Block
{
  [SerializeField] private float _speed = 2.0f;
  [SerializeField] private float _speedRotaion = 7.0f;

  [SerializeField] private Transform _meshTransform;

  [SerializeField] private AudioClip _soundMove;

  [SerializeField] private Transform _deathEffect;

  //--------------------------------------

  private Animator animator;

  private InputHandler inputHandler;

  private LevelManager levelManager;

  /// <summary>
  /// True, если игрок движется
  /// </summary>
  private bool isMoving = false;
  /// <summary>
  /// True, если игрок поворачивается
  /// </summary>
  private bool isPlayerRotation = false;

  /// <summary>
  /// Кнопка перемещения была нажата
  /// </summary>
  private bool isMoveButtonPressed = false;
  private Vector2 axisMove;

  /// <summary>
  /// Новая позиция
  /// </summary>
  private Vector3 lastPosition;
  /// <summary>
  /// Направление движения
  /// </summary>
  private Vector3 direction;

  /// <summary>
  /// Стоим на месте
  /// </summary>
  private bool isStandingStill = true;

  #region Камера

  /// <summary>
  /// True, если камера поворачивается
  /// </summary>
  private bool isCameraRotation = false;
  private float targetRotation;

  #endregion

  //======================================

  public Rigidbody Rigidbody { get; private set; }

  //======================================

  public UnityEvent OnPlayerDeath { get; } = new UnityEvent();

  //======================================

  protected override void Awake()
  {
    base.Awake();

    Rigidbody = GetComponent<Rigidbody>();

    animator = GetComponent<Animator>();

    inputHandler = InputHandler.Instance;

    levelManager = LevelManager.Instance;
  }

  private void Start()
  {
    if (GridEditor.GridEditorEnabled)
      RemoveRigidbody();

    if (!GridEditor.GridEditorEnabled)
      levelManager.CinemachineVirtual.Follow = transform;
  }

  private void OnEnable()
  {
    inputHandler.AI_Player.Player.Move.performed += Move_performed;

    if (!isCameraRotation)
    {
      inputHandler.AI_Player.Camera.RotationLeft.performed += parValue => CameraRotation(90);
      inputHandler.AI_Player.Camera.RotationRight.performed += parValue => CameraRotation(-90);
    }
  }

  private void OnDisable()
  {
    inputHandler.AI_Player.Player.Move.performed -= Move_performed;

    if (!isCameraRotation)
    {
      inputHandler.AI_Player.Camera.RotationLeft.performed -= parValue => CameraRotation(90);
      inputHandler.AI_Player.Camera.RotationRight.performed -= parValue => CameraRotation(-90);
    }
  }

  private void Update()
  {
    if (GridEditor.GridEditorEnabled)
      return;

    SmoothCameraRotation();
    SmoothPlayerRotation();

    if (!isMoving)
      return;

    transform.position = Vector3.MoveTowards(transform.position, lastPosition + direction, _speed * Time.deltaTime);

    if (transform.position == lastPosition + direction)
    {
      CheckGround(new Vector3Int((int)lastPosition.x, (int)lastPosition.y, (int)lastPosition.z));
      if (!IsBlocked(direction))
      {
        lastPosition = transform.position;
        return;
      }

      CheckGround(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z));
      isMoving = false;
    }
  }

  //======================================

  public void CheckGround(Vector3Int parPosition)
  {
    foreach (var ground in LevelManager.Instance.GridLevel.GetListGroundObjects())
    {
      var groundPosition = ground.GetObjectPosition();
      if (new Vector3Int(groundPosition.x, 2, groundPosition.z) != new Vector3Int(parPosition.x, parPosition.y - 1, parPosition.z))
        continue;

      if (ground.IsBlockActive)
        continue;

      ground.ChangeBlock();
      break;
    }
  }

  private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
  {
    if (levelManager.LevelCompleted || levelManager.IsPause || levelManager.GridLevel.TryStatesLevel() || levelManager.IsLevelMenu || isMoving)
      return;

    if (axisMove != Vector2.zero && isMoveButtonPressed)
      return;

    Vector2 axisMovement = inputHandler.GetMove();
    axisMovement.Normalize();

    isMoveButtonPressed = true;
    axisMove = axisMovement;

    var cinemachineVirtualDirection = levelManager.CinemachineVirtual.transform.TransformDirection(axisMove);
    Vector3 direction = new(cinemachineVirtualDirection.x, 0.0f, cinemachineVirtualDirection.z);

    isMoveButtonPressed = false;
    axisMove = Vector2.zero;

    if (!Move(direction))
      return;

    levelManager.NumberMoves++;

    GameManager.Instance.ProgressData.TotalNumberMoves++;

    if (_soundMove != null)
      AudioManager.Instance.OnPlaySound?.Invoke(_soundMove);
  }

  private bool Move(Vector3 parDirection)
  {
    if (levelManager.LevelCompleted || levelManager.IsPause || levelManager.GridLevel.TryStatesLevel() || levelManager.IsLevelMenu || isMoving)
      return false;

    if (Mathf.Abs(parDirection.x) < 0.5f)
      parDirection.x = 0;
    else
      parDirection.z = 0;

    parDirection.Normalize();

    if (IsBlocked(parDirection))
    {
      isMoveButtonPressed = false;
      axisMove = Vector2.zero;
      return false;
    }

    isMoving = true;
    isPlayerRotation = true;
    direction = parDirection;
    lastPosition = transform.position;

    return true;
  }

  /// <summary>
  /// Возвращает True, если перед игроком 2 и более ящика, блок который нельзя двигать, и т.д.
  /// </summary>
  /// <param name="parDirection">Направление движения</param>
  private bool IsBlocked(Vector3 parDirection)
  {
    if (!CheckGroundPlayer(parDirection))
      return true;

    if (Physics.Raycast(transform.position, parDirection, out RaycastHit hit, 1))
    {
      if (hit.collider)
      {
        Block block = hit.collider.GetComponent<Block>();
        if (block.GetTypeObject() == TypeObject.staticObject)
          return true;

        #region Проверка движущихся объектов

        if (hit.collider.TryGetComponent(out DynamicObjects dynamicObject))
        {
          Instantiate(_deathEffect, transform.position, transform.rotation);
          axisMove = Vector2.zero;
          LevelManager.Instance.ReloadLevelDeath();
          return true;
        }

        #endregion

        #region Проверка шипов

        if (hit.collider.TryGetComponent(out SpikeObject spikeObject))
        {
          gameObject.SetActive(false);
          return spikeObject.IsSpikeActivated;
        }

        #endregion

        if (block.GetTypeObject() == TypeObject.foodObject)
          return false;
      }

      return true;
    }

    return false;
  }

  private bool CheckGroundPlayer(Vector3 direction)
  {
    // Проверяем позицию клетки впереди игрока
    if (direction.z > 0f && !Physics.Raycast(transform.position + transform.forward, Vector3.down, 1f))
      return false;
    // Проверяем позицию клетки слева от игрока
    else if (direction.x < 0f && !Physics.Raycast(transform.position - transform.right, Vector3.down, 1f))
      return false;
    // Проверяем позицию клетки справа от игрока
    else if (direction.x > 0f && !Physics.Raycast(transform.position + transform.right, Vector3.down, 1f))
      return false;
    // Проверяем позицию клетки позади игрока
    else if (direction.z < 0f && !Physics.Raycast(transform.position - transform.forward, Vector3.down, 1f))
      return false;

    return true;
  }

  #region Поворот камеры

  private void CameraRotation(float parValue)
  {
    if (levelManager.LevelCompleted || levelManager.IsPause || levelManager.GridLevel.TryStatesLevel() || levelManager.IsLevelMenu)
      return;

    targetRotation += parValue;
    isCameraRotation = true;
  }

  private void SmoothCameraRotation()
  {
    if (!isCameraRotation)
      return;

    Quaternion currentRotation = levelManager.CinemachineVirtual.transform.rotation;
    Quaternion targetQuaternion = Quaternion.Euler(48.0f, targetRotation, 0.0f);
    Quaternion newRotation = Quaternion.Slerp(currentRotation, targetQuaternion, 3f * Time.deltaTime);

    levelManager.CinemachineVirtual.transform.rotation = newRotation;

    // Проверяем, достигли ли нужного угла поворота
    if (Quaternion.Angle(currentRotation, targetQuaternion) < 0.01f)
    {
      isCameraRotation = false;
      levelManager.CinemachineVirtual.transform.rotation = targetQuaternion;
    }
  }

  #endregion

  /// <summary>
  /// Плавный поворот игрока
  /// </summary>
  private void SmoothPlayerRotation()
  {
    if (!isPlayerRotation)
      return;

    Quaternion rotation = Quaternion.LookRotation(direction);
    _meshTransform.rotation = Quaternion.Lerp(_meshTransform.rotation, rotation, _speedRotaion * Time.deltaTime);

    if (_meshTransform.rotation == rotation)
      isPlayerRotation = false;
  }

  //======================================

  public override void RemoveRigidbody()
  {
    Destroy(Rigidbody);
  }

  //======================================
}