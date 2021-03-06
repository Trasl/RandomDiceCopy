﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Crowd;

public class EnemyControler : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer sprite;
    [SerializeField]
    Enemy enemy;
    Enemy enemyTmp;
    PoolChild child;
    [SerializeField]
    TextMeshPro text;
    [SerializeField]
    GameObject crowdControl;
    [SerializeField]
    string damageIndcatorsKey;
    ObjectPool damageIndecator;

    public Dictionary<OfensiveType, CrowdControl> crowdControls;
    Queue<Vector3> destinations;

    GameMaster master;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(1,1,1);
        crowdControls = new Dictionary<OfensiveType, CrowdControl>();
        enemyTmp = enemy;
    

    }
    void Start()
    {
        master = GameObject.Find(BoardType.UserBoard.ToString()).GetComponent<GameMaster>();
        child = GetComponent<PoolChild>();
        if (!damageIndcatorsKey.Equals(""))
        {
            damageIndecator = ObjectPoolManager.GetInstance().Get(damageIndcatorsKey);

        }

    }

    public IEnumerator MoveTo(Vector3 targetPositon)
    {


   //  Debug.Log("움직임 " + transform.localPosition +"  "+ targetPositon);
      

        for (; Vector3.Distance(transform.localPosition,targetPositon)>0.005;)
        {
         
            transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetPositon, enemy.speed*Time.deltaTime);
           // Debug.Log(name + transform.localPosition);
            yield return new WaitForFixedUpdate();
           
        }
        transform.localPosition = targetPositon;
        //Debug.Log(transform.localPosition);
        if (destinations.Count > 0)
        {
           
            StartCoroutine( MoveTo(destinations.Dequeue()));
   
        }
        else
        {

            Turn();

        }
        yield return null;

        
    }


    public void SetEnemy(Enemy enemy,Queue<Vector3> paths,Vector3 positon)
    {
        enemyTmp = enemy.Clone();

        if (destinations != null)
        {

            destinations.Clear();
         
        }
        StopAllCoroutines();
        //Debug.Log("set"+name);
        transform.localPosition = positon;
        destinations = new Queue<Vector3>(paths);
       // sprite.sprite = enemy.sprite;
        this.enemy = enemy;
        text.text = enemy.maxHp.ToString();
        enemy.hp = enemy.maxHp;
        if (enemy.sprite != null)
        {
            sprite.sprite = enemy.sprite;
        }
        if (destinations.Count > 0)
        {
            StartCoroutine(MoveTo(destinations.Dequeue()));
        }

    }

    public void Damage(float damage)
    {
        Damage(damage, Color.black);
    }

    public void Damage(float damage,Color color)
    {
        enemy.hp -= (int)damage;
        text.text = enemy.hp.ToString();
        if (damageIndecator != null)
        {
            FloatText floatText = damageIndecator.GetChild().GetComponent<FloatText>();
            // Debug.Log(floatText.name+"  " + floatText.isActiveAndEnabled);
            floatText.transform.position = transform.position;
            floatText.GetDamage((int)damage,color);

        }


        if (enemy.hp <= 0)
        {
            master.AddMoeny(100);
            Turn();
            if (enemy.isBoss)
            {
                StageManager.GetInstance().Vitory();
            }

           
        }
    }
    public void Debuff(float coefficient,float time ,EnemyStat stat)
    {
        time = time * 4;
        switch (stat)
        {
            case EnemyStat.speed:
                
                float speed = enemy.speed* coefficient /time;
                if (speed > enemyTmp.speed * 0.4f)
                {
                    enemy.speed = speed;
                   // Debug.Log("속도" + speed +"  "+ enemyTmp.speed * 0.4f);
                }
                else
                {
                    enemy.speed = enemyTmp.speed * 0.4f;
                  //  Debug.Log("속도" + enemy.speed + "  " + enemyTmp.speed * 0.4f);
                }
                break;
        }


    }

    public void Recover(EnemyStat stat)
    {
        switch (stat)
        {
            case EnemyStat.speed:
                enemy.speed = enemyTmp.speed;
                Debug.Log("복구"+enemy.speed+"   "+ enemyTmp.speed);
                break;
        }


    }
    private void Turn()
    {

        destinations.Clear();
        child.Turn();

    }


    public void SideEffect(float time,AttackType type,CrowdControlEffect effect, FollowUpEffect followUpEffect =null)
    {



        if (!crowdControls.ContainsKey(type.ofensiveType))
        {
            GameObject crowd = Instantiate(crowdControl);
            SizeFitter.FittingContent(crowd, gameObject);
            crowdControls.Add(type.ofensiveType, crowd.GetComponent<CrowdControl>());
            crowdControls[type.ofensiveType].controler = this;
        }
        crowdControls[type.ofensiveType].SetCrowdControl(time, type.effect, effect,followUpEffect);
        //   subSprites[0].sprite = sprite;
        //   IEnumerator coroutine =CrowdControl(time, effect);






    }



    private void OnDisable()
    {
       
        if (destinations != null)
        {
            destinations.Clear();
        }
   
     
    }

  
}
