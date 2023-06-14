using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallType ballType;

    public enum BallType
    {
        red,
        yellow
    }

    public Color paintColor;
    public bool isMoving;

    public Animator animator;
    private Collider collider;
    private TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void Init(Vector3 _position)
    {
        StartCoroutine(C_Init(_position));
    }

    private IEnumerator C_Init(Vector3 _position)
    {
        collider = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        trail.enabled = false;

        yield return null;

        transform.position = _position;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        trail.enabled = true;



   


        Paint();
        DisableTrigger();

    }

    public void Move(Vector3 direction)
    {
        if (gameObject.activeSelf == false) return;

        StartCoroutine(C_Move(direction));
    }

    private IEnumerator C_Move(Vector3 direction)
    {
        isMoving = true;

        bool isLoop = true;
        bool isExplosion = false;
        bool isMoveAnim = false;

        while (isLoop)
        {
            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, 20.0f);

            var multiHitInfo = hits;
            System.Array.Sort(multiHitInfo, (x, y) => x.distance.CompareTo(y.distance));
            int n = 0;

            foreach (var hit in multiHitInfo)
            {
                if (hit.collider.CompareTag("Brick"))
                {
                    n++;
                }
                else if (hit.collider.CompareTag("Tile"))
                {
                    break;
                }
            }

            if(n == 0)
            {
                isLoop = false;
            }
            else
            {
                if(isMoveAnim == false)
                {
                    transform.rotation = Quaternion.LookRotation(direction);

                    isMoveAnim = true;
                    animator.SetTrigger("Move");
                }

                isExplosion = true;

                float t = 0.0f;

                Vector3 fromPosition = transform.position;
                Vector3 toPosition = fromPosition + direction;

                bool isPaint = false;

                while (t < 1)
                {
                    t += Time.deltaTime * 30.0f;
                    transform.position = Vector3.Lerp(fromPosition, toPosition, t);

                    if(t > 0.75f && isPaint == false)
                    {
                        isPaint = true;

                        Paint();
                        EnableTrigger();
                        DisableTrigger();
                    }

                    yield return null;
                }

                transform.position = toPosition;
            }
        }

        if (isExplosion)
        {
            animator.SetTrigger("Idle");

            if (ballType == BallType.red)
            {
                GameManager.instance.RedExplosion(transform.position);
            }
            else
            {
                GameManager.instance.YellowExplosion(transform.position);
            }
        }

        yield return new WaitForSeconds(0.05f);

        isMoving = false;
    }

    private void DisableTrigger()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 10, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider != null)
            {
                collider = hit.collider.gameObject.GetComponent<Collider>();
                collider.enabled = false;
            }
        }
    }

    private void EnableTrigger()
    {
        if (collider != null)
            collider.enabled = true;
    }
    
    private void Paint()
    {
        GameManager.instance.Vibration();

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 10, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Brick"))
            {
                Brick brick = hit.collider.gameObject.GetComponent<Brick>();
                brick.PaintColor(paintColor);
            }
        }
    }
}
