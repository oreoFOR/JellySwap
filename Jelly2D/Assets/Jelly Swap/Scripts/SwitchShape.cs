using UnityEngine;
using UnityEngine.UI;

public class SwitchShape : MonoBehaviour
{
    [SerializeField]GameObject[] jellyShapes;
    [SerializeField]GameObject currentShape;
    [SerializeField] GameObject splatPs;
    int currentShapeType;
    [SerializeField]Transform camFollow;
    [SerializeField] CameraFollow followCam;

    [SerializeField] Slider progressBar;
    public float finish;
    float dist;

    Vector2 avgVel;
    public enum JellyType
    {
        Player,
        Enemy
    }
    [SerializeField] JellyType jellyType = JellyType.Player;
    private void Start()
    {
        dist = camFollow.position.y - finish;
        progressBar.maxValue = dist -2.5f;
    }

    public void SwitchJellyShape(int shape)
    {
        if(shape != currentShapeType)
        {
            Instantiate(splatPs, camFollow.position, camFollow.rotation);
            currentShapeType = shape;
            Vector3 spawnPos = camFollow.position;
            Destroy(currentShape);
            avgVel = GetAverageVelocity(currentShape.GetComponent<Shape>().rbs);
            currentShape = Instantiate(jellyShapes[shape],spawnPos, Quaternion.identity,transform);
            SetAverageVelocity(currentShape.GetComponent<Shape>().rbs);
            camFollow = currentShape.GetComponent<Shape>().spawnPoint;
            if(jellyType == JellyType.Player)
            followCam.target = camFollow;
        }
    }
    private void Update()
    {
        SetProgress(-camFollow.position.y);
    }
    public void SetProgress(float progress)
    {
        progressBar.value = progress;
    }

    Vector2 GetAverageVelocity(Rigidbody2D[] rbs)
    {
        Vector2 total = Vector2.zero;
        for (int i = 0; i < rbs.Length; i++)
        {
            total += rbs[i].velocity;
        }
        Vector2 avg = total / rbs.Length;
        return avg;
    }
    void SetAverageVelocity(Rigidbody2D[] rbs)
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].velocity = avgVel;
        }
    }
}
