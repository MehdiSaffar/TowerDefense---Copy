using UnityEngine;
using System.Collections;

public class ParticleSystemExtra : MonoBehaviour {
    public bool destroyOnEnd;
    public bool deactivateGameObjectOnEnd;

    protected ParticleSystem ps;
    protected float elapsedTime = 0f;
    void Start() {
        ps = GetComponent<ParticleSystem>();
        if(ps)
        {
            if(destroyOnEnd)
                Destroy(gameObject, ps.duration);
        }
    }
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= ps.duration)
        {
            elapsedTime = 0f;
            gameObject.SetActive(!deactivateGameObjectOnEnd);
        }
    }
}
