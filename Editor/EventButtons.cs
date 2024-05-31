using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovingPlatformEvent))]
public class MovingPlatformButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MovingPlatformEvent button = (MovingPlatformEvent)target;
        if (GUILayout.Button("Moving Start"))
        {
            button.StartInteractEvent();
        }
    }
}

[CustomEditor(typeof(GravityAreaGenerateEvent))]
public class GravityAreaGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GravityAreaGenerateEvent button = (GravityAreaGenerateEvent)target;

        if (GUILayout.Button("Generate GravityAreaObject"))
        {
            InitList(button);
            AddList(button);
        }

        if (GUILayout.Button("Delete GravityAreaObject"))
        {
            InitList(button);
            DeleteList(button);
        }
    }
    private void InitList(GravityAreaGenerateEvent button)
    {
        GravityAreaEvent[] instanceObjects = button.GetComponentsInChildren<GravityAreaEvent>();
        if (instanceObjects != null)
        {
            button.gravityAreasList.Clear();
            for (int i = 0; i < instanceObjects.Length; i++)
            {
                button.gravityAreasList.Add(instanceObjects[i].gameObject);
            }
        }
    }
    private void DeleteList(GravityAreaGenerateEvent button)
    {
        if (button.gravityAreasList.Count - 1 < 0)
            return;
        GameObject instanceObject = button.gravityAreasList[button.gravityAreasList.Count - 1];
        button.gravityAreasList.RemoveAt(button.gravityAreasList.Count - 1);
        DestroyImmediate(instanceObject);
    }
    private void AddList(GravityAreaGenerateEvent button)
    {
        GameObject instanceObject = Instantiate(Resources.Load("Prefabs/GravityArea") as GameObject, button.transform.position, Quaternion.identity);
        instanceObject.transform.parent = button.transform;
        button.gravityAreasList.Add(instanceObject);
    }
}