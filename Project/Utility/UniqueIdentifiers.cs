using UnityEngine;

namespace Lerp2API.Utility
{
    internal class UniqueIdentifiers
    {
        private string UniqueMachineId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
}