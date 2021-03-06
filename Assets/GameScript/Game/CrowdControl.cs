﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crowd;
using System;

public class CrowdControl : MonoBehaviour
{

   public EnemyControler controler;
   public SpriteRenderer spriteRenderer;

   private AttackType type;
   IEnumerator crowd;

    public void SetCrowdControl(float time, Sprite sprite, CrowdControlEffect effect, FollowUpEffect followUpEffect = null)//나중에 Effect라는 란을 따로 만들자
    {

        if(crowd != null)
        {
            StopCoroutine(crowd);
            crowd = null;
          //  Debug.Log("상태이상을 갱신합니다.");
        }


        this.spriteRenderer.sprite = sprite;
        crowd = Control(time, effect, followUpEffect);

        if (isActiveAndEnabled)
        {
            StartCoroutine(crowd);
        }
         
  
       
        

    }

    public IEnumerator Control(float time, CrowdControlEffect effect, FollowUpEffect followUpEffect = null)
    {
        for (float i = 0; i < time; i += 0.25f)
        {
            effect(controler);
           // Debug.Log("시간"+ i + "  "+ time);
            yield return new WaitForSeconds(0.25f);
        }
        followUpEffect?.Invoke(controler);
        //Debug.Log("복구");
        spriteRenderer.sprite = null;
        crowd = null;
    }

    private void OnDisable()
    {

        if (crowd != null)
        {
            StopCoroutine(crowd);
        }
        spriteRenderer.sprite = null;
        crowd = null;
    }
    private void OnEnable()
    {
        if (crowd != null)
        {
            StopCoroutine(crowd);
        }
        spriteRenderer.sprite = null;
        crowd = null;
    }
}
