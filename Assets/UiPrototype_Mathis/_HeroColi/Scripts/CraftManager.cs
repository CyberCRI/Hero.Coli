using System;
using System.Collections.Generic;
using UIProto.Data;
using UIProto.Data.Crafting;
using UIProto.Display;
using UIProto.Scriptable;
using UIProto.Scriptable.Enums;
using UIProto.Scriptable.Level;
using UnityEngine;
using UnityEngine.UI;

namespace UIProto.Manager
{
    #region Enums
    public enum CraftingState
    {
        Overview,
        Focus
    }

    public enum ToggleSelected
    {
        None,
        Promoter,
        RBS,
        CodingSequence,
        Terminator
    }
    #endregion

    public class CraftManager : MonoBehaviour
    {
        #region Variables
        private static CraftManager _instance;
        public static CraftManager Instance
        {
            get
            {
                return _instance;
            }
        }

        [Header("Main Properties")]
        private int _plasmideNumber;
        public int PlasmidesNumber
        {
            set
            {
                if (value >= minPlasmidesNumber && value <= maxPlasmidesNumber)
                    _plasmideNumber = value;

                CreateDeviceSockets();
            }
        }

        [SerializeField] private int minPlasmidesNumber = 1;
        [SerializeField] private int maxPlasmidesNumber = 3;

        private int minPlasmidesIndex;
        private int maxPlasmidesIndex;

        public BaseLevelData baseData;

        [Space(10)]

        private List<DisplayDevice> _craftingDevices = new List<DisplayDevice> ();
        public List<DisplayDevice> CraftingDevices
        {
            get { return _craftingDevices; }
        }

        private List<DeviceId> storedDevices = new List<DeviceId> ();

        private List<CraftingBricks> craftingBricks = new List<CraftingBricks>();

        [Header("Overview Display")]
        public List<PlasmideDisplay> plasmidesDisplays;

        [Header("Focus Screen")]
        [SerializeField] private PlasmideDisplay focusedPlasmideDisplay;

        [SerializeField] private PlasmideDisplay nextPlasmideDisplay;
        [SerializeField] private PlasmideDisplay previousPlasmideDisplay;

        [SerializeField] private PlasmideInformationsDisplay plasmideInformationsDisplay;

        private int focusedPlasmideIndex = 0;
        private int nextPlasmideIndex = 0;
        private int previousPlasmideIndex = 0;

        [SerializeField] private Toggle promoterToggle;
        [SerializeField] private Toggle RBSToggle;
        [SerializeField] private Toggle codSeqToggle;
        [SerializeField] private Toggle terminatorToggle;

        [SerializeField] private ToggleGroup toggleGroup;

        private Scriptable.DisplayDevice _focus;
        public Scriptable.DisplayDevice Focus
        {
            set
            {
                _focus = value;

                if (_focus == null)
                    return;

                focusedPlasmideIndex = _focus.craftManagerIndex;

                nextPlasmideIndex = focusedPlasmideIndex + 1 > maxPlasmidesIndex ? minPlasmidesIndex : focusedPlasmideIndex + 1;
                previousPlasmideIndex = focusedPlasmideIndex - 1 < minPlasmidesIndex ? maxPlasmidesIndex : focusedPlasmideIndex - 1;

                plasmideInformationsDisplay.Device = _focus;

                //Debug.Log("Focus : " + _focus.craftManagerIndex);
                //Debug.Log("Next : " + nextPlasmideIndex);
                //Debug.Log("Previous : " + previousPlasmideIndex);
            }
        }
        
        private CraftingState state = CraftingState.Overview;

        private ToggleSelected _toggleGroupState;
        public ToggleSelected ToggleGroupState
        {
            get { return _toggleGroupState; }
            set
            {
                if (_toggleGroupState == value)
                    return;

                _toggleGroupState = value;

                switch (_toggleGroupState)
                {
                    case ToggleSelected.None:
                        Debug.Log("Afficher Model Store");
                        break;
                    case ToggleSelected.Promoter:
                        Debug.Log("Afficher Promoter Store");
                        break;
                    case ToggleSelected.RBS:
                        Debug.Log("Afficher RBS Store");
                        break;
                    case ToggleSelected.CodingSequence:
                        Debug.Log("Afficher CodingSequence Store");
                        break;
                    case ToggleSelected.Terminator:
                        Debug.Log("Afficher Terminator Store");
                        break;
                }
            }
        }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            foreach (PlasmideDisplay dis in plasmidesDisplays)
                dis.gameObject.SetActive(false);
        }

