using UnityEngine;
using System.Collections;

namespace VARLab.RespiratoryTherapy
{
    public class NetworkController : MonoBehaviour
    {
        public bool IsConnected { get; private set; }
        private bool shouldScanForNetwork = true;
        public float NetworkPingInterval = 5f; 

        //checks if network is available
        public bool IsNetworkAvailable()
        {
            if (!IsConnectedToNetwork())
            {
                IsConnected = false;

                // Prevent the event from being triggered multiple times
                if (shouldScanForNetwork)
                {
                    shouldScanForNetwork = false;
                    StartCoroutine(ScanForNetwork(NetworkPingInterval));
                }
                Debug.Log("No internet connection.");
                return false;
            }

            IsConnected = true;
            Debug.Log("Connected to the internet.");
            return true;
        }

        // scans for network availability
        private IEnumerator ScanForNetwork(float scanTimeIntervals)
        {
            while (!IsConnectedToNetwork())
            {
                yield return new WaitForSeconds(scanTimeIntervals);
            }

            IsConnected = true;
            shouldScanForNetwork = true; // Reset
            Debug.Log("Reconnected to the internet.");
        }

        // Virtual method to check network connectivity
        public virtual bool IsConnectedToNetwork()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
