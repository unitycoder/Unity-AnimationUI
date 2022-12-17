using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AnimationUI : MonoBehaviour
{
    [HideInInspector] public float TotalDuration = 0; //Value automatically taken care of by AnimationUIInspector
    public Sequence[] AnimationSequence;
    [HideInInspector] public bool PlayOnStart = false;
    void Start()
    {
#if UNITY_EDITOR
        if(Application.isPlaying)
#endif
        foreach(Sequence sequence in AnimationSequence)sequence.Init();
        if(PlayOnStart)StartCoroutine(PlayAnimation());
    }
    public void Play() => StartCoroutine(PlayAnimation());
    public void PlayReversed() => StartCoroutine(PlayReversedAnimation());
    IEnumerator PlayAnimation()
    {
        Singleton.LoadSingleton();
        for(int i = 0; i < atTimeEvents.Count; i++)StartCoroutine(AtTimeEvent(atTimeEvents[i], atTimes[i])); //Function to call at time

        foreach(Sequence sequence in AnimationSequence)
        {
            if(sequence.SequenceType == Sequence.Type.Animation)
            {
                if(sequence.TargetComp == null)
                {
                    Debug.Log("Please assign Target for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }
                
                if(sequence.TargetType == Sequence.ObjectType.RectTransform)
                {
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchoredPosition))
                        StartCoroutine(TaskAnchoredPosition(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchoredPositionStart, sequence.AnchoredPositionEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalEulerAngles))
                        StartCoroutine(TaskLocalEulerAngles(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.LocalEulerAnglesStart, sequence.LocalEulerAnglesEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalScale))
                        StartCoroutine(TaskLocalScale(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.LocalScaleStart, sequence.LocalScaleEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMax))
                        StartCoroutine(TaskAnchorMax(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchorMaxStart, sequence.AnchorMaxEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMin))
                        StartCoroutine(TaskAnchorMin(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchorMinStart, sequence.AnchorMinEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.SizeDelta))
                        StartCoroutine(TaskSizeDelta(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.SizeDeltaStart, sequence.SizeDeltaEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.Pivot))
                        StartCoroutine(TaskPivot(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.PivotStart, sequence.PivotEnd, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Transform)
                {
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalPosition))
                        StartCoroutine(TaskLocalPosition(sequence.TargetComp.transform, 
                            sequence.LocalPositionStart, sequence.LocalPositionEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalScale))
                        StartCoroutine(TaskLocalEulerAngles(sequence.TargetComp.transform, 
                            sequence.LocalEulerAnglesStart, sequence.LocalEulerAnglesEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalEulerAngles))
                        StartCoroutine(TaskLocalScale(sequence.TargetComp.transform, 
                            sequence.LocalScaleStart, sequence.LocalScaleEnd, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Image)
                {
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.Color))
                        StartCoroutine(TaskColor(sequence.TargetComp.GetComponent<Image>(), 
                            sequence.ColorStart, sequence.ColorEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.FillAmount))
                        StartCoroutine(TaskFillAmount(sequence.TargetComp.GetComponent<Image>(), 
                            sequence.FillAmountStart, sequence.FillAmountEnd, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.CanvasGroup)
                {
                    if(sequence.TargetCgTask.HasFlag(Sequence.CgTask.Alpha))
                        StartCoroutine(TaskAlpha(sequence.TargetComp.GetComponent<CanvasGroup>(), 
                            sequence.AlphaStart, sequence.AlphaEnd, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Camera)
                {
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.BackgroundColor))
                        StartCoroutine(TaskBackgroundColor(sequence.TargetComp.GetComponent<Camera>(), 
                            sequence.BackgroundColorStart, sequence.BackgroundColorEnd, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.OrthographicSize))
                        StartCoroutine(TaskOrthographicSize(sequence.TargetComp.GetComponent<Camera>(), 
                            sequence.OrthographicSizeStart, sequence.OrthographicSizeEnd, sequence.Duration, sequence.EaseFunction
                        ));
                }
            }
            else if(sequence.SequenceType == Sequence.Type.Wait)
            {
                yield return new WaitForSecondsRealtime(sequence.Duration);
            }
            else if(sequence.SequenceType == Sequence.Type.SetActiveAllInput)
            {
                Singleton.Instance.Game.SetActiveAllInput(sequence.IsActivating);
            }
            else if(sequence.SequenceType == Sequence.Type.SetActive)
            {
                if(sequence.Target == null)
                {
                    // Debug.LogError("Please assign Target for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }
                sequence.Target.SetActive(sequence.IsActivating);
            }
            else if(sequence.SequenceType == Sequence.Type.SFX)
            {
                if(sequence.SFX == null)
                {
                    // Debug.LogWarning("Please assign SFX for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }
                Singleton.Instance.Audio.PlaySound(sequence.SFX);
            }
            else if(sequence.SequenceType == Sequence.Type.LoadScene)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sequence.SceneToLoad);
            }
            else if(sequence.SequenceType == Sequence.Type.UnityEvent)
            {
                sequence.Event?.Invoke();
            }
        }

        atEndEvents?.Invoke(); //Function to call at end

        atEndEvents = null;
        atTimeEvents.Clear();
        atTimes.Clear();
    }

    IEnumerator ReverseArrayAtTime(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        Array.Reverse(AnimationSequence);
    }
    IEnumerator PlayReversedAnimation()
    {
        Singleton.LoadSingleton();
        Array.Reverse(AnimationSequence);
        ReverseArrayAtTime(TotalDuration);

        for(int i = 0; i < atTimeEvents.Count; i++)StartCoroutine(AtTimeEvent(atTimeEvents[i], atTimes[i])); //Function to call at time
        // for(int i = atTimeEvents.Count-1; i >= 0; i++)StartCoroutine(AtTimeEvent(atTimeEvents[i], atTimes[i])); //Function to call at time

        foreach(Sequence sequence in AnimationSequence)
        {
            if(sequence.SequenceType == Sequence.Type.Animation)
            {
                if(sequence.TargetComp == null)
                {
                    Debug.Log("Please assign Target for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }

                if(sequence.TargetType == Sequence.ObjectType.RectTransform)
                {
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchoredPosition))
                        StartCoroutine(TaskAnchoredPosition(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchoredPositionEnd, sequence.AnchoredPositionStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalEulerAngles))
                        StartCoroutine(TaskLocalEulerAngles(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.LocalEulerAnglesEnd, sequence.LocalEulerAnglesStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalScale))
                        StartCoroutine(TaskLocalScale(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.LocalScaleEnd, sequence.LocalScaleStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMax))
                        StartCoroutine(TaskAnchorMax(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchorMaxEnd, sequence.AnchorMaxStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMin))
                        StartCoroutine(TaskAnchorMin(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.AnchorMinEnd, sequence.AnchorMinStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.SizeDelta))
                        StartCoroutine(TaskSizeDelta(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.SizeDeltaEnd, sequence.SizeDeltaStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.Pivot))
                        StartCoroutine(TaskPivot(sequence.TargetComp.GetComponent<RectTransform>(), 
                            sequence.PivotEnd, sequence.PivotStart, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Transform)
                {
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalPosition))
                        StartCoroutine(TaskLocalPosition(sequence.TargetComp.transform, 
                            sequence.LocalPositionEnd, sequence.LocalPositionStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalScale))
                        StartCoroutine(TaskLocalEulerAngles(sequence.TargetComp.transform, 
                            sequence.LocalEulerAnglesEnd, sequence.LocalEulerAnglesStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalEulerAngles))
                        StartCoroutine(TaskLocalScale(sequence.TargetComp.transform, 
                            sequence.LocalScaleEnd, sequence.LocalScaleStart, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Image)
                {
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.Color))
                        StartCoroutine(TaskColor(sequence.TargetComp.GetComponent<Image>(), 
                            sequence.ColorEnd, sequence.ColorStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.FillAmount))
                        StartCoroutine(TaskFillAmount(sequence.TargetComp.GetComponent<Image>(), 
                            sequence.FillAmountEnd, sequence.FillAmountStart, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.CanvasGroup)
                {
                    if(sequence.TargetCgTask.HasFlag(Sequence.CgTask.Alpha))
                        StartCoroutine(TaskAlpha(sequence.TargetComp.GetComponent<CanvasGroup>(), 
                            sequence.AlphaEnd, sequence.AlphaStart, sequence.Duration, sequence.EaseFunction
                        ));
                }
                else if(sequence.TargetType == Sequence.ObjectType.Camera)
                {
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.BackgroundColor))
                        StartCoroutine(TaskBackgroundColor(sequence.TargetComp.GetComponent<Camera>(), 
                            sequence.BackgroundColorEnd, sequence.BackgroundColorStart, sequence.Duration, sequence.EaseFunction
                        ));
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.OrthographicSize))
                        StartCoroutine(TaskOrthographicSize(sequence.TargetComp.GetComponent<Camera>(), 
                            sequence.OrthographicSizeEnd, sequence.OrthographicSizeStart, sequence.Duration, sequence.EaseFunction
                        ));
                }
            }
            else if(sequence.SequenceType == Sequence.Type.Wait)
            {
                yield return new WaitForSecondsRealtime(sequence.Duration);
            }
            else if(sequence.SequenceType == Sequence.Type.SetActiveAllInput)
            {
                Singleton.Instance.Game.SetActiveAllInput(!sequence.IsActivating);
            }
            else if(sequence.SequenceType == Sequence.Type.SetActive)
            {
                if(sequence.Target == null)
                {
                    // Debug.LogError("Please assign Target for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }
                sequence.Target.SetActive(!sequence.IsActivating);
            }
            else if(sequence.SequenceType == Sequence.Type.SFX)
            {
                if(sequence.SFX == null)
                {
                    // Debug.LogWarning("Please assign SFX for Sequence at "+sequence.StartTime.ToString()+"s");
                    continue;
                }
                Singleton.Instance.Audio.PlaySound(sequence.SFX);
            }
            else if(sequence.SequenceType == Sequence.Type.LoadScene)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sequence.SceneToLoad);
            }
            else if(sequence.SequenceType == Sequence.Type.UnityEvent)
            {
                sequence.Event?.Invoke();
            }
        }

        atEndEvents?.Invoke(); //Function to call at end

        Array.Reverse(AnimationSequence);
        atEndEvents = null;
        atTimeEvents.Clear();
        atTimes.Clear();
    }

