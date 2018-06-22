using UIProto.Scriptable.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UIProto.Display
{
    public class BricksDisplay : MonoBehaviour
    {
        [SerializeField] public BricksType displayType;

        protected Sprite _image;
        public Sprite Image
        {
            set
            {
                _image = value;
                GetComponent<Image>().sprite = _image;
            }
        }
    }
}
