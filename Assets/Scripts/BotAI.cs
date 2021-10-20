using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    private Transform TargetEnd;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    AudioSource feetSteps;

    [SerializeField]
    AudioSource shoot;

    private Transform deathZone;

    private bool isWalking;
    private bool AIChecked;
    private bool isStopped;
    private bool isInDeathZone;
    private bool isDying;
    private float intelligence;

    // Start is called before the first frame update
    void Start()
    {
        speed -= Random.Range(0f, 1f);
        intelligence = Random.Range(0f, 100f);

        TargetEnd = GameObject.Find("TargetEnd").transform;
        deathZone = GameObject.Find("DeathZone").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
            return;

        Move();
        CheckDeathTime();
    }

    private void Move()
    {
        if (!GameManager.headTime)
        {
            AIChecked = false;
            Walk();
        }
        else
        {
            if (!AIChecked)
            {
                AIChecked = true;
                float probDead = Random.Range(0f, 100f);

                if (probDead < intelligence)
                    Stop();
            }
            else if (AIChecked && !isStopped)
            {
                Walk();
            }
        }
    }

    private void Walk()
    {
        isWalking = true;
        isStopped = false;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, TargetEnd.position, step);

        anim.SetBool("isWalking", true);
        feetSteps.loop = true;
        feetSteps.Play(0);
    }

    private void Stop()
    {
        isWalking = false;
        isStopped = true;
        anim.SetBool("isWalking", false);
        feetSteps.loop = false;
    }

    private void CheckDeathTime()
    {
        if (GameManager.headTime && isWalking || GameManager.headTimeFinish)
        {
            if (!isInDeathZone)
                return;

            isDying = true;
            StartCoroutine(DeadthAnimation());
        }
    }

    IEnumerator DeadthAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        anim.SetBool("isDying", true);
        feetSteps.Stop();
        shoot.Play(0);
    }


    private void OnTriggerEnter(Collider other)
    {
        isInDeathZone = other.transform == deathZone;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == deathZone)
            isInDeathZone = false;
    }
}
