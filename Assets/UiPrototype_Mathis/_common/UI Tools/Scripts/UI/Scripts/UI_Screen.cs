using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Misc.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_Screen : MonoBehaviour 
    {
        #region Variables
        [Header("Main Properties")]
        // The starting Screen
        public Selectable startSelectable;

        [Header("Screen Events")]
        public UnityEvent onScreenStart = new UnityEvent();
        public UnityEvent onScreenClose = new UnityEvent();

        private Animator _animator;
        public Animator Animator
        {
            get { return _animator; }
            set
            {
                if (value != null)
                    _animator = value;
            }
        }
        #endregion

        #region MonoBehaviour Methods
    	void Start () 
        {
            if(startSelectable)
                EventSystem.current.SetSelectedGameObject(startSelectable.gameObject);
    	}
        #endregion

        #region Screen Methods
        public virtual void Open()
        {
            if(onScreenStart != null)
                onScreenStart.Invoke();

            HandleAnimator("show");
        }

        public virtual void Close()
        {
            if(onScreenClose != null)
                onScreenClose.Invoke();

            HandleAnimator("hide");
        }

        void HandleAnimator(string aTrigger)
        {
            if(_animator)
                _animator.SetTrigger(aTrigger);
        }
        #endregion
    }
}
