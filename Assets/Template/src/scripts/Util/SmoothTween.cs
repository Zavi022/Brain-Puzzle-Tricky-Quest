using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * SmoothTween - DOTween Replacement
 * 
 * Implemented Features:
 * - Transform.DOMove, DOLocalMove, DOMoveX/Y/Z, DOLocalMoveX/Y/Z
 * - Transform.DORotate, DOLocalRotate  
 * - Transform.DOScale (supports Vector2, Vector3, float)
 * - Image.DOFade, SpriteRenderer.DOFade, DOColor
 * - CanvasGroup.DOFade
 * - RectTransform.DOAnchorPos
 * - TweenHandle.SetEase, OnComplete, SetDelay, SetUpdate, From, Kill
 * - Complete easing functions: Linear, Sine, Quad, Cubic, Quart, Quint, Expo, Circ, Back, Bounce, Elastic
 * - Delay and ignore time scale support
 */

    public enum EaseType
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce
    }

    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }

    public class SmoothTween : MonoBehaviour
    {
        private static SmoothTween _instance;
        public static SmoothTween Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SmoothTween");
                    _instance = go.AddComponent<SmoothTween>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private List<TweenData> activeTweens = new List<TweenData>();

        public class TweenData
        {
            public Func<float> getter;
            public Action<float> setter;
            public float startValue;
            public float targetValue;
            public float duration;
            public float elapsedTime;
            public EaseType easeType;
            public Action onComplete;
            public bool isCompleted;
            public float delay;
            public float elapsedDelay;
            public bool useUnscaledTime;
            public UnityEngine.Object targetObject; // Target object reference for null reference checking
            public int loops; // For single tween loops
            public LoopType loopType; // Loop type for single tweens
            public int currentLoop; // Current loop count for single tweens
            public bool paused; // New: paused state

            public void Update()
            {
                if (isCompleted) return;
                if (paused) return;

                // If target object is bound and has been destroyed, stop
                if (targetObject == null && targetObject != null)
                {
                    isCompleted = true;
                    return;
                }
                
                float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                if (elapsedDelay < delay)
                {
                    elapsedDelay += dt;
                    return;
                }

                elapsedTime += dt;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                
                float easedProgress = ApplyEase(progress, easeType);
                float currentValue = Mathf.Lerp(startValue, targetValue, easedProgress);
                
                try
                {
                    setter?.Invoke(currentValue);
                }
                catch (MissingReferenceException)
                {
                    isCompleted = true;
                    return;
                }
                catch (System.NullReferenceException)
                {
                    isCompleted = true;
                    return;
                }

                if (progress >= 1f)
                {
                    // Check if we should loop
                    if (loops == -1 || currentLoop < loops)
                    {
                        currentLoop++;
                        // Reset for next loop
                        elapsedTime = 0f;
                        elapsedDelay = 0f;
                        
                        // Handle different loop types
                        if (loopType == LoopType.Yoyo)
                        {
                            // Swap start and target values for yoyo effect
                            float temp = startValue;
                            startValue = targetValue;
                            targetValue = temp;
                        }
                        else if (loopType == LoopType.Incremental)
                        {
                            // For incremental, we need to adjust the target
                            // This is a simplified implementation
                            startValue = targetValue;
                            targetValue = startValue + (targetValue - startValue);
                        }
                        // For LoopType.Restart, we just reset to original values
                        else
                        {
                            // Reset to original values (handled in constructor)
                        }
                    }
                    else
                    {
                        isCompleted = true;
                        onComplete?.Invoke();
                    }
                }
            }

            private float ApplyEase(float t, EaseType ease)
            {
                switch (ease)
                {
                    case EaseType.Linear:
                        return t;
                    case EaseType.InSine:
                        return 1f - Mathf.Cos(t * Mathf.PI / 2f);
                    case EaseType.OutSine:
                        return Mathf.Sin(t * Mathf.PI / 2f);
                    case EaseType.InOutSine:
                        return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
                    case EaseType.InQuad:
                        return t * t;
                    case EaseType.OutQuad:
                        return 1f - (1f - t) * (1f - t);
                    case EaseType.InOutQuad:
                        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
                    case EaseType.InCubic:
                        return t * t * t;
                    case EaseType.OutCubic:
                        return 1f - Mathf.Pow(1f - t, 3f);
                    case EaseType.InOutCubic:
                        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
                    case EaseType.InQuart:
                        return t * t * t * t;
                    case EaseType.OutQuart:
                        return 1f - Mathf.Pow(1f - t, 4f);
                    case EaseType.InOutQuart:
                        return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
                    case EaseType.InQuint:
                        return t * t * t * t * t;
                    case EaseType.OutQuint:
                        return 1f - Mathf.Pow(1f - t, 5f);
                    case EaseType.InOutQuint:
                        return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
                    case EaseType.InExpo:
                        return t == 0f ? 0f : Mathf.Pow(2f, 10f * (t - 1f));
                    case EaseType.OutExpo:
                        return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                    case EaseType.InOutExpo:
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        return t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;
                    case EaseType.InCirc:
                        return 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
                    case EaseType.OutCirc:
                        return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
                    case EaseType.InOutCirc:
                        return t < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f :
                                          (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
                    case EaseType.InElastic:
                        float c4 = (2f * Mathf.PI) / 3f;
                        return t == 0f ? 0f : t == 1f ? 1f : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
                    case EaseType.OutElastic:
                        c4 = (2f * Mathf.PI) / 3f;
                        return t == 0f ? 0f : t == 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                    case EaseType.InOutElastic:
                        float c5 = (2f * Mathf.PI) / 4.5f;
                        return t == 0f ? 0f : t == 1f ? 1f : t < 0.5f
                            ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f
                            : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;
                    case EaseType.InBack:
                        float c1 = 1.70158f;
                        float c3 = c1 + 1f;
                        return c3 * t * t * t - c1 * t * t;
                    case EaseType.OutBack:
                        c1 = 1.70158f;
                        c3 = c1 + 1f;
                        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                    case EaseType.InOutBack:
                        c1 = 1.70158f;
                        float c2 = c1 * 1.525f;
                        return t < 0.5f ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f :
                                          (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
                    case EaseType.InBounce:
                        return 1f - BounceOut(1f - t);
                    case EaseType.OutBounce:
                        return BounceOut(t);
                    case EaseType.InOutBounce:
                        return t < 0.5f ? (1f - BounceOut(1f - 2f * t)) / 2f : (1f + BounceOut(2f * t - 1f)) / 2f;
                    default:
                        return t;
                }
            }

            private float BounceOut(float t)
            {
                float n1 = 7.5625f;
                float d1 = 2.75f;

                if (t < 1f / d1)
                {
                    return n1 * t * t;
                }
                else if (t < 2f / d1)
                {
                    return n1 * (t -= 1.5f / d1) * t + 0.75f;
                }
                else if (t < 2.5f / d1)
                {
                    return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                }
                else
                {
                    return n1 * (t -= 2.625f / d1) * t + 0.984375f;
                }
            }
        }

        private void Update()
        {
            for (int i = activeTweens.Count - 1; i >= 0; i--)
            {
                activeTweens[i].Update();
                if (activeTweens[i].isCompleted)
                {
                    activeTweens.RemoveAt(i);
                }
            }
        }

        public static TweenHandle To(Func<float> getter, Action<float> setter, float targetValue, float duration)
        {
            var tweenData = new TweenData
            {
                getter = getter,
                setter = setter,
                startValue = getter(),
                targetValue = targetValue,
                duration = duration,
                elapsedTime = 0f,
                easeType = EaseType.Linear,
                onComplete = null,
                isCompleted = false,
                delay = 0f,
                elapsedDelay = 0f,
                useUnscaledTime = false,
                targetObject = null, // Default value, will be set to specific object in extension methods
                loops = 0,
                loopType = LoopType.Restart,
                currentLoop = 0
            };

            Instance.activeTweens.Add(tweenData);
            return new TweenHandle(tweenData);
        }

        // Single Tween Vector2 support (interpolate within 0..1 progress)
        public static TweenHandle ToVector2(Func<Vector2> getter, Action<Vector2> setter, Vector2 targetValue, float duration, UnityEngine.Object target)
        {
            Vector2 start = getter();
            var tweenData = new TweenData
            {
                getter = () => 0f,
                setter = v => setter(Vector2.Lerp(start, targetValue, v)),
                startValue = 0f,
                targetValue = 1f,
                duration = duration,
                elapsedTime = 0f,
                easeType = EaseType.Linear,
                onComplete = null,
                isCompleted = false,
                delay = 0f,
                elapsedDelay = 0f,
                useUnscaledTime = false,
                targetObject = target,
                loops = 0,
                loopType = LoopType.Restart,
                currentLoop = 0
            };
            Instance.activeTweens.Add(tweenData);
            return new TweenHandle(tweenData);
        }

        // Single Tween Vector3 support (interpolate within 0..1 progress)
        public static TweenHandle ToVector3(Func<Vector3> getter, Action<Vector3> setter, Vector3 targetValue, float duration, UnityEngine.Object target)
        {
            Vector3 start = getter();
            var tweenData = new TweenData
            {
                getter = () => 0f,
                setter = v => setter(Vector3.Lerp(start, targetValue, v)),
                startValue = 0f,
                targetValue = 1f,
                duration = duration,
                elapsedTime = 0f,
                easeType = EaseType.Linear,
                onComplete = null,
                isCompleted = false,
                delay = 0f,
                elapsedDelay = 0f,
                useUnscaledTime = false,
                targetObject = target,
                loops = 0,
                loopType = LoopType.Restart,
                currentLoop = 0
            };
            Instance.activeTweens.Add(tweenData);
            return new TweenHandle(tweenData);
        }

        // Vector2 legacy (no longer using multiple tweens, keeping method signature)
        public static TweenHandle To(Func<Vector2> getter, Action<Vector2> setter, Vector2 targetValue, float duration)
        {
            return ToVector2(getter, setter, targetValue, duration, null);
        }

        // Vector3 legacy (no longer using multiple tweens, keeping method signature)
        public static TweenHandle To(Func<Vector3> getter, Action<Vector3> setter, Vector3 targetValue, float duration)
        {
            return ToVector3(getter, setter, targetValue, duration, null);
        }

        public static void KillAll()
        {
            Instance.activeTweens.Clear();
        }
    }

    public class TweenHandle
    {
        private SmoothTween.TweenData tweenData;

        public TweenHandle(SmoothTween.TweenData data)
        {
            tweenData = data;
        }

        public TweenHandle SetEase(EaseType ease)
        {
            if (tweenData != null)
                tweenData.easeType = ease;
            return this;
        }

        public TweenHandle OnComplete(Action callback)
        {
            if (tweenData != null)
                tweenData.onComplete += callback; // additive
            return this;
        }

        public TweenHandle From(float fromValue)
        {
            if (tweenData != null)
            {
                tweenData.startValue = fromValue;
                tweenData.setter?.Invoke(fromValue); // Set initial value
            }
            return this;
        }

        public TweenHandle SetDelay(float delay)
        {
            if (tweenData != null)
                tweenData.delay = Mathf.Max(0f, delay);
            return this;
        }

        public TweenHandle SetUpdate(bool isIndependentUpdate)
        {
            if (tweenData != null)
                tweenData.useUnscaledTime = isIndependentUpdate;
            return this;
        }

        public TweenHandle SetTargetObject(UnityEngine.Object target)
        {
            if (tweenData != null)
                tweenData.targetObject = target;
            return this;
        }

        public TweenHandle SetLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            // For single tweens, we need to implement loop logic
            if (tweenData != null)
            {
                // Store loop information in the tween data
                tweenData.loops = loops;
                tweenData.loopType = loopType;
                tweenData.currentLoop = 0;
            }
            return this;
        }

        public TweenHandle Pause()
        {
            if (tweenData != null)
                tweenData.paused = true;
            return this;
        }

        public TweenHandle Resume()
        {
            if (tweenData != null)
                tweenData.paused = false;
            return this;
        }

        public void Kill()
        {
            if (tweenData != null)
                tweenData.isCompleted = true;
        }

        public IEnumerator WaitForCompletion()
        {
            if (tweenData != null)
            {
                while (!tweenData.isCompleted)
                {
                    yield return null;
                }
            }
        }
    }

    // Static helper class providing DOTween-like API
    public static class Ease
    {
        public static EaseType OutCubic = EaseType.OutCubic;
        public static EaseType InCubic = EaseType.InCubic;
        public static EaseType InOutCubic = EaseType.InOutCubic;
        public static EaseType OutQuad = EaseType.OutQuad;
        public static EaseType InQuad = EaseType.InQuad;
        public static EaseType InOutQuad = EaseType.InOutQuad;
        public static EaseType OutSine = EaseType.OutSine;
        public static EaseType InSine = EaseType.InSine;
        public static EaseType InOutSine = EaseType.InOutSine;
        public static EaseType OutBack = EaseType.OutBack;
        public static EaseType InBack = EaseType.InBack;
        public static EaseType InOutBack = EaseType.InOutBack;
        public static EaseType OutBounce = EaseType.OutBounce;
        public static EaseType InBounce = EaseType.InBounce;
        public static EaseType InOutBounce = EaseType.InOutBounce;
        public static EaseType OutElastic = EaseType.OutElastic;
        public static EaseType InElastic = EaseType.InElastic;
        public static EaseType InOutElastic = EaseType.InOutElastic;
        public static EaseType Linear = EaseType.Linear;
    }

    // Sequence class to handle sequential animations
    public class Sequence
    {
        private List<TweenHandle> tweenHandles = new List<TweenHandle>();
        private List<float> delays = new List<float>();
        private int currentTweenIndex = 0;
        private bool isPlaying = false;
        private bool isCompleted = false;
        private Action onCompleteCallback;
        private int loops = 0; // 0 = play once, -1 = infinite
        private int currentLoop = 0;

        public Sequence Append(TweenHandle tweenHandle)
        {
            if (tweenHandle != null)
            {
                tweenHandle.Pause(); // pause until Play
                tweenHandles.Add(tweenHandle);
            }
            delays.Add(0f);
            return this;
        }

        public Sequence AppendInterval(float interval)
        {
            tweenHandles.Add(null);
            delays.Add(interval);
            return this;
        }

        public Sequence Play()
        {
            isPlaying = true;
            PlayNextTween();
            return this;
        }

        public Sequence OnComplete(Action callback)
        {
            onCompleteCallback = callback;
            return this;
        }

        public Sequence SetLoops(int loops)
        {
            this.loops = loops;
            return this;
        }

        private void PlayNextTween()
        {
            if (currentTweenIndex >= tweenHandles.Count)
            {
                // Check if we should loop
                if (loops == -1 || currentLoop < loops)
                {
                    currentLoop++;
                    currentTweenIndex = 0;
                    PlayNextTween();
                    return;
                }
                
                isCompleted = true;
                onCompleteCallback?.Invoke();
                return;
            }

            var currentHandle = tweenHandles[currentTweenIndex];
            var currentDelay = delays[currentTweenIndex];

            if (currentHandle == null)
            {
                // This is an interval
                SmoothTween.Instance.StartCoroutine(WaitAndContinue(currentDelay));
            }
            else
            {
                // This is a tween
                currentHandle.Resume(); // resume this tween now
                currentHandle.OnComplete(() => {
                    currentTweenIndex++;
                    PlayNextTween();
                });
            }
        }

        private IEnumerator WaitAndContinue(float delay)
        {
            yield return new WaitForSeconds(delay);
            currentTweenIndex++;
            PlayNextTween();
        }

        public void Kill()
        {
            foreach (var handle in tweenHandles)
            {
                handle?.Kill();
            }
            isCompleted = true;
        }
    }

    // DOTween static class for compatibility
    public static class DOTween
    {
        public static TweenHandle To(Func<float> getter, Action<float> setter, float targetValue, float duration)
        {
            return SmoothTween.To(getter, setter, targetValue, duration);
        }

        public static Sequence Sequence()
        {
            return new Sequence();
        }

        public static void Kill()
        {
            SmoothTween.KillAll();
        }

        public static void KillAll()
        {
            SmoothTween.KillAll();
        }
    }

    // ATween compatibility class
    public static class ATween
    {
        public static System.Collections.Hashtable Hash(params object[] args)
        {
            var hash = new System.Collections.Hashtable();
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    hash[args[i]] = args[i + 1];
                }
            }
            return hash;
        }

        private static EaseType ParseEaseType(string easeTypeStr)
        {
            switch (easeTypeStr)
            {
                case "easeOutExpo":
                    return EaseType.OutExpo;
                case "easeInExpo":
                    return EaseType.InExpo;
                case "easeInOutExpo":
                    return EaseType.InOutExpo;
                case "easeOutElastic":
                    return EaseType.OutElastic;
                case "easeInElastic":
                    return EaseType.InElastic;
                case "easeInOutElastic":
                    return EaseType.InOutElastic;
                case "easeOutBack":
                    return EaseType.OutBack;
                case "easeInBack":
                    return EaseType.InBack;
                case "easeInOutBack":
                    return EaseType.InOutBack;
                case "easeOutBounce":
                    return EaseType.OutBounce;
                case "easeInBounce":
                    return EaseType.InBounce;
                case "easeInOutBounce":
                    return EaseType.InOutBounce;
                default:
                    return EaseType.Linear;
            }
        }

        public static TweenHandle MoveTo(GameObject target, System.Collections.Hashtable args)
        {
            var transform = target.transform;
            bool isLocal = args.ContainsKey("islocal") && System.Convert.ToBoolean(args["islocal"]);
            float duration = args.ContainsKey("time") ? System.Convert.ToSingle(args["time"]) : 1f;
            bool ignoreTimeScale = args.ContainsKey("ignoretimescale") && System.Convert.ToBoolean(args["ignoretimescale"]);
            
            Vector3 targetPos = isLocal ? transform.localPosition : transform.position;
            
            if (args.ContainsKey("x"))
                targetPos.x = System.Convert.ToSingle(args["x"]);
            if (args.ContainsKey("y"))
                targetPos.y = System.Convert.ToSingle(args["y"]);
            if (args.ContainsKey("z"))
                targetPos.z = System.Convert.ToSingle(args["z"]);

            TweenHandle handle;
            if (isLocal)
                handle = transform.DOLocalMove(targetPos, duration);
            else
                handle = transform.DOMove(targetPos, duration);

            if (ignoreTimeScale)
                handle.SetUpdate(true);

            // Handle ease type
            if (args.ContainsKey("easeType"))
            {
                string easeTypeStr = (string)args["easeType"];
                handle.SetEase(ParseEaseType(easeTypeStr));
            }

            // Handle callbacks
            if (args.ContainsKey("oncomplete") && args.ContainsKey("oncompletetarget"))
            {
                string methodName = (string)args["oncomplete"];
                GameObject targetGO = (GameObject)args["oncompletetarget"];
                handle.OnComplete(() => {
                    if (targetGO != null)
                        targetGO.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                });
            }

            return handle;
        }

        public static TweenHandle ScaleTo(GameObject target, System.Collections.Hashtable args)
        {
            float duration = args.ContainsKey("time") ? System.Convert.ToSingle(args["time"]) : 1f;
            Vector3 targetScale = Vector3.one;
            
            if (args.ContainsKey("scale"))
            {
                var scaleValue = args["scale"];
                if (scaleValue is Vector3)
                    targetScale = (Vector3)scaleValue;
                else if (scaleValue is float)
                    targetScale = Vector3.one * (float)scaleValue;
            }

            var handle = target.transform.DOScale(targetScale, duration);

            // Handle callbacks
            if (args.ContainsKey("oncomplete") && args.ContainsKey("oncompletetarget"))
            {
                string methodName = (string)args["oncomplete"];
                GameObject targetGO = (GameObject)args["oncompletetarget"];
                handle.OnComplete(() => {
                    if (targetGO != null)
                        targetGO.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                });
            }

            return handle;
        }

        public static TweenHandle ValueTo(GameObject target, System.Collections.Hashtable args)
        {
            float from = args.ContainsKey("from") ? System.Convert.ToSingle(args["from"]) : 0f;
            float to = args.ContainsKey("to") ? System.Convert.ToSingle(args["to"]) : 1f;
            float duration = args.ContainsKey("time") ? System.Convert.ToSingle(args["time"]) : 1f;
            bool ignoreTimeScale = args.ContainsKey("ignoretimescale") && System.Convert.ToBoolean(args["ignoretimescale"]);

            var handle = SmoothTween.To(() => from, v => {
                // Handle onupdate callback
                if (args.ContainsKey("onupdate") && args.ContainsKey("onupdatetarget"))
                {
                    string methodName = (string)args["onupdate"];
                    GameObject targetGO = (GameObject)args["onupdatetarget"];
                    if (targetGO != null)
                        targetGO.SendMessage(methodName, v, SendMessageOptions.DontRequireReceiver);
                }
            }, to, duration);

            if (ignoreTimeScale)
                handle.SetUpdate(true);

            // Handle oncomplete callback
            if (args.ContainsKey("oncomplete") && args.ContainsKey("oncompletetarget"))
            {
                string methodName = (string)args["oncomplete"];
                GameObject targetGO = (GameObject)args["oncompletetarget"];
                object completeparams = args.ContainsKey("oncompleteparams") ? args["oncompleteparams"] : null;
                
                handle.OnComplete(() => {
                    if (targetGO != null)
                    {
                        if (completeparams != null)
                            targetGO.SendMessage(methodName, completeparams, SendMessageOptions.DontRequireReceiver);
                        else
                            targetGO.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                    }
                });
            }

            return handle;
        }

        public static void ShakePosition(GameObject obj, System.Collections.Hashtable args)
        {
            // This is a simplified shake implementation
            float duration = args.ContainsKey("time") ? System.Convert.ToSingle(args["time"]) : 1f;
            float strength = args.ContainsKey("amount") ? System.Convert.ToSingle(args["amount"]) : 1f;
            
            Vector3 originalPos = obj.transform.position;
            SmoothTween.To(() => 0f, v => {
                Vector3 shake = new Vector3(
                    UnityEngine.Random.Range(-strength, strength) * (1f - v),
                    UnityEngine.Random.Range(-strength, strength) * (1f - v),
                    0f
                );
                obj.transform.position = originalPos + shake;
            }, 1f, duration).OnComplete(() => {
                obj.transform.position = originalPos;
            });
        }
    }

    // UI and Transform extension methods
    public static class SmoothTweenExtensions
    {
        public static TweenHandle DOAnchorPos(this RectTransform rectTransform, Vector2 targetPosition, float duration)
        {
            var handle = SmoothTween.ToVector2(
                () => rectTransform.anchoredPosition,
                pos => rectTransform.anchoredPosition = pos,
                targetPosition,
                duration,
                rectTransform
            );
            return handle;
        }

        public static TweenHandle DOFade(this CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            var handle = SmoothTween.To(
                () => canvasGroup.alpha,
                alpha => canvasGroup.alpha = alpha,
                targetAlpha,
                duration
            );
            handle.SetTargetObject(canvasGroup);
            return handle;
        }

        public static TweenHandle DOFade(this UnityEngine.UI.Image image, float targetAlpha, float duration)
        {
            var handle = SmoothTween.To(
                () => image.color.a,
                alpha => {
                    var color = image.color;
                    color.a = alpha;
                    image.color = color;
                },
                targetAlpha,
                duration
            );
            handle.SetTargetObject(image);
            return handle;
        }

        public static TweenHandle DOColor(this SpriteRenderer spriteRenderer, Color targetColor, float duration)
        {
            Color startColor = spriteRenderer.color;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    spriteRenderer.color = new Color(
                        Mathf.Lerp(startColor.r, targetColor.r, v),
                        Mathf.Lerp(startColor.g, targetColor.g, v),
                        Mathf.Lerp(startColor.b, targetColor.b, v),
                        Mathf.Lerp(startColor.a, targetColor.a, v)
                    );
                },
                1f,
                duration
            );
            handle.SetTargetObject(spriteRenderer);
            return handle;
        }

        public static TweenHandle DOColor(this UnityEngine.UI.Image image, Color targetColor, float duration)
        {
            Color startColor = image.color;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    image.color = new Color(
                        Mathf.Lerp(startColor.r, targetColor.r, v),
                        Mathf.Lerp(startColor.g, targetColor.g, v),
                        Mathf.Lerp(startColor.b, targetColor.b, v),
                        Mathf.Lerp(startColor.a, targetColor.a, v)
                    );
                },
                1f,
                duration
            );
            handle.SetTargetObject(image);
            return handle;
        }

        public static TweenHandle DOFade(this SpriteRenderer spriteRenderer, float targetAlpha, float duration)
        {
            float startAlpha = spriteRenderer.color.a;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    var c = spriteRenderer.color;
                    c.a = Mathf.Lerp(startAlpha, targetAlpha, v);
                    spriteRenderer.color = c;
                },
                1f,
                duration
            );
            handle.SetTargetObject(spriteRenderer);
            return handle;
        }

        public static TweenHandle DOMove(this Transform transform, Vector3 targetPosition, float duration)
        {
            return SmoothTween.ToVector3(
                () => transform.position,
                pos => transform.position = pos,
                targetPosition,
                duration,
                transform
            );
        }

        public static TweenHandle DOLocalMove(this Transform transform, Vector3 targetLocalPosition, float duration)
        {
            return SmoothTween.ToVector3(
                () => transform.localPosition,
                pos => transform.localPosition = pos,
                targetLocalPosition,
                duration,
                transform
            );
        }

        public static TweenHandle DOMoveX(this Transform transform, float x, float duration)
        {
            return transform.DOMove(new Vector3(x, transform.position.y, transform.position.z), duration);
        }

        public static TweenHandle DOMoveY(this Transform transform, float y, float duration)
        {
            return transform.DOMove(new Vector3(transform.position.x, y, transform.position.z), duration);
        }

        public static TweenHandle DOMoveZ(this Transform transform, float z, float duration)
        {
            return transform.DOMove(new Vector3(transform.position.x, transform.position.y, z), duration);
        }

        public static TweenHandle DOLocalRotate(this Transform transform, Vector3 targetEulerAngles, float duration)
        {
            return SmoothTween.ToVector3(
                () => transform.localEulerAngles,
                rot => transform.localEulerAngles = rot,
                targetEulerAngles,
                duration,
                transform
            );
        }

        public static TweenHandle DORotate(this Transform transform, Vector3 targetEulerAngles, float duration)
        {
            return SmoothTween.ToVector3(
                () => transform.eulerAngles,
                rot => transform.eulerAngles = rot,
                targetEulerAngles,
                duration,
                transform
            );
        }

        public static TweenHandle DOScale(this Transform transform, Vector3 targetScale, float duration)
        {
            return SmoothTween.ToVector3(
                () => transform.localScale,
                s => transform.localScale = s,
                targetScale,
                duration,
                transform
            );
        }

        public static TweenHandle DOScale(this Transform transform, Vector2 targetScale, float duration)
        {
            return transform.DOScale(new Vector3(targetScale.x, targetScale.y, transform.localScale.z), duration);
        }

        public static TweenHandle DOScale(this Transform transform, float uniformScale, float duration)
        {
            return transform.DOScale(new Vector3(uniformScale, uniformScale, uniformScale), duration);
        }

        public static TweenHandle DOLocalMoveX(this Transform transform, float x, float duration)
        {
            return transform.DOLocalMove(new Vector3(x, transform.localPosition.y, transform.localPosition.z), duration);
        }

        public static TweenHandle DOLocalMoveY(this Transform transform, float y, float duration)
        {
            return transform.DOLocalMove(new Vector3(transform.localPosition.x, y, transform.localPosition.z), duration);
        }

        public static TweenHandle DOLocalMoveZ(this Transform transform, float z, float duration)
        {
            return transform.DOLocalMove(new Vector3(transform.localPosition.x, transform.localPosition.y, z), duration);
        }

        public static TweenHandle DOShakeRotation(this Transform transform, float duration, float strength, int vibrato = 10)
        {
            Vector3 originalRotation = transform.eulerAngles;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    float shakeAmount = strength * (1f - v);
                    float noise = Mathf.PerlinNoise(Time.time * vibrato, 0f) * 2f - 1f;
                    Vector3 shake = new Vector3(0, 0, noise * shakeAmount);
                    transform.eulerAngles = originalRotation + shake;
                },
                1f,
                duration
            );
            handle.SetTargetObject(transform);
            handle.OnComplete(() => transform.eulerAngles = originalRotation);
            return handle;
        }

        public static TweenHandle DOShakePosition(this Transform transform, float duration, float strength = 1f, int vibrato = 10)
        {
            Vector3 originalPosition = transform.position;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    float shakeAmount = strength * (1f - v);
                    Vector3 shake = new Vector3(
                        UnityEngine.Random.Range(-shakeAmount, shakeAmount),
                        UnityEngine.Random.Range(-shakeAmount, shakeAmount),
                        0f
                    );
                    transform.position = originalPosition + shake;
                },
                1f,
                duration
            );
            handle.SetTargetObject(transform);
            handle.OnComplete(() => transform.position = originalPosition);
            return handle;
        }

        public static TweenHandle DOShakeScale(this Transform transform, float duration, float strength = 1f, int vibrato = 10)
        {
            Vector3 originalScale = transform.localScale;
            var handle = SmoothTween.To(
                () => 0f,
                v => {
                    float shakeAmount = strength * (1f - v);
                    Vector3 shake = new Vector3(
                        UnityEngine.Random.Range(-shakeAmount, shakeAmount),
                        UnityEngine.Random.Range(-shakeAmount, shakeAmount),
                        UnityEngine.Random.Range(-shakeAmount, shakeAmount)
                    );
                    transform.localScale = originalScale + shake;
                },
                1f,
                duration
            );
            handle.SetTargetObject(transform);
            handle.OnComplete(() => transform.localScale = originalScale);
            return handle;
        }

        public static TweenHandle DOScaleX(this Transform transform, float x, float duration)
        {
            return transform.DOScale(new Vector3(x, transform.localScale.y, transform.localScale.z), duration);
        }

        public static TweenHandle DOScaleY(this Transform transform, float y, float duration)
        {
            return transform.DOScale(new Vector3(transform.localScale.x, y, transform.localScale.z), duration);
        }

        public static TweenHandle DOScaleZ(this Transform transform, float z, float duration)
        {
            return transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, z), duration);
        }
    }
