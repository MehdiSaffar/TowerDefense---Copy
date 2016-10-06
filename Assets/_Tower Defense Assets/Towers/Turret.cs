using UnityEngine;
using System.Collections;

public class Turret : Tower
{
    void FixedUpdate()
    {
        if (!currentTarget || !InRange(currentTarget.gameObject))
        {
            currentTarget = GetEnemy();
        }
        // If we have no target, no need to keep going
        if (!currentTarget) return;

        // We calculate the target rotation
        Vector3 predictionDir = (currentTarget.transform.position + currentTarget.transform.forward * currentTarget.speed * Time.fixedDeltaTime) - head.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(predictionDir, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation,
                                                           targetRotation,
                                                           turnSpeed * (1 / Mathf.Clamp01(predictionDir.sqrMagnitude)) * Time.fixedDeltaTime);
       
        // We make sure that the bullets will be fired straight out
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
        if (target != null)
        {
            if (projectile != null)
            {
                for (int i = 0; i < muzzleEnd.Length; i++)
                {
                    Bullet _bullet = (Instantiate(projectile, muzzleEnd[i].transform.position, muzzleEnd[i].transform.rotation) as GameObject).GetComponent<Bullet>();
                    _bullet.HitDamage = hitDamage;
                    _bullet.enemy = target;
                    if(muzzleFire) _muzzleFire[i].Play();
                }
                SoundManager.RandomizeFx(onFire);
            }
        }
    }
}
