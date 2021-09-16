using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{

    public float reloadTime;
    public int clipSize;
    public int currentClipSize;
    public float timeBetweenShots;
    public float Damage;
    public GameObject Bullet;
    public Text ammoText;
    public enum GunType { Pistol, SMG, AR, LMG }; 
    public GunType gunType;
    public float bulletSpeed;

    private int maxClipSize;

    public SpriteRenderer spriteRenderer;

    public GameObject Player;

    public Sprite gunForward;

    public Image GunImage;
    public Sprite pistol;
    public Sprite SMG;
    public Sprite LMG;
    public Sprite AR;

    public int currentPistolAmmo;
    private int currentSMGAmmo;
    private int currentLMGAmmo;
    private int currentARAmmo;

    public bool  canShoot;

    public AudioSource gunShot;


    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        currentClipSize = clipSize;
        canShoot = true;
        gunType = GunType.Pistol;
        bulletSpeed = 45f;
        switchGun();
        currentPistolAmmo = determinAmmo(currentPistolAmmo, 20);
        switchSprite();
        ammoText.text = ": " + currentClipSize;
    }

    private void Update()
    {
        switchGunTypes();
    }

    public void shoot(Vector2 slope, float angle)
    {
        if (canShoot == true)
        {
            gunShot.Play();
            GameObject currentBullet;
            currentBullet = Instantiate(Bullet, new Vector3(transform.position.x,transform.position.y,6), Quaternion.Euler(0,0, angle));
            currentBullet.GetComponent<Rigidbody2D>().velocity = slope;
            currentBullet.GetComponent<bulletBehaviour>().slope = slope;
            currentBullet.GetComponent<bulletBehaviour>().Damage = Damage;
            StartCoroutine(currentBullet.GetComponent<bulletBehaviour>().outOfReach());
            canShoot = false;
            StartCoroutine(waitToShoot());
        }
    }

    public IEnumerator waitToShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(timeBetweenShots);
        currentClipSize -= 1;
        switch (gunType)
        {
            case GunType.Pistol:
                currentPistolAmmo = currentClipSize;
                break;

            case GunType.SMG:
                currentSMGAmmo = currentClipSize;
                break;

            case GunType.AR:
                currentARAmmo = currentClipSize;
                break;

            case GunType.LMG:
                currentLMGAmmo = currentClipSize;
                break;

            default: // Not a gun
                print("ERROR");
                break;
        }
        ammoText.text = ": " + currentClipSize;
        if(currentClipSize > 0)
        {
            canShoot = true;
        }
        else if(currentClipSize <= 0)
        {
            switch (gunType)
            {
                case GunType.Pistol:
                    animator.SetInteger("GunReloaded", 1);
                    break;

                case GunType.SMG:
                    animator.SetInteger("GunReloaded", 2);
                    break;

                case GunType.AR:
                    animator.SetInteger("GunReloaded", 3);
                    break;

                case GunType.LMG:
                    animator.SetInteger("GunReloaded", 4);
                    break;
            }
            //print("reloading");
            yield return new WaitForSeconds(reloadTime);
            animator.SetInteger("GunReloaded", 0);
            canShoot = true;
            switch (gunType)
            {
                case GunType.Pistol:
                    currentPistolAmmo = determinAmmo(currentPistolAmmo, 20);
                    break;

                case GunType.SMG:
                    currentSMGAmmo = determinAmmo(currentSMGAmmo, 50);
                    break;

                case GunType.AR:
                    currentARAmmo = determinAmmo(currentARAmmo, 35);
                    break;

                case GunType.LMG:
                    currentLMGAmmo = determinAmmo(currentLMGAmmo, 100);
                    break;
            }
            ammoText.text = ": " + currentClipSize;
            canShoot = true;
        }
    }


    public IEnumerator reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        switch (gunType)
        {
            case GunType.Pistol:
                currentPistolAmmo = determinAmmoOnReload(currentPistolAmmo, 20);
                break;

            case GunType.SMG:
                currentSMGAmmo = determinAmmoOnReload(currentSMGAmmo, 50);
                break;

            case GunType.AR:
                currentARAmmo = determinAmmoOnReload(currentARAmmo, 35);
                break;

            case GunType.LMG:
                currentLMGAmmo = determinAmmoOnReload(currentLMGAmmo, 100);
                break;
        }
        ammoText.text = ": " + currentClipSize;
        canShoot = true;
        animator.SetInteger("GunReloaded", 0);
    }

    public void switchGunTypes()
    {
        if (canShoot == true)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                gunType++;
                switchGun();
                ammoText.text = ": " + currentClipSize;
            }

            else if (Input.mouseScrollDelta.y < 0)
            {
                gunType--;
                switchGun();
                ammoText.text = ": " + currentClipSize;
            }

            if (Input.GetKeyDown(KeyCode.R) && currentClipSize != maxClipSize)
            {
                switch (gunType)
                {
                    case GunType.Pistol:
                        animator.SetInteger("GunReloaded", 1);
                        break;

                    case GunType.SMG:
                        animator.SetInteger("GunReloaded", 2);
                        break;

                    case GunType.AR:
                        animator.SetInteger("GunReloaded", 3);
                        break;

                    case GunType.LMG:
                        animator.SetInteger("GunReloaded", 4);
                        break;
                }
                StartCoroutine(reload());
                ammoText.text = ": " + currentClipSize;
            }
        }
    }

    private IEnumerator waitToSwitch()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.2f);
        switchGun();
        canShoot = true;
    }


    public void switchGun()
    {
        switch (gunType)
        {
            case GunType.Pistol:
                currentPistolAmmo = determinAmmo(currentPistolAmmo, 20);
                Damage = 1;
                timeBetweenShots = 0.3f;
                reloadTime = 0.5f;
                GunImage.sprite = pistol;
                spriteRenderer.sprite = pistol;
                break;

            case GunType.SMG:
                currentSMGAmmo = determinAmmo(currentSMGAmmo, 50);
                Damage = 2;
                timeBetweenShots = 0.15f;
                reloadTime = 0.5f;
                ammoText.text = ": " + currentClipSize;
                GunImage.sprite = SMG;
                spriteRenderer.sprite = SMG;
                break;

            case GunType.AR:
                currentARAmmo = determinAmmo(currentARAmmo, 35);
                Damage = 3;
                timeBetweenShots = 0.2f;
                reloadTime = 2f;
                ammoText.text = ": " + currentClipSize;
                GunImage.sprite = AR;
                spriteRenderer.sprite = AR;
                break;

            case GunType.LMG:
                currentLMGAmmo = determinAmmo(currentLMGAmmo, 100);
                Damage = 2;
                timeBetweenShots = 0.05f;
                reloadTime = 5f;
                ammoText.text = ": " + currentClipSize;
                GunImage.sprite = LMG;
                spriteRenderer.sprite = LMG;
                break;

            default: // Not a gun
                gunType = GunType.Pistol;
                currentPistolAmmo = determinAmmo(currentPistolAmmo, 20);
                Damage = 1;
                timeBetweenShots = 0.3f;
                reloadTime = 0.5f;
                ammoText.text = ": " + currentClipSize;
                GunImage.sprite = pistol;
                spriteRenderer.sprite = pistol;
                break;
        }
    }

    public void switchSprite()
    {
        switch (gunType)
        {
            case GunType.Pistol:

                spriteRenderer.sprite = pistol;
                break;

            case GunType.SMG:

                spriteRenderer.sprite = SMG;
                break;

            case GunType.AR:

                spriteRenderer.sprite = AR;
                break;

            case GunType.LMG:

                spriteRenderer.sprite = LMG;
                break;
        }
    }


    private int determinAmmo(int currentAmmoTypeClipSize, int maxClipSize)
    {
        if(currentAmmoTypeClipSize == 0)
        {
            currentClipSize = maxClipSize;
            this.maxClipSize = maxClipSize;
            return currentClipSize;
        }
        else
        {
            currentClipSize = currentAmmoTypeClipSize;
            return currentAmmoTypeClipSize;
        }
    }

    private int determinAmmoOnReload(int currentAmmoTypeClipSize, int maxClipSize)
    {
        currentClipSize = maxClipSize;
        this.maxClipSize = maxClipSize;
        return maxClipSize;
    }

}
