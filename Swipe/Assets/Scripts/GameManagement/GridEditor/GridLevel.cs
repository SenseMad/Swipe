using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sokoban.GameManagement;
using Sokoban.LevelManagement;
using UnityEngine.UIElements;
using Unity.VisualScripting;

namespace Sokoban.GridEditor
{
  public class GridLevel : MonoBehaviour
  {
    [SerializeField, Tooltip("Список типов блочных объектов")]
    private ListBlockObjectTypes _listBlockObjectTypes;

    [SerializeField] private AnimationCurve animationCurve;

    //--------------------------------------

    private StatesLevel statesLevel;

    private Coroutine myCoroutine;

    //======================================

    private GameManager gameManager;

    private LevelManager levelManager;

    private RandomGrassSpawn randomGrassSpawn;

    private Block[,,] blockObjects;



    private PlayerObjects playerObject;
    private Vector3Int playerStartingPosition;

    private List<FoodObject> listFoodObjects = new();
    private List<DoorObject> listDoorObjects = new();
    private List<GroundObject> listGroundObjects = new();
    private List<DecoreObject> listDecoreObject = new();

    //======================================

    public Block[,,] BlockObjects => blockObjects;

    public bool IsLevelCreated { get; private set; }

    public bool IsLevelDeleted { get; private set; } = true;

    //======================================

    public UnityEvent OnLevelCreated { get; } = new UnityEvent();

    public UnityEvent OnLevelDeleted { get; } = new UnityEvent();

    //======================================

    private void Awake()
    {
      randomGrassSpawn = GetComponent<RandomGrassSpawn>();
    }

    private void Start()
    {
      gameManager = GameManager.Instance;

      levelManager = LevelManager.Instance;

      transform.localPosition = new Vector3(0, -2, 0);
    }

    //======================================

    #region Поиск еды на уровне

    private List<Vector3Int> ListSpawnFood()
    {
      List<Vector3Int> retSpawnList = new();

      int newArraySize = listGroundObjects.Count * 25 / 100;
      System.Random random = new();

      for (int i = 0; i < newArraySize; i++)
      {
        int index = random.Next(0, listGroundObjects.Count);

        while (retSpawnList.Contains(listGroundObjects[index].GetObjectPosition()))
        {
          index = random.Next(0, listGroundObjects.Count);
        }

        retSpawnList.Add(listGroundObjects[index].GetObjectPosition());
      }

      return retSpawnList;
    }

    private void SpawnFood()
    {
      listFoodObjects = new();

      System.Random random = new();

      foreach (var position in ListSpawnFood())
      {
        Vector3Int newPosition = new(position.x, position.y + 3, position.z);

        Block newBlockObjectFood = Instantiate(_listBlockObjectTypes.GetBlockObject(TypeObject.foodObject, random.Next(0, 7)), newPosition, Quaternion.identity);
        newBlockObjectFood.transform.localScale = Vector3.zero;

        newBlockObjectFood.SetPositionObject(newPosition);

        FoodObject foodObject = newBlockObjectFood.GetComponent<FoodObject>();
        listFoodObjects.Add(foodObject);
      }
    }

    private void FindAllFoodObjects()
    {
      /*if (blockObjects == null)
        return;

      listFoodObjects = new();

      foreach (var blockObject in blockObjects)
      {
        if (blockObject == null)
          continue;

        if (!blockObject.TryGetComponent(out FoodObject foodObject))
          continue;

        listFoodObjects.Add(foodObject);
      }*/


    }

    /*public List<FoodObject> GetListFoodObjects()
    {
      return listFoodObjects;
    }*/

    #endregion

    #region Поиск объектов дверей на уровне

    private void FindAllDoorObjects()
    {
      if (blockObjects == null)
        return;

      listDoorObjects = new List<DoorObject>();

      foreach (var blockObject in blockObjects)
      {
        if (blockObject == null)
          continue;

        if (!blockObject.TryGetComponent(out DoorObject doorObject))
          continue;

        listDoorObjects.Add(doorObject);
      }
    }

    public List<DoorObject> GetListDoorObjects()
    {
      return listDoorObjects;
    }

    #endregion

    #region

    private void FindGround()
    {
      if (blockObjects == null)
        return;

      listGroundObjects = new();
      foreach (var blockObject in blockObjects)
      {
        if (blockObject == null)
          continue;

        if (!blockObject.TryGetComponent(out GroundObject groundObject))
          continue;

        //groundObject.Position = new Vector3Int((int)transform.position.x, 2, (int)transform.position.z);
        listGroundObjects.Add(groundObject);
      }

      //Debug.Log($"{listGroundObjects.Count}");
    }    

