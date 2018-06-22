using UIProto.Scriptable.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIProto.Scriptable
{
    [System.Serializable]
    public class BricksData : ScriptableObject
    {
        [Header("Main Properties")]
        [SerializeField] protected DataState _state = DataState.Empty;
        public DataState State
        {
            get { return _state; }
            set
            {
                _state = value;

                if (_state == DataState.Empty)
                    CleanBrickProperties();
            }
        }

        [FormerlySerializedAs("symbole")]
        [SerializeField] public Sprite symbol;
        [SerializeField] private new string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;

                _state = CheckState() ? DataState.Filled : DataState.Empty;
            }
        }

        [SerializeField] public BricksType type;

        [HideInInspector] public DisplayDevice deviceContainingThis;

        [Space(10)]

        protected string _brickDescription;
        public string Description
        {
            get
            {
                if (_brickDescription == "")
                    GenerateDescriptionElements();

                return _brickDescription;
            }
        }

        protected string _deviceDescriptionPart;
        public string DeviceDescriptionPart
        {
            get
            {
                if (_deviceDescriptionPart == "")
                    GenerateDescriptionElements();

                return _deviceDescriptionPart;
            }
        }

        virtual public void GenerateDescriptionElements ()
        {

        }

        virtual protected void CleanBrickProperties()
        {
            symbol = null;
            name = "";
        }
        
        virtual protected bool CheckState ()
        {
            if (name != null && name != "")
                return true;

            return false;
        }
    }
}
