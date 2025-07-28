using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public int shape;
    bool collided;
    [SerializeField] Vector2 maxMinDelay;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collided)
        {
            collided = true;
            StartCoroutine(Delay(collision));
        }
    }
    IEnumerator Delay(Collider2D collision)
    {
        float delay = Random.Range(maxMinDelay.x, maxMinDelay.y);
        yield return new WaitForSeconds(delay);
        if(collision != null)
        {
            SwitchShape switcher = collision.transform.root.GetComponent<SwitchShape>();
            switcher.SwitchJellyShape(shape);
        }
    }
}
