using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int minutes;
    private float timeValue;
    private float lastTimeToHead;

    [SerializeField]
    private Transform Head;
    [SerializeField]
    private int headTimer;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private AudioSource dollSing;
    [SerializeField]
    private AudioSource dollHeadOff;
    [SerializeField]
    private AudioSource dollHeadOn;

    public static bool headTime;
    public static bool headTimeFinish;

    [SerializeField]
    private int totalBots;

    [SerializeField]
    GameObject Bot;

    [SerializeField]
    Transform SpawnArea;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBots();
        headTime = false;
        timeValue = minutes * 60;
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
    }

    private void SpawnBots()
    {
        for (int i = 0; i < totalBots; i++)
            Instantiate(Bot, RandomPosition(), SpawnArea.rotation);
    }

    private Vector3 RandomPosition()
    {
        Vector3 origin = SpawnArea.position;
        Vector3 range = SpawnArea.localScale / 2.0f;
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x),
                                          Random.Range(-range.y, range.y),
                                          Random.Range(-range.z, range.z));
        Vector3 randomCoordinate = origin + randomRange;
        return randomCoordinate;
    }

    private void CountDown()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
        }

        DisplayTime(timeValue);
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
            timeToDisplay = 0;

        float mins = Mathf.FloorToInt(timeToDisplay / 60);
        float secs = Mathf.FloorToInt(timeToDisplay % 60);
        HeadTime(secs);

        timeText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }

    private void HeadTime(float secs)
    {
        if (timeValue <= 0)
        {
            headTimeFinish = true;
            RotHead(180);
            return;
        }

        if (secs % headTimer == 0 && secs != lastTimeToHead)
        {
            lastTimeToHead = secs;
            headTime = !headTime;

            if (headTime)
            {
                dollHeadOn.Play(0);
            }
            else
            {
                if (!dollSing.isPlaying)
                    dollHeadOff.Play(0);

                if (!dollSing.isPlaying)
                    dollSing.PlayDelayed(1);
            }
        }

        if (headTime)
            RotHead(180);
        else
            RotHead(0);
    }

    private void RotHead(int deg)
    {
        Vector3 direction = new Vector3(Head.rotation.eulerAngles.x, deg, Head.rotation.eulerAngles.z);
        Quaternion targetRotation = Quaternion.Euler(direction);
        Head.rotation = Quaternion.Lerp(Head.rotation, targetRotation, Time.deltaTime * 3);
    }
}
