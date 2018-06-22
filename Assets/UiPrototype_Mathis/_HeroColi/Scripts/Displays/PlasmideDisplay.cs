using System;
using System.Collections.Generic;
using UIProto.Manager;
using UIProto.Scriptable;
using UIProto.Scriptable.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UIProto.Display
{
    public class PlasmideDisplay : MonoBehaviour
    {
        #region Variables
        protected Image symbole;

        protected DisplayDevice _device;
        public DisplayDevice Device
        {
            get { return _device; }
            set
            {
                //bool wasEmpty = _device.State == State.Empty;

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

        [Header("Bricks Displays")]

        [SerializeField] protected BricksDisplay promoterDisplay;
        [SerializeField] protected BricksDisplay RBSDisplay;
        [SerializeField] protected BricksDisplay CodingSequenceDisplay;
        [SerializeField] protected BricksDisplay TerminatorDisplay;

        private List<BricksDisplay> brickDisplays = new List<BricksDisplay>();

        [Header("Empty Sprites")]

        [SerializeField] protected Sprite promoterEmptySprite;
        [SerializeField] protected Sprite RBSEmptySprite;
        [SerializeField] protected Sprite CodingSequenceEmptySprite;
        [SerializeField] protected Sprite TerminatorEmptySprite;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            foreach (BricksDisplay display in GetComponentsInChildren<BricksDisplay>())
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
            if (brick != null)
            {
                BricksDisplay display = GetBricksDisplay(brick.type);

                if (brick.State == DataState.Empty)
                {
                    Sprite image;

                    switch (brick.type)
                    {
                        case BricksType.Promoter:
                            image = promoterEmptySprite;
                            break;
                        case BricksType.RBS:
                            image = RBSEmptySprite;
                            break;
                        case BricksType.CodingSequence:
                            image = CodingSequenceEmptySprite;
                            break;
                        case BricksType.Terminator:
                            image = TerminatorEmptySprite;
                            break;
                        default:
                            throw new Exception("No empty Sprite for : " + brick.type);
                    }

                    display.Image = image;
                }
                else
                    display.Image = brick.symbole;
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

        #region Plasmides Display Methods
        public void SetPlasmideFocus()
        {
            CraftManager.Instance.SetPlasmideFocus(_device.craftManagerIndex);
        }

        private BricksDisplay GetBricksDisplay (BricksType type)
        {
            foreach (BricksDisplay display in brickDisplays)
                if (display.displayType == type)
                    return display;

            throw new Exception(name + " Cannot got Display of type : " + type.ToString());
        }

        #endregion
    }
}