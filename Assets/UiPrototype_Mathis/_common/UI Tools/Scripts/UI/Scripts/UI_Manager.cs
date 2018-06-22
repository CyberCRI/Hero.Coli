using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace Misc.UI
{
    public class UI_Manager : MonoBehaviour 
    {
        #region Variables
        [Header("Main Properties")]
        public UI_Screen startingScreen;

        [Header("System Events")]
        public UnityEvent onSwitchedScreen = new UnityEvent();

        [Header("Fader Properties")]
        public Image fader;

        public float fadeInDuration = 1f;
        public float fadeInAlpha = 1f;

        public float fadeOutDuration = 1f;
        public float fadeOutAlpha = 0f;

        private Component[] screens = new Component[0];

        private UI_Screen previousScreen;
        public UI_Screen PreviousScreen
        {
            get {return previousScreen;}
        }

        private UI_Screen currentScreen;
        public UI_Screen CurrentScreen
        {
            get {return currentScreen;}
        }
        #endregion

        #region MonoBehaviour Methods
    	void Start () 
        {
            screens = GetComponentsInChildren<UI_Screen>(true);
            InitializeScreens();

            if (startingScreen)
                ToggleScreen(startingScreen);
            else
                throw new Exception("No Starting Screen");

            if(fader)
                fader.gameObject.SetActive(true);

            Fade(fadeOutAlpha, fadeOutDuration);
    	}
        #endregion

        #region Manager Methods
        public void ToggleScreen(UI_Screen screen)
        {
            if(screen)
            {
                if (currentScreen)
                    currentScreen.Close();

                previousScreen = currentScreen;

                currentScreen = screen;
                currentScreen.gameObject.SetActive(true);
                currentScreen.Open();

                if(onSwitchedScreen != null)
                    onSwitchedScreen.Invoke();
            }
        }

        public void Fade (float targetAlpha, float fadeDuration)
        {
            if (fader)
                fader.CrossFadeAlpha(targetAlpha, fadeDuration, false);
        }

        public void GoToPreviousScreen()
        {
            if(previousScreen)
                ToggleScreen(previousScreen);
        }

        void InitializeScreens()
        {
            foreach(UI_Screen screen in screens)
            {
                screen.Animator = screen.GetComponent<Animator>();
                screen.gameObject.SetActive(false);
            }
            startingScreen.gameObject.SetActive(true);
        }
        #endregion
    }
}
