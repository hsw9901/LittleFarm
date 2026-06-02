using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HUDManager : UIBase
{
    [Header("시간 UI 연결")]
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DateText;
    [Header("아이콘")]
    public Image TimeIconImage;
    public Sprite DayIcon;
    public Sprite SunsetIcon;
    public Sprite NightIcon;
    [Header("시간별 화살표 설정")]
    public GameObject ArrowMorning;   
    public GameObject ArrowAfternoon; 
    public GameObject ArrowNight;

    private void OnEnable()
    {
        if (TimeManager.Inst != null)
        {
            TimeManager.Inst.OnTimeChanged += UpdateTimeUI;
            TimeManager.Inst.OnDayChanged += UpdateDateUI;

            UpdateTimeUI();
            UpdateDateUI();
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Inst != null)
        {
            TimeManager.Inst.OnTimeChanged -= UpdateTimeUI;
            TimeManager.Inst.OnDayChanged -= UpdateDateUI;
        }
    }

    private void UpdateTimeUI()
    {
        int hour = TimeManager.Inst.Hour;
        int minute = TimeManager.Inst.Minute;

        string amPm = hour >= 12 ? "오후" : "오전";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;

        TimeText.text = $"{amPm} {displayHour:D2}:{minute:D2}";

        if (TimeIconImage != null)
        {
            if (hour >= 6 && hour < 16) TimeIconImage.sprite = DayIcon;
            else if (hour >= 16 && hour < 19) TimeIconImage.sprite = SunsetIcon;
            else TimeIconImage.sprite = NightIcon;
        }
        if (ArrowMorning != null && ArrowAfternoon != null && ArrowNight != null)
        {
            ArrowMorning.SetActive(false);
            ArrowAfternoon.SetActive(false);
            ArrowNight.SetActive(false);

            if (hour >= 6 && hour < 16)
            {
                ArrowMorning.SetActive(true);
            }
            else if (hour >= 16 && hour < 19)
            {
                ArrowAfternoon.SetActive(true);
            }
            else
            {
                ArrowNight.SetActive(true);
            }
        }
    }
    private void UpdateDateUI()
    {
        string seasonName = "봄";

        switch (TimeManager.Inst.Season % 4)
        {
            case 0: seasonName = "봄"; break;
            case 1: seasonName = "여름"; break;
            case 2: seasonName = "가을"; break;
            case 3: seasonName = "겨울"; break;
        }

        DateText.text = $"{seasonName} {TimeManager.Inst.Day}일";
    }
}