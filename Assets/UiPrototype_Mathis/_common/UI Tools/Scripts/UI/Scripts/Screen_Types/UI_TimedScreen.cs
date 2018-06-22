using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Misc.UI
{
    public class UI_TimedScreen : UI_Screen 
    {
        #region Variables
        [Header("Timed Screen Properties")]
        public float screenTime = 2f;
        public UnityEvent onTimeCompleted = new UnityEvent();

        private float startTime;
        #endregion

        #region TimedScreen Methods
        public override void Open()
        {
            base.Open();

            startTime = Time.time;
            StartCoroutine(WaitForTime());
        }

        IEnumerator WaitForTime()
        {
            yield return new WaitForSeconds(screenTime);

            if(onTimeCompleted != null)
                onTimeCompleted.Invoke();
        }
        #endregion
    }
}
