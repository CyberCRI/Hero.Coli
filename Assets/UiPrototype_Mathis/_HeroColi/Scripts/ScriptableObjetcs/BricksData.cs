using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    [System.Serializable]
    public class BricksData : ScriptableObject
    {
        [Header("Main Properties")]
        protected DataState _state = DataState.Empty;
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

        [SerializeField] public Sprite symbole;
        [SerializeField] public new string name;

        [SerializeField] public BricksType type;

        [SerializeField] public DisplayDevice deviceContainingThis;

        [Space(10)]

        protected string _brickDescription;
        public string Description
        {
            get { return _brickDescription; }
        }

        protected string _deviceDescriptionPart;
        public string DeviceDescriptionPart
        {
            get { return _deviceDescriptionPart; }
        }

        virtual public void GenerateDescriptionElements ()
        {

        }

        virtual protected void CleanBrickProperties()
        {
            
        }
    }
}