#region Tasks

#region RectTransform
    IEnumerator TaskAnchoredPosition(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.anchoredPosition = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.anchoredPosition = end;
    }
    IEnumerator TaskLocalScale(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.localScale = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.localScale = end;
    }
    IEnumerator TaskLocalEulerAngles(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.localEulerAngles = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.localEulerAngles = end;
    }
    IEnumerator TaskAnchorMax(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.anchorMax = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.anchorMax = end;
    }
    IEnumerator TaskAnchorMin(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.anchorMin = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.anchorMin = end;
    }
    IEnumerator TaskSizeDelta(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.sizeDelta = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.sizeDelta = end;
    }
    IEnumerator TaskPivot(RectTransform rt, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            rt.pivot = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        rt.pivot = end;
    }
#endregion RectTransform

#region TransformTask
    IEnumerator TaskLocalPosition(Transform trans, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            trans.localPosition = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        trans.localPosition = end;
    }
    IEnumerator TaskLocalScale(Transform trans, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            trans.localScale = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        trans.localScale = end;
    }
    IEnumerator TaskLocalEulerAngles(Transform trans, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            trans.localEulerAngles = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        trans.localEulerAngles = end;
    }
#endregion TransformTask

#region ImageTask
    IEnumerator TaskColor(Image img, Color start, Color end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            img.color = Color.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        img.color = end;
    }
    IEnumerator TaskFillAmount(Image img, float start, float end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            img.fillAmount = Mathf.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        img.fillAmount = end;
    }
