using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCineCam: Singleton<MainCineCam>
{
    [HideInInspector] public CinemachineVirtualCamera vCam { get; private set; }
    [HideInInspector] public Transform target { get; private set; }
    public List<Transform> targetTrList = new List<Transform>();
    public StateMachine<MainCineCam> stateMachine { get; private set; }
    [Min(10)] public float minOrthoSize;

    private Volume volume;
    private Coroutine vignetteCor;
    private Vignette vig;


    void Start()
    {
        volume = Camera.main.GetComponent<Volume>();
        vig = GetVolumeComponent<Vignette>();

        vCam = gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        target = transform.Find("Target");
        stateMachine = new StateMachine<MainCineCam>(this, new CSingleLookAt());
    }

    void Update()
    {
        stateMachine.Update();
        if(targetTrList.Count <= 1)
        {
            stateMachine.ChangeState(new CSingleLookAt());
        }
        else
        {
            stateMachine.ChangeState(new CMultipleLookAt());
        }
    }

    public void LookAt(Vector2 pos)
    {
        target.position = pos;
    }

    public void SetOrthoSize(float size)
    {
        vCam.m_Lens.OrthographicSize = size;
    }


    public void FadeVignette(Color color, bool fadeIn, float time, float minVal = 0.2f, float maxVal = 0.4f)
    {
        if (vignetteCor != null)
            StopCoroutine(vignetteCor);
        StartCoroutine(VignetteCoroutine(color, fadeIn, time, minVal, maxVal));
    }

    public void SetShadowTone(Color col, float val, float endVal)
    {
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out var colorGrading))
        {

            colorGrading.shadows.overrideState = true;
            colorGrading.shadowsEnd = new MinFloatParameter(endVal, 0);
            Vector4 color = new Vector4(col.r, col.g, col.b, 0) * Mathf.Lerp(0f, endVal, val);
            colorGrading.shadows.value = color;
        }
    }

    public T GetVolumeComponent<T>() where T : VolumeComponent
    {
        foreach (var component in volume.profile.components)
        {
            if (component is T)
            {
                return component as T;
            }
        }
        return null;
    }

    private IEnumerator VignetteCoroutine(Color color, bool fadeIn, float time, float minVal, float maxVal)
    {
        vig.color = new ColorParameter(color);
        if (fadeIn)
        {
            for (float eT = 0; eT <= time; eT += Time.deltaTime)
            {
                float intensity = Mathf.Lerp(minVal, maxVal, eT / time);
                vig.intensity = new ClampedFloatParameter(intensity, 0, 1);
                yield return null;
            }
            vig.intensity = new ClampedFloatParameter(maxVal, 0, 1);
        }
        else
        {
            for (float eT = 0; eT <= time; eT += Time.deltaTime)
            {
                float intensity = Mathf.Lerp(maxVal, minVal, eT / time);
                vig.intensity = new ClampedFloatParameter(intensity, 0, 1);
                yield return null;
            }
            vig.intensity = new ClampedFloatParameter(minVal, 0, 1);
        }
    }
}
