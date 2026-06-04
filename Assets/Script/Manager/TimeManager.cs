using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Inst {  get; private set; }
    public event Action OnDayChanged;
    public event Action OnTimeChanged;
    public int Hour { get; private set; } = 0;
    public int Minute { get; private set; } = 0;
    public int Day { get; private set; } = 1;
    public int Season { get; private set; } = 0;
    public int Year { get; private set; } = 1;
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
            Debug.Log("강제로 다음 날로 넘어갑니다!");
            ChangeToNextDay();
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
                ChangeToNextDay();
            }
        }
        OnTimeChanged?.Invoke();
    }
    public void ChangeToNextDay()
    {
        Hour = 0;
        Minute = 0; 
        Day++;

        if (Day > 28)
        {
            Day = 1;
            Season++;

            if (Season > 3)
            {
                Season = 0;
                Year++;
            }
        }

        OnDayChanged?.Invoke();
        OnTimeChanged?.Invoke();
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
