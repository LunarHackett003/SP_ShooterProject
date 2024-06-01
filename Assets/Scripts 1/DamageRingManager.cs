using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageRingManager : MonoBehaviour
{
    public class DamageRing
    {
        public GameObject ringObject;
        public Image ringImage;
        public Vector3 sourcePosition;
        public float lifetime;
        public CanvasGroup group;
    }
    public GameObject ringPrefab;
    public Transform ringParent;
    public float ringLifetime;
    public List<DamageRing> ringList;
    public AnimationCurve lifetimeAlphaCurve;

    public static DamageRingManager Instance;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        ringList = new();
    }
    public void ClearRings()
    {
        ringList.ForEach(x =>
        {
            Destroy(x.ringObject);
        });
        ringList.Clear();
    }
    public void AddRing(Vector3 source)
    {
        var r = new DamageRing()
        {
            ringObject = Instantiate(ringPrefab, ringParent)
        };
        r.ringImage = r.ringObject.GetComponent<Image>();
        r.ringObject.transform.localPosition = Vector3.zero;
        r.sourcePosition = source;
        r.group = r.ringObject.GetComponent<CanvasGroup>();
        ringList.Add(r);
    }

    private void Update()
    {
        if(ringList.Count > 0)
        {
            for (int i = 0; i < ringList.Count; i++)
            {
                var r = ringList[i];
                Vector3 dir = r.sourcePosition - GameManager.instance.playerRef.transform.position;
                Quaternion t = Quaternion.LookRotation(dir);
                t.z = -t.y;
                t.x = 0;
                t.y = 0;
                r.group.alpha = lifetimeAlphaCurve.Evaluate(Mathf.InverseLerp(ringLifetime, 0, r.lifetime));
                Vector3 north = new(0, 0, GameManager.instance.playerRef.transform.eulerAngles.y);
                r.ringObject.transform.localRotation = t * Quaternion.Euler(north);

            }
        }
    }
    private void FixedUpdate()
    {
        if (ringList.Count > 0)
        {
            for (int i = 0; i < ringList.Count; i++)
            {
                var r = ringList[i];
                r.lifetime += Time.fixedDeltaTime;
            }
        }
        ringList.ForEach(x =>
        {
            if(x.lifetime > ringLifetime)
            {
                Destroy(x.ringObject);
            }
        });
        ringList.RemoveAll(x => x.lifetime > ringLifetime);
    }

    float RotationFromVector(Vector3 start, Vector3 end)
    {
        float r = Mathf.Atan2(start.y - end.y, start.x - end.x);
        float d = r * Mathf.Rad2Deg;
        return d;
    }
}
