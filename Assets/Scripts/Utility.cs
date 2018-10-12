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

        public void AnimateMoveWithDelay(float delay, GameObject go, Vector3 targetPosition, float time, Finished onFinished = null)
        {
            StartCoroutine(this.Delay(delay,
                delegate
                {
                    this.AnimateMove(go, targetPosition, time, onFinished);
                }));
        }

        public void AnimateMoveLocalWithDelay(float delay, GameObject go, Vector3 targetPosition, float time, Finished onFinished = null)
        {
            StartCoroutine(this.Delay(delay,
                delegate
                {
                    this.AnimateMoveLocal(go, targetPosition, time, onFinished);
                }));
        }

        public void AnimateScaleWithDelay(float delay, GameObject go, Vector3 targetScale, float time, Finished onFinished = null)
        {
            StartCoroutine(this.Delay(delay,
                delegate
                {
                    this.AnimateScale(go, targetScale, time, onFinished);
                }));
        }

        public void AnimateMove(GameObject go, Vector3 targetScale, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateMoveActual(go, targetScale, time, onFinished));
        }

        public void AnimateMoveLocal(GameObject go, Vector3 targetScale, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateMoveLocalActual(go, targetScale, time, onFinished));
        }


        public void AnimateScale(GameObject go, Vector3 targetScale, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateScaleActual(go, targetScale, time, onFinished));
        }

        public void AnimateAlpha(Renderer renderer, float targetAlpha, float time, Finished onFinished = null)
        {
            StartCoroutine(this.AnimateAlphaActual(renderer, targetAlpha, time, onFinished));
        }

        private IEnumerator AnimateAlphaActual(Renderer renderer, float targetAlpha, float seconds,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();

            float r = renderer.material.color.r;
            float g = renderer.material.color.g;
            float b = renderer.material.color.b;
            float a = renderer.material.color.a;


            float elapsedTime = 0;
            
            while (elapsedTime < 1)
            {

                float alpha = a + elapsedTime * (targetAlpha - a);

                Color newColor = new Color(r, g, b, alpha);
                renderer.material.SetColor("_Color", newColor);
                elapsedTime += Time.deltaTime / seconds;

                yield return delay;
            }
            
            renderer.material.SetColor("_Color", new Color(r, g, b, targetAlpha));

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator AnimateScaleActual(GameObject objectToMove, Vector3 targetScale, float seconds,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();
            float elapsedTime = 0; // 0 to 1
            Vector3 startScale = objectToMove.transform.localScale;
            while (elapsedTime < 1)
            {
                objectToMove.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime);
                elapsedTime += Time.deltaTime/seconds;
                yield return delay;
            }
            objectToMove.transform.position = targetScale;

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator AnimateMoveActual(GameObject objectToMove, Vector3 targetPosition, float seconds,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();
            float elapsedTime = 0;
            Vector3 startPos = objectToMove.transform.position;
            while (elapsedTime < 1)
            {
                objectToMove.transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime);
                elapsedTime += Time.deltaTime/seconds;
                yield return delay;
            }
            objectToMove.transform.position = targetPosition;

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator AnimateMoveLocalActual(GameObject objectToMove, Vector3 targetPosition, float seconds,
                                                Finished onFinished = null)
        {
            var delay = new WaitForEndOfFrame();
            float elapsedTime = 0;
            Vector3 startPos = objectToMove.transform.localPosition;
            while (elapsedTime < 1)
            {
                objectToMove.transform.localPosition = Vector3.Lerp(startPos, targetPosition, elapsedTime);
                elapsedTime += Time.deltaTime / seconds;
                yield return delay;
            }
            objectToMove.transform.localPosition = targetPosition;

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator Delay(float time, Finished OnFinished)
        {
            var delay = new WaitForEndOfFrame();
            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return delay;
            }

            if (OnFinished != null)
            {
                OnFinished();
            }
        }

    }
}