using UnityEngine;
using System.Collections;

namespace TankUtility
{
    public  class Utility : MonoBehaviour
    {
        public delegate void Finished();

        public static Utility Instance
        {
            get;
            private set;
        }

        // Use this for initialization
        void Awake()
        {
            //Check if instance already exists
            if (Instance == null)
            {
                //if not, set instance to this
                Instance = this;
            }
            //If instance already exists and it's not this:
            else if (Instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
        
        public void AnimateScale(GameObject go, Vector3 targetScale, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateScaleActual(go, targetScale, time, onFinished));
        }

        public void AnimateAlpha(Renderer[] renderers, float targetAlpha, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateAlphaActual(renderers, targetAlpha, time, onFinished));
        }

        private IEnumerator AnimateAlphaActual(Renderer[] renderers, float targetAlpha, float time,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();

            float elapsedTime = 0;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float alpha = elapsedTime / time;
                foreach(var r in renderers)
                {
                    Color oldColor = r.material.color;
                    Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
                    r.material.SetColor("_Color", newColor);
                }
                yield return delay;
            }

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator AnimateScaleActual(GameObject objectToMove, Vector3 targetScale, float seconds,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();
            float elapsedTime = 0;
            Vector3 startScale = objectToMove.transform.localScale;
            while (elapsedTime < seconds)
            {
                objectToMove.transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return delay;
            }
            objectToMove.transform.position = targetScale;

            if (onFinished != null)
            {
                onFinished();
            }
        }

    }
}