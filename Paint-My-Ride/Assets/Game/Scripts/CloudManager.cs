using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public enum CloudGroup
    {
        Left,
        Right
    }

    [System.Serializable]
    public class CloudData
    {
        public SpriteRenderer cloud;
        public CloudGroup group;

        [Header("Movement")]
        public Vector3 targetPos;
        public float speed = 2f;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public bool isMoving;
        [HideInInspector] public bool finished;
    }

    public List<CloudData> clouds = new List<CloudData>();

    [SerializeField] private CloudGroup startGroup = CloudGroup.Left;

    private CloudGroup currentGroup;

    private void Start()
    {
        currentGroup = startGroup;

        foreach (var cloud in clouds)
        {
            if (cloud.cloud == null) continue;

            // Store initial position
            cloud.startPos = cloud.cloud.transform.position;

            cloud.isMoving = (cloud.group == currentGroup);
            cloud.finished = false;
        }
    }

    private void Update()
    {
        bool allFinished = true;

        foreach (var cloud in clouds)
        {
            if (!cloud.isMoving || cloud.cloud == null) continue;

            cloud.cloud.transform.position = Vector3.MoveTowards(
                cloud.cloud.transform.position,
                cloud.targetPos,
                cloud.speed * Time.deltaTime
            );

            if (Vector3.Distance(cloud.cloud.transform.position, cloud.targetPos) < 0.05f)
            {
                cloud.finished = true;
            }
            else
            {
                allFinished = false;
            }
        }

        if (allFinished)
        {
            SwitchGroup();
        }
    }

    private void SwitchGroup()
    {
        foreach (var cloud in clouds)
        {
            if (cloud.group == currentGroup && cloud.cloud != null)
            {
                cloud.cloud.transform.position = cloud.startPos;
            }
        }

        currentGroup = (currentGroup == CloudGroup.Left)
            ? CloudGroup.Right
            : CloudGroup.Left;

        foreach (var cloud in clouds)
        {
            if (cloud.cloud == null) continue;

            cloud.isMoving = (cloud.group == currentGroup);
            cloud.finished = false;
        }
    }

    internal void UpdateView(bool isVisible)
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<SpriteRenderer>().enabled = isVisible;
        }
    }
}