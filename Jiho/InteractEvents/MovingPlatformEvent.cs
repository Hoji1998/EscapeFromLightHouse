using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformEvent : InteractEvent
{
    [Header("Moving Point")]
    [SerializeField] private List<Vector3> Points;

    [Header("Moving Component")]
    public float speed = 1f;
    [SerializeField] private float arrivalDelayTime = 0f;
    [SerializeField] private float startDelayTime = 0f;
    [SerializeField] private bool IsReturnining = false;
    [SerializeField] private bool IsCancleMove = true;
    [SerializeField] private bool AutoStart = false;
    [SerializeField] private CycleType cycleType;
    [SerializeField] private MovementType movementType;

    [HideInInspector] public bool IsMove = false;
    [HideInInspector] public Vector3 currentMoveDirection;

    [Header("Moving Sound")]
    [SerializeField] private AudioSource[] audioSource;

    [Header("Guide Line")]
    public LineRenderer guideLine;

    private Coroutine coroutine;
    private int currentPoint = 0;
    private int loopCount = 0;
    private bool IsForward = true;
    private Vector3 basePosition = Vector3.zero;
    private enum MovementType { MoveTowards, Lerp , Slerp, SmoothDamp}
    private enum CycleType { Loop, BackAndForth, MoveOnePoint }
    private void Start()
    {
        Initialize();
    }
    private void Awake()
    {
        base.SetGuideLine(guideLine);
    }
    private void Initialize()
    {
        basePosition = transform.position;
        coroutine = StartCoroutine(Move(true));
        StopCoroutine(coroutine);
        IsMove = false;

        if (AutoStart)
        {
            StartInteractEvent();
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        if (basePosition == Vector3.zero) //에디터
        {
            if (Points.Count > 0)
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(Points[i] + position, new Vector3(0.3f, 0.3f, 0.3f));

                    Gizmos.color = Color.magenta;
                    if (i < Points.Count - 1)
                    {
                        Gizmos.DrawLine(Points[i] + position, Points[i + 1] + position);
                    }
                }
            }
        }
        else //인게임
        {
            if (Points.Count > 0)
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(Points[i] + basePosition, new Vector3(0.3f, 0.3f, 0.3f));

                    Gizmos.color = Color.magenta;
                    if (i < Points.Count - 1)
                    {
                        Gizmos.DrawLine(Points[i] + basePosition, Points[i + 1] + basePosition);
                    }
                }
            }
        }
        
    }
    private void CheckMoveDirection()
    {
        if (currentPoint == 0)
            IsForward = true;

        if (currentPoint == Points.Count - 1)
            IsForward = false;

        if (IsForward)
            currentPoint++;
        else
            currentPoint--;

        CheckCycleState();
    }
    private void CheckCycleState()
    {
        switch (cycleType)
        {
            case CycleType.BackAndForth:
                if ((Points.Count - 1) * 2 > loopCount)
                {
                    loopCount++;
                    coroutine = StartCoroutine(Move(true));
                }
                else
                {
                    if (IsForward)
                        currentPoint--;
                    else
                        currentPoint++;

                    loopCount = 0;
                    IsMove = false;
                }
                break;
            case CycleType.Loop:
                coroutine = StartCoroutine(Move(true));
                break;
            case CycleType.MoveOnePoint:
                coroutine = StartCoroutine(Move(false));
                break;
        }
    }

    public override void StartInteractEvent()
    {
        if (IsCancleMove)
        {
            StopCoroutine(coroutine);
            CheckMoveDirection();
        }

        if (IsMove)
            return;

        if (!IsCancleMove)
        {
            CheckMoveDirection();
        }
    }

    public override void StopInteractEvent()
    {
        if (cycleType == CycleType.Loop && IsCancleMove)
        {
            StopCoroutine(coroutine);
            MoveStop();

            if (IsForward)
                currentPoint--;
            else
                currentPoint++;

            return;
        }

        if (IsCancleMove && currentPoint != 0)
        {
            StopCoroutine(coroutine);

            if (audioSource[0] != null)
            {
                audioSource[0].Play();
                audioSource[1].Play();
            }

            if (IsForward)
                IsForward = false;
            else
                IsForward = true;

            CheckMoveDirection();
        }
    }

    private IEnumerator Move(bool loop)
    {
        currentMoveDirection = (Points[currentPoint] + basePosition) - transform.position;
        float currentDistance = 0f;
        float curTime = 0f;
        bool arriveOn = false;
        IsMove = true;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (startDelayTime >= curTime)
            {
                curTime += Time.deltaTime;
                continue;
            }

            break;
        }
        if (audioSource[0] != null)
        {
            audioSource[0].Play();
            audioSource[1].Play();
        }

        while (true)
        {
            yield return new WaitForFixedUpdate();
            currentMoveDirection = (Points[currentPoint] + basePosition) - transform.position;
            if (!arriveOn)
            {
                CheckMovementType();
                currentDistance = Vector3.Distance((Points[currentPoint] + basePosition), transform.position);
            }
            else
            {
                curTime += Time.deltaTime;
                if (arrivalDelayTime >= curTime)
                {
                    curTime += Time.deltaTime;
                    continue;
                }

                break;
            }

            if (currentDistance <= speed * Time.fixedDeltaTime)
            {
                transform.position = Points[currentPoint] + basePosition;
                curTime = 0f;
                arriveOn = true;
            }
        }

        if (loop)
            CheckMoveDirection();
        else
        {
            MoveStop();
        }
    }
    private void MoveStop()
    {
        if (audioSource[0] != null)
        {
            audioSource[1].Stop();
            audioSource[2].Play();
        }
        IsMove = false;
    }
    private Vector3 refVector = Vector3.zero;
    private void CheckMovementType()
    {
        switch (movementType)
        {
            case MovementType.MoveTowards:
                transform.position = Vector3.MoveTowards(transform.position, (Points[currentPoint] + basePosition), speed * Time.fixedDeltaTime);
                break;
            case MovementType.Lerp:
                transform.position = Vector3.Lerp(transform.position, (Points[currentPoint] + basePosition), speed * Time.fixedDeltaTime);
                break;
            case MovementType.Slerp:
                transform.position = Vector3.Slerp(transform.position, (Points[currentPoint] + basePosition), speed * Time.fixedDeltaTime);
                break;
            case MovementType.SmoothDamp:
                transform.position = Vector3.SmoothDamp(transform.position, (Points[currentPoint] + basePosition), ref refVector, Time.fixedDeltaTime * 20f - speed * Time.fixedDeltaTime);
                break;
        }
    }
}
