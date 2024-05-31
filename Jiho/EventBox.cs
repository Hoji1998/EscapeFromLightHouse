using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;
public class EventBox : MonoBehaviour
{
    [Header("Event Component")]
    [SerializeField] private EventTrigger thisEvent;
    [SerializeField] private Vector3 lockRotation = Vector3.zero;
    [SerializeField] private bool IsLockRotation = false;
    [SerializeField] public List<InteractEvent> interactEvents;

    [Header("ElevatorEvent Component")]
    [SerializeField] private BreakablePlatform breakablePlatform;
    private bool onEnter = false;
    public enum EventTrigger { SubTitle, Gass, ElevatorFall, None }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (interactEvents.Count > 0)
        {
            foreach (InteractEvent eventObject in interactEvents)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, eventObject.transform.position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(eventObject.transform.position, 0.3f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();

        if (characterController == null || onEnter)
            return;

        onEnter = true;

        switch (thisEvent)
        {
            case EventTrigger.SubTitle:
                LevelManager.Instance.StartSubtitlesEvent();
                break;
            case EventTrigger.Gass:
                Camera.main.GetComponentInChildren<PostProcessingControl>().GassOn();
                break;
            case EventTrigger.ElevatorFall:
                breakablePlatform.ShakeBox.transform.parent = breakablePlatform.transform;
                LevelManager.Instance.eventBox = this;
                LevelManager.Instance.StartSubtitlesEvent();
                break;
            case EventTrigger.None:
                break;
        }

        GameManager.Instance.PlayerMoving.LockRotation = lockRotation;
        GameManager.Instance.PlayerMoving.rotateLockEvent = IsLockRotation;

        foreach (InteractEvent interactEvent in interactEvents)
        {
            interactEvent.StartInteractEvent();
        }
    }

    public void ElevatorFallSequanceStart()
    {
        breakablePlatform.transform.parent = null;
        GameManager.Instance.PlayerMoving.LockRotation = new Vector3(90f, 0f, 0f);
        GameManager.Instance.PlayerMoving.rotateLockEvent = true;
        breakablePlatform.transform.localScale *= 0.9f;
        breakablePlatform.coll.isTrigger = true;
        breakablePlatform.StartInteractEvent();

        StartCoroutine(WaitForBreakPlatform());
    }
    private IEnumerator WaitForBreakPlatform()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.StageClear();
    }
}
