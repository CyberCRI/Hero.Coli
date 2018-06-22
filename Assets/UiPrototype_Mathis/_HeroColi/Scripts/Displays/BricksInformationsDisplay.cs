using UIProto.Scriptable.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UIProto.Display
{
    public class BricksInformationsDisplay : MonoBehaviour
    {
        [SerializeField] public BricksType displayType;

        [SerializeField] private Text _brickName;
        public string BrickName
        {
            get { return _brickName.text; }
            set
            {
                _brickName.text = value;
            }
        }

        [SerializeField] private Text _informations;
        public string Informations
        {
            get { return _informations.text; }
            set
            {
                _informations.text = value;
            }
        }

        public void HandleAnimatorTrigger (string triggerName)
        {
            GetComponent<Animator>().SetTrigger(triggerName);
        }
        
    }
}
