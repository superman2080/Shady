using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class MainCineCam: Singleton<MainCineCam>
{
    [HideInInspector] public CinemachineVirtualCamera vCam { get; private set; }
    [HideInInspector] public Transform target { get; private set; }
    public List<Transform> targetTrList = new List<Transform>();
    public StateMachine<MainCineCam> stateMachine { get; private set; }
    [Min(10)] public float minOrthoSize;

    void Start()
    {
        vCam = gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        target = transform.Find("Target");
        stateMachine = new StateMachine<MainCineCam>(this, new CSingleLookAt());
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void LookAt(Vector2 pos)
    {
        target.position = pos;
    }

    public void SetOrthoSize(float size)
    {
        vCam.m_Lens.OrthographicSize = size;
    }
}
