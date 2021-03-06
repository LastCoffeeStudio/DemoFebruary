﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    public AudioSource shotAudio;
    public AudioSource reloadAudio;
    public AudioSource lastReloadAudio;
    public float amount;
    public float maxAmount;
    public float smoothAmount;
    public Transform shootPoint;
    public Transform camera;
    public uint maxAmmo = 10;
    public uint ammunition;
    public Text text;
    public Text numDronsText;

    private bool firing = false;
    private bool reloading = false;
    private bool aimming = false;
    private Animator animator;
    private Vector3 initialposition;

    public Vector3 aimPosition;
    public Vector3 originalPosition;
    public float aimSpeed;
    public int shotgunBullets;
    public float bulletDispersion;
    public float weaponRange;
    public GameObject esferaVerde;
    public GameObject esferaRoja;
    public GameObject explosion;

    public int numDorms = 5;

    void Start () {
        ammunition = maxAmmo;
        initialposition = transform.localPosition;
        animator = gameObject.GetComponent<Animator>();
    }

    private bool playLastReload = false;
	void Update () {
        
        if (GameManager.debugMode)
        {
            Vector3 dir = transform.forward + (Vector3.Cross(camera.forward,Random.insideUnitSphere) * Random.Range(0f, bulletDispersion));
            Debug.DrawRay(camera.position, dir*2, new Color(0f, 255f, 0f));
        }

        text.text = ammunition.ToString();

        if (!firing && !reloading && !aimming)
        {
            swagWeaponMovement();
        }
        
	    if (!playLastReload && reloading && ammunition == maxAmmo)
	    {
	        playLastReload = true;

            lastReloadAudio.Play();
	    }
        checkInputAnimations();
        aimAmmo();
    }

     void aimAmmo()
    {
        if (!reloading)
        {
            if (Input.GetButton("Fire2"))
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aimSpeed);
                aimming = true;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimSpeed);
                aimming = false;
            }
        }
    }

    private void swagWeaponMovement()
    {
        reloadAudio.Stop();
        float movX = -Input.GetAxis("Mouse X") * amount;
        float movY = -Input.GetAxis("Mouse Y") * amount;
        movX = Mathf.Clamp(movX, -maxAmount, maxAmount);
        movY = Mathf.Clamp(movY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(movX, movY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialposition, Time.deltaTime * smoothAmount);
    }

    private void checkInputAnimations()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (ammunition > 0)
            {
                animator.SetBool("shooting",true);
                if (animator.GetBool("reloading"))
                {
                    reloading = false;
                    animator.SetBool("reloading",false);
                }
            }
        }
        if (Input.GetKey(KeyCode.R) && ammunition < maxAmmo)
        {
            reloading = true;
            animator.SetBool("reloading",true);
            playLastReload = false;
            reloadAudio.Play();
            if (ammunition == maxAmmo - 1)
            {
                reloadAudio.Stop();
                lastReloadAudio.Play();
                animator.SetBool("lastBullet",true);
            }
        }
    }

    void checkMouseInput()
    {
        if (!Input.GetKey(KeyCode.Mouse0) || ammunition == 0)
        {
            animator.SetBool("shooting", false);
        }
    }

    void decreaseAmmo()
    {
        --ammunition;
        bool destroyed = false;
        shotAudio.Play();
        //switch with different weapons
        for (uint i = 0; i < shotgunBullets; ++i)
        {
            Vector3 dir = transform.forward + (Vector3.Cross(camera.forward, Random.insideUnitSphere) * Random.Range(0f, bulletDispersion));
            RaycastHit hitInfo;


            if (Physics.Raycast(camera.position, dir, out hitInfo, weaponRange))
            {
                if (hitInfo.transform.tag == "Agent")
                {
                    destroyed = true;
                    //numDronsText.text = numDorms.ToString();
                    Destroy(hitInfo.collider.gameObject);
                    GameObject.Instantiate(explosion, hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
                }
                else
                {
                    GameObject.Instantiate(esferaVerde, hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
                }
            }
            
        }
        if (destroyed)
        {
            --numDorms;
            numDronsText.text = numDorms.ToString();
        }
    }
       

    void increaseAmmo()
    {
        ++ammunition;
    }

    void endReload()
    {
        reloading = false;
        animator.SetBool("lastBullet", false);
        animator.SetBool("reloading",false);
    }

    void reloadAndCheck()
    {
        if (ammunition == maxAmmo - 1)
        {
            animator.SetBool("lastBullet", true);
        }
    }
    
}
