using UnityEngine;

using Sokoban.GameManagement;

public class DynamicObjects : Block
{
  private bool isMoving = false;

  private float speed = 2.0f;

  private Vector3 lastPosition;

  private Vector3 direction;

  //--------------------------------------

  public new Rigidbody rigidbody { get; private set; }

  //======================================

  protected override void Awake()
  {
    base.Awake();

    rigidbody = GetComponent<Rigidbody>();
  }

  private void Start()
  {
    if (Sokoban.GridEditor.GridEditor.GridEditorEnabled)
      RemoveRigidbody();
  }

  private void Update()
  {
    if (!isMoving)
      return;

    transform.position = Vector3.MoveTowards(transform.position, lastPosition + direction, speed * Time.deltaTime);

    if (transform.position == lastPosition + direction)
      isMoving = false;
  }

  //======================================

  public bool ObjectMove(Vector3 parDirection, float parSpeed)
  {
    if (IsBlocked(parDirection))
      return false;

    isMoving = true;
    lastPosition = transform.position;
    direction = parDirection;
    speed = parSpeed;
    GameManager.Instance.ProgressData.TotalNumberMovesBox++;
    return true;
  }

  /// <summary>
  /// True, если движение объекта вперед заблокировано
  /// </summary>
  /// <param name="direction">Направление движения</param>
  private bool IsBlocked(Vector3 direction)
  {
    if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 1))
    {
      if (hit.collider)
      {
        if (hit.collider.TryGetComponent<DecoreObject>(out var decoreObject))
          return decoreObject.IsEnableBoxCollider;

        if (hit.collider.GetComponent<DynamicObjects>() || hit.collider.GetComponent<StaticObjects>())
          return true;

        if (hit.collider.TryGetComponent(out SpikeObject spikeObject))
          return spikeObject.IsSpikeActivated;

        if (hit.collider.GetComponent<ButtonDoorObject>())
          return false;
      }
    }

    return false;
  }

  /// <summary>
  /// True, если объект падает
  /// </summary>
  public bool IsObjectFalling()
  {
    return rigidbody.velocity.y < -0.1f;
  }

  //======================================

  public override void RemoveRigidbody()
  {
    Destroy(rigidbody);
  }

  //======================================
}