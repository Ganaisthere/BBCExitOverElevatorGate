using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace BBCExitOverElevatorGate
{
    public static class Other
    {
        public static bool TryFind(this Transform transform0, string n, out Transform transform1)
        {
            bool success = false;
            transform1 = transform0.Find(n);
            if (transform1 != null)
            {
                success = true;
            }
            return success;
        }

        public static bool TryGet<T>(this AssetManager assetMan, string name, out T thing)
        {
            bool success = false;
            thing = assetMan.Get<T>(name);
            if (thing != null)
            {
                success = true;
            }
            return success;
        }
    }
}