        private void Start ()
        {
            craftingBricks = baseData.baseBricks;

            List<DeviceDisplayData> baseDevices = baseData.baseDevices;

            PlasmidesNumber = baseData.plasmidesNumber;

            for (int i = 0; i < baseDevices.Count; i ++)
                SetDevice(i, baseDevices[i]);

            minPlasmidesIndex = minPlasmidesNumber - 1;
            maxPlasmidesIndex = maxPlasmidesNumber - 1;
        }

        private void OnDestroy()
        {
            foreach(DisplayDevice dev in _craftingDevices)
            {
                dev.OnStoreNewDevice -= StoreNewDevice;
                dev.OnCompareWithStoredDevice -= CompareWithStoredDevices;
            }
        }
        #endregion

        #region Device Methods

        void CreateDeviceSockets()
        {
            int startI = _craftingDevices.Count;

            for (int i = startI; i < _plasmideNumber; i++)
            {
                DisplayDevice newDevice = new DisplayDevice();

                newDevice.Initialyze();

                _craftingDevices.Add(newDevice);

                plasmidesDisplays[i].gameObject.SetActive(true);

                newDevice.OnStoreNewDevice += StoreNewDevice;
                newDevice.OnCompareWithStoredDevice += CompareWithStoredDevices;
            }  
        }

        public bool CompareWithStoredDevices(DisplayDevice deviceToCompare)
        {
            for (int i = 0; i < storedDevices.Count; i++)
            {
                DeviceDisplayData lDeviceData = storedDevices[i].device;

                bool identicalPromoter =
                    lDeviceData.promoter == deviceToCompare.Promoter;
                bool identicalRBS =
                    lDeviceData.RBS == deviceToCompare.RBS;
                bool identicalCodingSequence =
                   lDeviceData.codingSequence == deviceToCompare.CodingSequence;
                bool identicalTerminator =
                   lDeviceData.terminator == deviceToCompare.Terminator;

                if (identicalPromoter && identicalRBS && identicalCodingSequence && identicalTerminator)
                    return true;
            }

            return false;
        }

        public DeviceDisplayData GetDevice(string id)
        {
            for (int i = 0; i < storedDevices.Count; i++)
                if (storedDevices[i].id == id)
                    return storedDevices[i].device;

            return null;
        }

        public void SetDevice(int id, DeviceDisplayData data)
        {
            DisplayDevice lDevice = _craftingDevices[id];

            plasmidesDisplays[id].Device = lDevice;

            lDevice.SetFromData(data);
            lDevice.craftManagerIndex = id;
        }

        public void StoreNewDevice(DeviceDisplayData data)
        {
            DeviceId newDevice = new DeviceId
            {
                id = data.name,
                device = data
            };

            storedDevices.Add(newDevice);
        }
        #endregion

        #region Bricks Methods
        public void SetBrickToDevice (Scriptable.DisplayDevice device, BricksData brick)
        {
            device.SetBrick(brick.type, brick);
        }

        public List<CraftingBricks> GetCraftingBricks(Scriptable.DisplayDevice device)
        {
            List<CraftingBricks> bricks = new List<CraftingBricks>();

            foreach (CraftingBricks data in craftingBricks)
                foreach (DisplayDevice dev in data.deviceContainingThis)
                    if (dev == device)
                        bricks.Add(data);

            return bricks;
        }

        public CraftingBricks GetCraftingBrick(DisplayDevice device, BricksType type)
        {
            foreach (CraftingBricks data in craftingBricks)
                foreach (DisplayDevice dev in data.deviceContainingThis)
                    if (dev == device && dev.GetBrick(type) == data.Brick)
                        return data;

            throw new Exception("No Crafting Bricks found with those parameters");
        }
        #endregion

        #region FocusScreen Methods
        public void SetPlasmideFocus(int plasmideIndex)
        {
            Focus = CraftManager.Instance.CraftingDevices[plasmideIndex];

            focusedPlasmideDisplay.Device = _focus;

            nextPlasmideDisplay.Device = CraftingDevices[nextPlasmideIndex];
            previousPlasmideDisplay.Device = CraftingDevices[previousPlasmideIndex];
        }

        public void ClearFocus()
        {
            _focus = null;
        }

        public void Toggle_OnValueChanged ()
        {
            if (promoterToggle.isOn)
                ToggleGroupState = ToggleSelected.Promoter;
            else if (RBSToggle.isOn)
                ToggleGroupState = ToggleSelected.RBS;
            else if (codSeqToggle.isOn)
                ToggleGroupState = ToggleSelected.CodingSequence;
            else if (terminatorToggle.isOn)
                ToggleGroupState = ToggleSelected.Terminator;
            else
                ToggleGroupState = ToggleSelected.None;
        }

        public void InitToggleSelected ()
        {
            toggleGroup.SetAllTogglesOff();
        }
        #endregion

        #region Misc
        #endregion
    }
}
