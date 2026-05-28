using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Inst {  get; private set; }
    public event Action OnDayChanged;
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public int Day { get; private set; }
    public int Season { get; private set; }
    private float _timer;

    private void Awake()
    {
        Inst = this;

    }

    private void Update()
    {
        if (!GameStateManager.Inst.IsPlaying)
        {
            return;
        }

        //테스트용 치트
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Hour = 0;
            Day++;
            Debug.Log("강제로 다음 날로 넘어갑니다!");
            OnDayChanged?.Invoke();
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            _timer = 0;
            AddMinutes(10);
        }
    }
    private void AddMinutes(int amount)
    {
        Minute += amount;

        if(Minute >= 60)
        {
            Minute = 0;
            Hour++;

            if(Hour >= 24)
            {
                Hour = 0;
                Day++;
                OnDayChanged?.Invoke();

                if(Day > 28)
                {
                    Day = 1;
                    Season++;
                }
            }
        }
    }
    public TimeSaveData PackingState()
    {
        return new TimeSaveData { Season = this.Season, Day = this.Day, Hour = this.Hour, Minute = this.Minute };
    }
    public void ReloadState(TimeSaveData data)
    {
        if (data == null) return;
        this.Season = data.Season;
        this.Day = data.Day;
        this.Hour = data.Hour;
        this.Minute = data.Minute;
        this._timer = 0f;
    }

}
