using UnityEngine;

using Alekrus.UnivarsalPlatform;
using Alekrus.UnivarsalPlatform.SaveLoad;

namespace Assets.Scripts.Save
{
  internal static class PC_Registrar
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Register()
    {
      SubSustemProvider.SetGetEventHandler<ISaveLoad>((main) => new PC_SaveLoad(main));
    }
  }
}