    public List<GroundObject> GetListGroundObjects()
    {
      return listGroundObjects;
    }

    #endregion

    //======================================

    public void CreatingLevelGrid()
    {
      DeletingLevelObjects();

      StartCoroutine(CreateLevel());
    }

    public void DeletingLevelObjects()
    {
      if (blockObjects == null)
        return;

      myCoroutine = StartCoroutine(DeleteLevel());
    }

    public void SkinReplace()
    {
      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        if (!block.GetComponent<PlayerObjects>())
          continue;

        foreach (var skinData in ShopData.Instance.SkinDatas)
        {
          if (skinData.IndexSkin != gameManager.ProgressData.CurrentActiveIndexSkin)
            continue;

          block.GetComponentInChildren<MeshFilter>().sharedMesh = skinData.ObjectSkin.GetComponentInChildren<MeshFilter>().sharedMesh;
          block.GetComponentInChildren<MeshRenderer>().sharedMaterial = skinData.ObjectSkin.GetComponentInChildren<Renderer>().sharedMaterial;
          break;
        }

        break;
      }
    }

    private void CreatingTrees()
    {
      LevelData levelData = levelManager.GetCurrentLevelData();

      List<Vector3Int> listEmptyCells = new();
      List<Vector3Int> listIgnore = new();

      #region Найти все блоки на карте

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        Vector3Int positionBlock = new((int)block.transform.position.x, (int)block.transform.position.y, (int)block.transform.position.z);
        listIgnore.Add(positionBlock);
      }

      #endregion

      #region Определить свободные клетки на карте

      for (int x = 0; x < levelData.FieldSize.x; x++)
      {
        for (int z = 0; z < levelData.FieldSize.z; z++)
        {
          if (listIgnore.Contains(new Vector3Int(x, 2, z)))
            continue;

          listEmptyCells.Add(new Vector3Int(x, 2, z));
        }
      }

      #endregion

      int newArraySize = (int)(listEmptyCells.Count / 7f);
      Vector3Int[] newArray = new Vector3Int[newArraySize];

      System.Random random = new();
      List<Vector3Int> tempListRandomPlaces = new();

      int i = 0;
      while (i < newArraySize)
      {
        int r = random.Next(0, listEmptyCells.Count);

        if (tempListRandomPlaces.Contains(listEmptyCells[r]))
          continue;

        tempListRandomPlaces.Add(listEmptyCells[r]);
        newArray[i] = listEmptyCells[r];
        i++;
      }

      Block[] blocks = new Block[newArray.Length];
      Block[] blocksDecore = new Block[newArray.Length];
      
      for (int j = 0; j < newArray.Length; j++)
      {
        Block newBlockObject = Instantiate(_listBlockObjectTypes.GetBlockObject(TypeObject.staticObject, 2), transform);
        newBlockObject.transform.localScale = Vector3.zero;

        Vector3Int posPlaceSpawn = newArray[j];
        newBlockObject.transform.position = posPlaceSpawn;
        newBlockObject.SetPositionObject(posPlaceSpawn);

        Block newBlockObjectDecore = Instantiate(_listBlockObjectTypes.GetBlockObject(TypeObject.decorObject, random.Next(0, 8)), transform);
        newBlockObjectDecore.transform.localScale = Vector3.zero;
        
        Vector3Int posPlaceSpawnDecore = new(newArray[j].x, newArray[j].y + 1, newArray[j].z);
        newBlockObjectDecore.transform.position = posPlaceSpawnDecore;
        newBlockObjectDecore.SetPositionObject(posPlaceSpawnDecore);

        blocks[j] = newBlockObject;
        blocksDecore[j] = newBlockObjectDecore;
        blockObjects[posPlaceSpawn.x, posPlaceSpawn.y, posPlaceSpawn.z] = newBlockObject;
        blockObjects[posPlaceSpawnDecore.x, posPlaceSpawnDecore.y, posPlaceSpawnDecore.z] = newBlockObjectDecore;
      }

      StartCoroutine(AnimationCreate());