#endregion ImageTask

#region CanvasGroupTask
    IEnumerator TaskAlpha(CanvasGroup cg, float start, float end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            cg.alpha = Mathf.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        cg.alpha = end;
    }
#endregion CanvasGroupTask

#region ImageTask
    IEnumerator TaskBackgroundColor(Camera cam, Color start, Color end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            cam.backgroundColor = Color.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        cam.backgroundColor = end;
    }
    IEnumerator TaskOrthographicSize(Camera cam, float start, float end, float duration, Ease.Function easeFunction)
    {
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            cam.orthographicSize = Mathf.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        cam.orthographicSize = end;
    }
#endregion ImageTask
#endregion Tasks

#region Event
    public delegate void AnimationUIEvent();
    AnimationUIEvent atEndEvents;
    List<AnimationUIEvent> atTimeEvents = new List<AnimationUIEvent>();
    List<float> atTimes = new List<float>();

    IEnumerator AtTimeEvent(AnimationUIEvent atTimeEvent, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        atTimeEvent();
    }
    public AnimationUI AddFunctionAt(float time, AnimationUIEvent func)
    {
        atTimes.Add(time);
        atTimeEvents.Add(func);
        return this;
    }
    
    public AnimationUI AddFunctionAtEnd(AnimationUIEvent func)
    {
        atEndEvents += func;
        return this;
    }
