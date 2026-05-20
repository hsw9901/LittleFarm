using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int Hour;
    public int Minute;
    public int Day;
    public int Season;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer = 0;
            Minute += 10;
        }
    }






}