      IEnumerator AnimationCreate()
      {
        float timer = 0f;

        while (timer < 1)
        {
          timer += Time.deltaTime;
          float t = Mathf.Clamp01(timer / 1);

          foreach (var block in blocks)
          {
            block.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
          }

          foreach (var block in blocksDecore)
          {
            block.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
          }

          foreach (var block in listFoodObjects)
          {
            block.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
          }

          yield return null;
        }

        statesLevel = StatesLevel.Completed;
      }
    }

    private void Test()
    {
      StartCoroutine(AnimationCreate());

      IEnumerator AnimationCreate()
      {
        float timer = 0f;

        while (timer < 1)
        {
          timer += Time.deltaTime;
          float t = Mathf.Clamp01(timer / 1);

          foreach (var block in listFoodObjects)
          {
            block.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
          }

          yield return null;
        }

        statesLevel = StatesLevel.Completed;
      }
    }

    private void SpawnGrass()
    {
      List<Vector3Int> emptyPositionsList = new();
      List<Vector3Int> ignorePositonsList = new();

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        if (!block.GetComponent<GroundObject>())
          continue;

        if (block.transform.position.y == 3)
          break;

        Vector3Int positionUpCurrentBlock = new((int)block.transform.position.x, (int)block.transform.position.y + 1, (int)block.transform.position.z);
        emptyPositionsList.Add(positionUpCurrentBlock);

        foreach (var blockUp in blockObjects)
        {
          if (blockUp == null)
            continue;

          if (blockUp.transform.position.y != 3)
            continue;

          if (blockUp.transform.position == positionUpCurrentBlock)
          {
            ignorePositonsList.Add(positionUpCurrentBlock);
            break;
          }
        }
      }

      List<Vector3Int> positionsList = new();
      for (int i = 0; i < emptyPositionsList.Count; i++)
      {
        if (ignorePositonsList.Contains(emptyPositionsList[i]))
          continue;

        positionsList.Add(emptyPositionsList[i]);
      }

      Vector3Int[] tempArray = new Vector3Int[positionsList.Count];
      for (int i = 0; i < tempArray.Length; i++)
        tempArray[i] = positionsList[i];

      Vector3Int[] placeSpawn = randomGrassSpawn.CheckSpawn(tempArray);
      for (int i = 0; i < placeSpawn.Length; i++)
      {
        Block newBlockObject = Instantiate(randomGrassSpawn.GetRandomTypeBlockObjects(), transform);

        Vector3Int posPlaceSpawn = placeSpawn[i];
        newBlockObject.transform.position = posPlaceSpawn;
        newBlockObject.SetPositionObject(posPlaceSpawn);
        blockObjects[posPlaceSpawn.x, posPlaceSpawn.y, posPlaceSpawn.z] = newBlockObject;
      }
    }

    public void ReloadLevel()
    {
      statesLevel = StatesLevel.Created;

      playerObject.transform.localScale = Vector3.zero;
      playerObject.gameObject.SetActive(true);
      playerObject.transform.position = playerStartingPosition;

      foreach (var foodObject in listFoodObjects)
      {
        foodObject.gameObject.SetActive(true);
        foodObject.IsFoodCollected = false;
      }

      StartCoroutine(AnimationCreatePlayer());

      IEnumerator AnimationCreatePlayer()
      {
        float timer = 0f;

        while (timer < 1)
        {
          timer += Time.deltaTime;
          float t = Mathf.Clamp01(timer / 1);

          playerObject.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
          yield return null;
        }

        statesLevel = StatesLevel.Completed;
      }
    }

    private IEnumerator CreateLevel()
    {
      while (myCoroutine != null || !IsLevelDeleted)
      {
        yield return null;
      }

      IsLevelDeleted = false;
      statesLevel = StatesLevel.Created;
      //transform.position = new Vector3(0, -3, 0);

      LevelData levelData = levelManager.GetCurrentLevelData();
      blockObjects = new Block[levelData.FieldSize.x, levelData.FieldSize.y, levelData.FieldSize.z];

      foreach (var levelObject in levelManager.GetCurrentLevelData().ListLevelObjects)
      {
        Block newBlockObject = Instantiate(_listBlockObjectTypes.GetBlockObject(levelObject.TypeObject, levelObject.IndexObject), transform);

        #region Select Skin

        var skinDatas = ShopData.Instance;

        if (skinDatas != null)
        {
          if (newBlockObject.TryGetComponent(out PlayerObjects outPlayerObject))
          {
            playerObject = outPlayerObject;
            playerStartingPosition = new Vector3Int(levelObject.PositionObject.x, 3, levelObject.PositionObject.z);

            foreach (var skinData in skinDatas.SkinDatas)
            {
              if (skinData.IndexSkin != gameManager.ProgressData.CurrentActiveIndexSkin)
                continue;

              newBlockObject.GetComponentInChildren<MeshFilter>().sharedMesh = skinData.ObjectSkin.GetComponentInChildren<MeshFilter>().sharedMesh;
              newBlockObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = skinData.ObjectSkin.GetComponentInChildren<Renderer>().sharedMaterial;
              break;
            }
          }
        }

        #endregion

        if (!newBlockObject.GetComponent<GroundObject>())
          newBlockObject.transform.localScale = Vector3.zero;

        newBlockObject.transform.position = levelObject.PositionObject;
        //Vector3Int pos = new(levelObject.PositionObject.x, levelObject.PositionObject.y + 2, levelObject.PositionObject.z);
        newBlockObject.SetPositionObject(levelObject.PositionObject);

        blockObjects[levelObject.PositionObject.x, levelObject.PositionObject.y, levelObject.PositionObject.z] = newBlockObject;
      }

      FindGround();

      SpawnFood();

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        if (!block.TryGetComponent(out PlayerObjects playerObjects))
          continue;

        var playerPos = new Vector3Int((int)block.transform.position.x, 0, (int)block.transform.position.z);

        foreach (var ground in GetListGroundObjects())
        {
          var groundPosition = ground.GetObjectPosition();
          if (new Vector3Int(groundPosition.x, groundPosition.y, groundPosition.z) != playerPos)
            continue;

          playerObjects.CheckGround(new Vector3Int(playerPos.x, 3, playerPos.z));
          break;
        }

        break;
      }

      #region The appearance of the platform (Ground)

      float timer = 0f;

      while (timer < 1)
      {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / 1);

        transform.localPosition = new Vector3(0, -2, 0) * animationCurve.Evaluate(1 - t);

        yield return null;
      }

      #endregion

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        block.BoxCollider.enabled = true;

        if (block.TryGetComponent(out DynamicObjects dynamicObjects))
          dynamicObjects.rigidbody.useGravity = true;

        if (block.TryGetComponent(out PlayerObjects playerObjects))
          playerObjects.Rigidbody.useGravity = true;
      }

      #region Random Grass

      CreatingTrees();
      //Test();

      SpawnGrass();

      #endregion

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        if (block.TryGetComponent(out DecoreObject decoreObject))
        {
          if (!decoreObject.IsEnableBoxCollider)
            block.BoxCollider.enabled = false;
        }
      }

      #region Animation of the appearance of all block except Ground

      timer = 0f;

      while (timer < 1)
      {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / 1);

        foreach (var block in blockObjects)
        {
          if (block == null)
            continue;

          if (block.GetComponent<GroundObject>())
            continue;
          /*if (block.GetTypeObject() == TypeObject.staticObject)
            continue;*/

          block.transform.localScale = Vector3.one * animationCurve.Evaluate(t);
        }

        yield return null;
      }

      #endregion

      FindAllFoodObjects();
      FindAllDoorObjects();
      statesLevel = StatesLevel.Completed;
      OnLevelCreated?.Invoke();

      IsLevelCreated = true;
      IsLevelDeleted = true;
    }

    private IEnumerator DeleteLevel()
    {
      if (blockObjects == null)
      {
        myCoroutine = null;
        yield break;
      }

      IsLevelCreated = false;
      IsLevelDeleted = false;
      statesLevel = StatesLevel.Deleted;

      float timer = 0f;

      #region Animation of the appearance of all block except Ground

      while (timer < 1)
      {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / 1);

        foreach (var block in blockObjects)
        {
          if (block == null)
            continue;

          if (block.GetComponent<GroundObject>())
            continue;
          /*if (block.GetTypeObject() == TypeObject.staticObject)
            continue;*/

          if (block.GetTypeObject() == TypeObject.dynamicObject || block.GetTypeObject() == TypeObject.playerObject)
            block.RemoveRigidbody();

          block.transform.localScale = Vector3.one * animationCurve.Evaluate(1 - t);
        }
        
        foreach (var block in listFoodObjects)
        {
          block.transform.localScale = Vector3.one * animationCurve.Evaluate(1 - t);
        }

        yield return null;
      }

      #endregion

      #region Platform Immersion (Ground)

      timer = 0;

      while (timer < 1)
      {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / 1);

        transform.localPosition = new Vector3(0, -2, 0) * animationCurve.Evaluate(t);
        yield return null;
      }

      #endregion

      foreach (var block in blockObjects)
      {
        if (block == null)
          continue;

        Destroy(block.gameObject);
      }

      foreach (var block in listFoodObjects)
      {
        Destroy(block.gameObject);
      }

      myCoroutine = null;
      blockObjects = null;

      IsLevelDeleted = true;
    }

    public bool TryStatesLevel()
    {
      return statesLevel == StatesLevel.Created || statesLevel == StatesLevel.Deleted;
    }

    //======================================

    public enum StatesLevel
    {
      Completed,
      Created,
      Deleted
    }

    //======================================
  }
}