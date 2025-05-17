using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class MainCineCam: Singleton<MainCineCam>
{
    public CinemachineVirtualCamera vCam;
    public Transform target;
    public List<Transform> targetTrList = new List<Transform>();
    public StateMachine<MainCineCam> stateMachine { get; private set; }
    public float minOrthoSize = 6;

    void Start()
    {
        vCam = gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        stateMachine = new StateMachine<MainCineCam>(this, new CSingleLookAt());
        SetOrthoSize(minOrthoSize);
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