#endregion Event




#if UNITY_EDITOR
    void ForceRepaint()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            // Editor.up
        }
    }
    void OnDrawGizmos()
    {
        ForceRepaint();
    }
    void Update()
    {
        if(Application.isPlaying)return;
        ForceRepaint();

        if(IsPlayingInEditMode && CurrentTime < TotalDuration)
        {
            CurrentTime = Mathf.Clamp(Time.time - _startTime, 0, TotalDuration);
            UpdateSequence(CurrentTime);
        } 
        else
        {
            if(UpdateSequence != null && IsPlayingInEditMode)UpdateSequence(TotalDuration); //Make sure the latest frame is called
            IsPlayingInEditMode = false;
        }
    }
    public void UpdateBySlider()
    {
        if(Application.isPlaying)return;
        if(IsPlayingInEditMode)return;
        InitFunction();
        if(UpdateSequence != null)UpdateSequence(CurrentTime);
    }
    [HideInInspector] public float CurrentTime = 0; // Don't forget this variable might be in build
    [HideInInspector] public bool IsPlayingInEditMode = false;
    float _startTime = 0;
    public void PreviewAnimation()
    {
        InitFunction();
        if(UpdateSequence == null)
        {
            Debug.Log("No animation exist");
            return;
        }
        _startTime = Time.time;
        CurrentTime = 0;
        IsPlayingInEditMode = true;
        UpdateSequence(0);// Make sure the first frame is called
    }
    public void PreviewStart()
    {
        InitFunction();
        if(UpdateSequence == null)
        {
            Debug.Log("No animation exist");
            return;
        }
        CurrentTime = 0;
        IsPlayingInEditMode = false;
        UpdateSequence(0);
    }
    public void PreviewEnd()
    {
        CurrentTime = TotalDuration;
        InitFunction();
        if(UpdateSequence == null)
        {
            Debug.Log("No animation exist");
            return;
        }
        IsPlayingInEditMode = false;
        CurrentTime = TotalDuration;
        UpdateSequence(Mathf.Clamp01(TotalDuration));
    }
    void Reset() //For the default value. A hacky way because the inspector reset the value for Serialized class
    {
        AnimationSequence = new Sequence[1]
        {
            new Sequence()
        };
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    public delegate void Animation(float t);
    public Animation UpdateSequence;
#region timing
    public void InitTime()
    {
        TotalDuration = 0;
        foreach(Sequence sequence in AnimationSequence)
        {
            TotalDuration += (sequence.SequenceType == Sequence.Type.Wait) ? sequence.Duration : 0;
        }
        // for case when the duration of a non wait is bigger
        float currentTimeCheck = 0;
        float tempTotalDuration = TotalDuration;
        foreach(Sequence sequence in AnimationSequence)
        {
            currentTimeCheck += (sequence.SequenceType == Sequence.Type.Wait) ? sequence.Duration : 0;
            if(sequence.SequenceType == Sequence.Type.Animation)
            {
                if(TotalDuration < currentTimeCheck + sequence.Duration)
                {
                    TotalDuration = currentTimeCheck + sequence.Duration;
                }
            }
        }
        CurrentTime = Mathf.Clamp(CurrentTime, 0, TotalDuration);
    }
#endregion timing
    void InitFunction()//For preview
    {
        UpdateSequence = null;
        Singleton.LoadSingleton();
        foreach(Sequence sequence in AnimationSequence)
        {
            // sequence.IsDone = false;
            if(sequence.SequenceType == Sequence.Type.Animation)
            {
                if(sequence.TargetComp == null)
                {
                    Debug.Log("Please assign Target");
                    return;
                }
                sequence.Init();
                if(sequence.TargetType == Sequence.ObjectType.RectTransform)
                {

                    RectTransform rt = sequence.TargetComp.GetComponent<RectTransform>();
                    void RtAnchoredPosition(float t)
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.AnchoredPositionState = Sequence.State.During;
                            rt.anchoredPosition
                            = Vector3.LerpUnclamped(sequence.AnchoredPositionStart, sequence.AnchoredPositionEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.AnchoredPositionState == Sequence.State.During || 
                            sequence.AnchoredPositionState == Sequence.State.Before))
                        {
                            rt.anchoredPosition = sequence.AnchoredPositionEnd;
                            sequence.AnchoredPositionState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.AnchoredPositionState == Sequence.State.During ||
                            sequence.AnchoredPositionState == Sequence.State.After))
                        {
                            rt.anchoredPosition = sequence.AnchoredPositionStart;
                            sequence.AnchoredPositionState = Sequence.State.Before;
                        }
                    }
                    void RtLocalEulerAngles(float t)
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.LocalEulerAnglesState = Sequence.State.During;
                            rt.localEulerAngles
                            = Vector3.LerpUnclamped(sequence.LocalEulerAnglesStart, sequence.LocalEulerAnglesEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.LocalEulerAnglesState == Sequence.State.During || 
                            sequence.LocalEulerAnglesState == Sequence.State.Before))
                        {
                            rt.localEulerAngles = sequence.LocalEulerAnglesEnd;
                            sequence.LocalEulerAnglesState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.LocalEulerAnglesState == Sequence.State.During ||
                            sequence.LocalEulerAnglesState == Sequence.State.After))
                        {
                            rt.localEulerAngles = sequence.LocalEulerAnglesStart;
                            sequence.LocalEulerAnglesState = Sequence.State.Before;
                        }
                    }
                    void RtLocalScale(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.LocalScaleState = Sequence.State.During;
                            rt.localScale
                            = Vector3.LerpUnclamped(sequence.LocalScaleStart, sequence.LocalScaleEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.LocalScaleState == Sequence.State.During || 
                            sequence.LocalScaleState == Sequence.State.Before))
                        {
                            rt.localScale = sequence.LocalScaleEnd;
                            sequence.LocalScaleState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.LocalScaleState == Sequence.State.During ||
                            sequence.LocalScaleState == Sequence.State.After))
                        {
                            rt.localScale = sequence.LocalScaleStart;
                            sequence.LocalScaleState = Sequence.State.Before;
                        }
                    }
                    void RtSizeDelta(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.SizeDeltaState = Sequence.State.During;
                            rt.sizeDelta
                            = Vector3.LerpUnclamped(sequence.SizeDeltaStart, sequence.SizeDeltaEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.SizeDeltaState == Sequence.State.During || 
                            sequence.SizeDeltaState == Sequence.State.Before))
                        {
                            rt.sizeDelta = sequence.SizeDeltaEnd;
                            sequence.SizeDeltaState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.SizeDeltaState == Sequence.State.During ||
                            sequence.SizeDeltaState == Sequence.State.After))
                        {
                            rt.sizeDelta = sequence.SizeDeltaStart;
                            sequence.SizeDeltaState = Sequence.State.Before;
                        }
                    }    
                    void RtAnchorMin(float t)
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.AnchorMinState = Sequence.State.During;
                            rt.anchorMin
                            = Vector3.LerpUnclamped(sequence.AnchorMinStart, sequence.AnchorMinEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.AnchorMinState == Sequence.State.During || 
                            sequence.AnchorMinState == Sequence.State.Before))
                        {
                            rt.anchorMin = sequence.AnchorMinEnd;
                            sequence.AnchorMinState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.AnchorMinState == Sequence.State.During ||
                            sequence.AnchorMinState == Sequence.State.After))
                        {
                            rt.anchorMin = sequence.AnchorMinStart;
                            sequence.AnchorMinState = Sequence.State.Before;
                        }
                    }
                    void RtAnchorMax(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.AnchorMaxState = Sequence.State.During;
                            rt.anchorMax
                            = Vector3.LerpUnclamped(sequence.AnchorMaxStart, sequence.AnchorMaxEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.AnchorMaxState == Sequence.State.During || 
                            sequence.AnchorMaxState == Sequence.State.Before))
                        {
                            rt.anchorMax = sequence.AnchorMaxEnd;
                            sequence.AnchorMaxState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.AnchorMaxState == Sequence.State.During ||
                            sequence.AnchorMaxState == Sequence.State.After))
                        {
                            rt.anchorMax = sequence.AnchorMaxStart;
                            sequence.AnchorMaxState = Sequence.State.Before;
                        }
                    }
                    void RtPivot(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.PivotState = Sequence.State.During;
                            rt.pivot
                            = Vector3.LerpUnclamped(sequence.PivotStart, sequence.PivotEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.PivotState == Sequence.State.During || 
                            sequence.PivotState == Sequence.State.Before))
                        {
                            rt.pivot = sequence.PivotEnd;
                            sequence.PivotState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.PivotState == Sequence.State.During ||
                            sequence.PivotState == Sequence.State.After))
                        {
                            rt.pivot = sequence.PivotStart;
                            sequence.PivotState = Sequence.State.Before;
                        }
                    }
                    
                    
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchoredPosition))
                        UpdateSequence += RtAnchoredPosition;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalEulerAngles))
                        UpdateSequence += RtLocalEulerAngles;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.LocalScale))
                        UpdateSequence += RtLocalScale;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.SizeDelta))
                        UpdateSequence += RtSizeDelta;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMax))
                        UpdateSequence += RtAnchorMax;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.AnchorMin))
                        UpdateSequence += RtAnchorMin;
                    if(sequence.TargetRtTask.HasFlag(Sequence.RtTask.Pivot))
                        UpdateSequence += RtPivot;
                    
                }
                else if(sequence.TargetType == Sequence.ObjectType.Transform)
                {
                    Transform trans = sequence.TargetComp.transform;
                    void TransLocalPosition(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.LocalPositionState = Sequence.State.During;
                            trans.localPosition
                            = Vector3.LerpUnclamped(sequence.LocalPositionStart, sequence.LocalPositionEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.LocalPositionState == Sequence.State.During || 
                            sequence.LocalPositionState == Sequence.State.Before))
                        {
                            trans.localPosition = sequence.LocalPositionEnd;
                            sequence.LocalPositionState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.LocalPositionState == Sequence.State.During ||
                            sequence.LocalPositionState == Sequence.State.After))
                        {
                            trans.localPosition = sequence.LocalPositionStart;
                            sequence.LocalPositionState = Sequence.State.Before;
                        }
                    }
                    void TransLocalEulerAngles(float t)
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.LocalEulerAnglesState = Sequence.State.During;
                            trans.localEulerAngles
                            = Vector3.LerpUnclamped(sequence.LocalEulerAnglesStart, sequence.LocalEulerAnglesEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.LocalEulerAnglesState == Sequence.State.During || 
                            sequence.LocalEulerAnglesState == Sequence.State.Before))
                        {
                            trans.localEulerAngles = sequence.LocalEulerAnglesEnd;
                            sequence.LocalEulerAnglesState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.LocalEulerAnglesState == Sequence.State.During ||
                            sequence.LocalEulerAnglesState == Sequence.State.After))
                        {
                            trans.localEulerAngles = sequence.LocalEulerAnglesStart;
                            sequence.LocalEulerAnglesState = Sequence.State.Before;
                        }
                    }
                    void TransLocalScale(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.LocalScaleState = Sequence.State.During;
                            trans.localScale
                            = Vector3.LerpUnclamped(sequence.LocalScaleStart, sequence.LocalScaleEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.LocalScaleState == Sequence.State.During || 
                            sequence.LocalScaleState == Sequence.State.Before))
                        {
                            trans.localScale = sequence.LocalScaleEnd;
                            sequence.LocalScaleState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.LocalScaleState == Sequence.State.During ||
                            sequence.LocalScaleState == Sequence.State.After))
                        {
                            trans.localScale = sequence.LocalScaleStart;
                            sequence.LocalScaleState = Sequence.State.Before;
                        }
                    }
                    
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalPosition))
                        UpdateSequence += TransLocalPosition;
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalEulerAngles))
                        UpdateSequence += TransLocalEulerAngles;
                    if(sequence.TargetTransTask.HasFlag(Sequence.TransTask.LocalScale))
                        UpdateSequence += TransLocalScale;
                }
                else if(sequence.TargetType == Sequence.ObjectType.Image)
                {
                    Image img = sequence.TargetComp.GetComponent<Image>();
                    void ImgColor(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.ColorState = Sequence.State.During;
                            img.color
                            = Color.LerpUnclamped(sequence.ColorStart, sequence.ColorEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.ColorState == Sequence.State.During || 
                            sequence.ColorState == Sequence.State.Before))
                        {
                            img.color = sequence.ColorEnd;
                            sequence.ColorState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.ColorState == Sequence.State.During ||
                            sequence.ColorState == Sequence.State.After))
                        {
                            img.color = sequence.ColorStart;
                            sequence.ColorState = Sequence.State.Before;
                        }
                    }
                    void ImgFillAmount(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.ColorState = Sequence.State.During;
                            img.fillAmount
                            = Mathf.LerpUnclamped(sequence.FillAmountStart, sequence.FillAmountEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.ColorState == Sequence.State.During || 
                            sequence.ColorState == Sequence.State.Before))
                        {
                            img.fillAmount = sequence.FillAmountEnd;
                            sequence.ColorState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.ColorState == Sequence.State.During ||
                            sequence.ColorState == Sequence.State.After))
                        {
                            img.fillAmount = sequence.FillAmountStart;
                            sequence.ColorState = Sequence.State.Before;
                        }
                    }
                    
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.Color))
                        UpdateSequence += ImgColor;
                    if(sequence.TargetImgTask.HasFlag(Sequence.ImgTask.FillAmount))
                        UpdateSequence += ImgFillAmount;
                }
                else if(sequence.TargetType == Sequence.ObjectType.CanvasGroup)
                {
                    CanvasGroup cg = sequence.TargetComp.GetComponent<CanvasGroup>();
                    void CgAlpha(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.AlphaState = Sequence.State.During;
                            cg.alpha
                            = Mathf.LerpUnclamped(sequence.AlphaStart, sequence.AlphaEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.AlphaState == Sequence.State.During || 
                            sequence.AlphaState == Sequence.State.Before))
                        {
                            cg.alpha = sequence.AlphaEnd;
                            sequence.AlphaState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.AlphaState == Sequence.State.During ||
                            sequence.AlphaState == Sequence.State.After))
                        {
                            cg.alpha = sequence.AlphaStart;
                            sequence.AlphaState = Sequence.State.Before;
                        }
                    }
                    
                    if(sequence.TargetCgTask.HasFlag(Sequence.CgTask.Alpha))
                        UpdateSequence += CgAlpha;
                }
                else if(sequence.TargetType == Sequence.ObjectType.Camera)
                {
                    Camera cam = sequence.TargetComp.GetComponent<Camera>();
                    void CamBackgroundColor(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.BackgroundColorState = Sequence.State.During;
                            cam.backgroundColor
                            = Color.LerpUnclamped(sequence.BackgroundColorStart, sequence.BackgroundColorEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.BackgroundColorState == Sequence.State.During || 
                            sequence.BackgroundColorState == Sequence.State.Before))
                        {
                            cam.backgroundColor = sequence.BackgroundColorEnd;
                            sequence.BackgroundColorState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.BackgroundColorState == Sequence.State.During ||
                            sequence.BackgroundColorState == Sequence.State.After))
                        {
                            cam.backgroundColor = sequence.BackgroundColorStart;
                            sequence.BackgroundColorState = Sequence.State.Before;
                        }
                    }
                    void CamOrthographicSize(float t) 
                    {
                        if((0 <= t-sequence.StartTime) && (t-sequence.StartTime < sequence.Duration))
                        {
                            sequence.OrthographicSizeState = Sequence.State.During;
                            cam.orthographicSize
                            = Mathf.LerpUnclamped(sequence.OrthographicSizeStart, sequence.OrthographicSizeEnd,
                                sequence.EaseFunction(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration)));
                        }
                        if((t-sequence.StartTime >= sequence.Duration) && 
                            (sequence.OrthographicSizeState == Sequence.State.During || 
                            sequence.OrthographicSizeState == Sequence.State.Before))
                        {
                            cam.orthographicSize = sequence.OrthographicSizeEnd;
                            sequence.OrthographicSizeState = Sequence.State.After;
                        }
                        else if((t-sequence.StartTime < 0) && 
                            (sequence.OrthographicSizeState == Sequence.State.During ||
                            sequence.OrthographicSizeState == Sequence.State.After))
                        {
                            cam.orthographicSize = sequence.OrthographicSizeStart;
                            sequence.OrthographicSizeState = Sequence.State.Before;
                        }
                    }
                    
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.BackgroundColor))
                        UpdateSequence += CamBackgroundColor;
                    if(sequence.TargetCamTask.HasFlag(Sequence.CamTask.OrthographicSize))
                        UpdateSequence += CamOrthographicSize;
                }
                else if(sequence.TargetType == Sequence.ObjectType.UnityEventDynamic)
                {
                    Image img = sequence.TargetComp.GetComponent<Image>();
                    void EventDynamic(float t) 
                    {
                        if(t-sequence.StartTime < 0)return;
                        sequence.EventDynamic?.Invoke(Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration));
                    }
                    UpdateSequence += EventDynamic;
                }
                
            }
            else if(sequence.SequenceType == Sequence.Type.Wait)
            {

            }
            else if(sequence.SequenceType == Sequence.Type.SetActiveAllInput)
            {
                void SetActiveALlInput(float t)
                {
                    float time = Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration);
                    if(!sequence.IsDone) // so that SetActiveAllInput in the first frame can also be called
                    {
                        if(t - sequence.StartTime > -0.01f)
                        {
                            sequence.IsDone = true;
                            Singleton.Instance.Game.SetActiveAllInput(sequence.IsActivating);
                        }
                    }
                    else if(t - sequence.StartTime < 0)
                    {
                        sequence.IsDone = false;
                        Singleton.Instance.Game.SetActiveAllInput(!sequence.IsActivating);
                    }
                }
                // sequence.IsDone = false;
                UpdateSequence += SetActiveALlInput;
            }
            else if(sequence.SequenceType == Sequence.Type.SetActive)
            {
                void SetActive(float t)
                {
                    float time = Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration);
                    if(sequence.Target == null)
                    {
                        Debug.Log("Please assign Target for Sequence at "+sequence.StartTime.ToString()+"s");
                        return;
                    }

                    if(!sequence.IsDone)
                    {
                        if(t-sequence.StartTime >= sequence.Duration)
                        {
                            sequence.IsDone = true;
                            sequence.Target.SetActive(sequence.IsActivating);
                        }
                    }
                    else if(t-sequence.StartTime < 0) // IsDone == true && t-sequence.StartTime < 0 
                    {
                        sequence.IsDone = false;
                        sequence.Target.SetActive(!sequence.IsActivating);
                    }
                }
                UpdateSequence += SetActive;
            }
            else if(sequence.SequenceType == Sequence.Type.SFX)
            {
                void SFX(float t)
                {
                    float time = Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration);
                    if(!sequence.IsDone) // so that SetActiveAllInput in the first frame can also be called
                    {
                        if(t - sequence.StartTime > -0.01f)
                        {
                            sequence.IsDone = true;
                            if(sequence.SFX == null)
                            {
                                Debug.Log("Please assign SFX for Sequence at "+sequence.StartTime.ToString()+"s");
                                return;
                            }
                            Singleton.Instance.Audio.PlaySound(sequence.SFX);
                        }
                    }
                    else if(t - sequence.StartTime < 0)
                    {
                        if(sequence.SFX == null)
                        {
                            Debug.Log("Please assign  for Sequence at "+sequence.StartTime.ToString()+"s");
                            return;
                        }
                        sequence.IsDone = false;
                        Singleton.Instance.Audio.PlaySound(sequence.SFX);
                    }
                }
                sequence.IsDone = false;
                UpdateSequence += SFX;
            }
            else if(sequence.SequenceType == Sequence.Type.UnityEvent)
            {
                void UnityEvent(float t)
                {
                    float time = Mathf.Clamp01((t-sequence.StartTime)/sequence.Duration);
                    if(!sequence.IsDone) 
                    {
                        // -0.01f so that SetActiveAllInput in the first frame can also be called
                        if(t - sequence.StartTime > -0.01f) //Nested conditional may actually more performant in this case
                        {
                            sequence.IsDone = true;
                            sequence.Event?.Invoke();
                        } 
                    }
                    else if(t - sequence.StartTime < 0)
                    {
                        sequence.IsDone = false;
                        sequence.Event?.Invoke();
                    } 
                }
                sequence.IsDone = false;
                UpdateSequence += UnityEvent;
            }
        }
    }
#endif
}
