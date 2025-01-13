using UnityEngine;
using UnityEngine.SceneManagement;

namespace SMZero
{
    public class ZoneManager : MonoBehaviour
    {
        private static ZoneManager instance;
        public static ZoneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ZoneManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ZoneManager");
                        instance = go.AddComponent<ZoneManager>();
                    }
                }
                return instance;
            }
        }

        private ZoneUI zoneUI;
        private BuildingPopup buildingPopup;
        private Building activeBuilding;
        private ZoneNFT activeZoneNFT;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            zoneUI = FindObjectOfType<ZoneUI>();
            buildingPopup = FindObjectOfType<BuildingPopup>();
        }

        public void StakeZoneNFT(ZoneNFT zoneNFT)
        {
            if (activeBuilding == null)
            {
                Debug.LogError("No active building to stake Zone NFT to!");
                return;
            }

            activeZoneNFT = zoneNFT;
            activeBuilding.SetActive(true);
            
            if (zoneUI != null)
            {
                zoneUI.InitializeSlots();
            }
        }

        public void UnstakeZoneNFT()
        {
            if (activeBuilding == null || activeZoneNFT == null)
            {
                Debug.LogWarning("No active building or Zone NFT to unstake!");
                return;
            }

            activeBuilding.SetActive(false);
            activeZoneNFT = null;
        }

        public void SetActiveBuilding(Building building)
        {
            activeBuilding = building;
            if (buildingPopup != null)
            {
                buildingPopup.Show(building);
            }
        }

        public Building GetActiveBuilding()
        {
            return activeBuilding;
        }

        public ZoneNFT GetActiveZoneNFT()
        {
            return activeZoneNFT;
        }
    }
} 