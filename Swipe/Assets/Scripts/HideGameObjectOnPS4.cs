using UnityEngine;

public sealed class HideGameObjectOnPS4 : MonoBehaviour
{
#if UNITY_PS4
  private void OnEnable()
  {
    gameObject.SetActive(false);
  }
#endif
}
