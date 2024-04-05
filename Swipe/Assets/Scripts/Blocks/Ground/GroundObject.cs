using Sokoban.LevelManagement;
using UnityEngine;

public class GroundObject : StaticObjects
{
  [SerializeField] private TypesGround _typeGround;

  [SerializeField] private GameObject _groundActive;

  //--------------------------------------

  private MeshFilter meshFilter;

  private MeshFilter groundMeshFilter;

  //======================================

  public bool IsBlockActive { get; private set; }

  //======================================

  protected override void Awake()
  {
    base.Awake();

    meshFilter = GetComponentInChildren<MeshFilter>();

    if (_groundActive != null)
      groundMeshFilter = _groundActive.GetComponentInChildren<MeshFilter>();
  }

  //======================================

  public void ChangeBlock()
  {
    meshFilter.sharedMesh = groundMeshFilter.sharedMesh;

    IsBlockActive = true;

    LevelManager.Instance.IsLevelComplete();
  }

  //======================================
}

public enum TypesGround
{
  Ground,
  Ground1
}