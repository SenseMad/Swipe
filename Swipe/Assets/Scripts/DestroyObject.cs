using UnityEngine;

public class DestroyObject : MonoBehaviour
{
  [SerializeField] private ParticleSystem _particleSystem;

  //======================================

  private void Awake()
  {
    Destroy(gameObject, _particleSystem.main.duration);
  }

  //======================================
}