using System;
using System.Collections.Generic;
using UIProto.Scriptable;
using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Display
{
    public class PlasmideInformationsDisplay : MonoBehaviour
    {
        #region Variables
        protected DisplayDevice _device;
        public DisplayDevice Device
        {
            get { return _device; }
            set
            {
                CleanDeviceListeners();

                _device = value;

                InitDeviceListeners();

                // Modifier affichage en conséquence

                Device_OnBrickChange(_device.Promoter);
                Device_OnBrickChange(_device.RBS);
                Device_OnBrickChange(_device.CodingSequence);
                Device_OnBrickChange(_device.Terminator);
            }
        }

        [Header("Informations Displays")]

        [SerializeField] protected BricksInformationsDisplay promoterInformationsDisplay;
        [SerializeField] protected BricksInformationsDisplay RBSInformationsDisplay;
        [SerializeField] protected BricksInformationsDisplay CodingSequenceInformationsDisplay;
        [SerializeField] protected BricksInformationsDisplay TerminatorInformationsDisplay;

        private List<BricksInformationsDisplay> brickDisplays = new List<BricksInformationsDisplay>();

        #endregion

        #region MonoBehaviour methods
        private void Awake()
        {
            foreach (BricksInformationsDisplay display in GetComponentsInChildren<BricksInformationsDisplay>())
                brickDisplays.Add(display);
        }

        private void Start ()
        {
            if (_device != null)
            {
                Device_OnBrickChange(_device.Promoter);
                Device_OnBrickChange(_device.RBS);
                Device_OnBrickChange(_device.CodingSequence);
                Device_OnBrickChange(_device.Terminator);
            }
        }

        private void OnDestroy()
        {
            CleanDeviceListeners();
        }
        #endregion

        #region Events Methods
        private void Device_OnBrickChange(BricksData brick)
        {
            if (_device != null)
            {
                BricksInformationsDisplay informationDisplay = GetBricksDisplay(brick.type);

                if (brick.State == DataState.Empty)
                    informationDisplay.gameObject.SetActive(false);
                else
                {
                    informationDisplay.gameObject.SetActive(true);
                    informationDisplay.BrickName = brick.name;
                    informationDisplay.Informations = brick.Description;
                }
            }
        }

        private void Device_OnDeviceCompleted()
        {
            //symbole.sprite = _device.symbol;

            // Affichage nom device
            // Affichage information device
        }

        private void InitDeviceListeners()
        {
            _device.OnBrickChange += Device_OnBrickChange;
            _device.OnDeviceCompleted += Device_OnDeviceCompleted;
        }

        private void CleanDeviceListeners()
        {
            if (_device == null)
                return;

            _device.OnBrickChange -= Device_OnBrickChange;
            _device.OnDeviceCompleted -= Device_OnDeviceCompleted;
        }
        #endregion

        #region Plasmide Informations Display Methods
        private BricksInformationsDisplay GetBricksDisplay(BricksType type)
        {
            foreach (BricksInformationsDisplay display in brickDisplays)
                if (display.displayType == type)
                    return display;

            throw new Exception(name + " Cannot got Display of type : " + type.ToString());
        }
        #endregion
    }
}
