using UnityEngine;

public class Canon : Tower {
	void FixedUpdate()
    {
        if(!currentTarget || !InRange(currentTarget.gameObject))
        {
            currentTarget = GetEnemy();
        }
        // if we have no target, no need to keep going
        if (!currentTarget) return;

        // We calculate the target rotation
        Vector3 predictionDir = (currentTarget.transform.position + currentTarget.transform.forward * currentTarget.speed * Time.fixedDeltaTime) - head.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(predictionDir, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation,
                                                           targetRotation,
                                                           turnSpeed * (1 / Mathf.Clamp01(predictionDir.sqrMagnitude)) * Time.fixedDeltaTime);

        // We make sure that the bombs will be blasted straight out
        head.transform.localRotation = Quaternion.Euler(
            0,
            head.transform.localRotation.eulerAngles.y,
            head.transform.localRotation.eulerAngles.z
            );

        // We check if we can fire
        elapsedSinceFire += Time.fixedDeltaTime;
        if (elapsedSinceFire >= (60f / hitRate)
            && Mathf.Abs(targetRotation.eulerAngles.y - head.transform.rotation.eulerAngles.y) <= angleEpsilon)
        {
            elapsedSinceFire = 0f;
            Fire(currentTarget);
        }
    }
    protected void Fire(Enemy target)
    {
        if (target)
        {
            if (projectile)
            {
                for (int i = 0; i < muzzleEnd.Length; i++)
                {
                    Bomb bomb = (Instantiate(projectile, muzzleEnd[i].transform.position, head.transform.rotation) as GameObject).GetComponent<Bomb>();
                    bomb.HitDamage = hitDamage;
                    bomb.targetDistance = (target.transform.position - muzzleEnd[i].transform.position).magnitude;
                    if (muzzleFire)
                    {
                        _muzzleFire[i].Play();
                    }
                }
                SoundManager.RandomizeFx(onFire);
            }
        }
    }
}
