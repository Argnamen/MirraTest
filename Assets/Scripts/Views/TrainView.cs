using ClockApp.ViewModels;
using ClockApp.Views;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TrainView : ClockView
{
    [SerializeField] private TrainImages trainSprites;
    [SerializeField] private TrainImages sunTrainSprites;
    [SerializeField] private Button trainButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Transform trainContainer;
    [SerializeField] private TMP_Text trainText;
    [SerializeField] private Image trainImage;

    private bool _isShow = false;
    private int _slideNumber = 0;

    [Inject]
    public void Construct(ClockViewModel viewModel)
    {
        Initialize(viewModel);
    }

    public override void Initialize(ClockViewModel viewModel)
    {
        base.Initialize(viewModel);

        trainButton.onClick.AddListener(Show);
        continueButton.onClick.AddListener(NextSlide);
    }

    private void Show()
    {
        _isShow = !_isShow;
        trainContainer.gameObject.SetActive(_isShow);
        Slides(_slideNumber);
    }

    private void NextSlide()
    {
        _slideNumber++;
        Slides(_slideNumber);
    }

    private void DownSlide()
    {
        _slideNumber--;
        Slides(_slideNumber);
    }

    private void Slides(int i)
    {
        if (ViewModel.CurrentTime.Value.Month > 8 || ViewModel.CurrentTime.Value.Month < 5)
        {
            switch (i)
            {
                case 0:
                    trainImage.overrideSprite = trainSprites.Sprites[0];
                    trainText.text = "Приветствую в Часах! \n Странно да? Это же всего лишь часы. Чего на них смотреть? \n Тем не менее, я должна показать как они работают";
                    break;
                case 1:
                    trainImage.overrideSprite = trainSprites.Sprites[1];
                    trainText.text = "Надпись EDIT отвечает за редактирование времени. Нажми на неё, когда захочешь изменить время. \n Или просто наслаждайся тем как медленно течёт время";
                    break;
                case 2:
                    trainImage.overrideSprite = trainSprites.Sprites[2];
                    trainText.text = "Слева мои любимые аналоговые часы! Стрелочки так прикольно двигаются. А ещё в режиме EDIT их можно двигатть, чтобы изменять время!";
                    break;
                case 3:
                    trainImage.overrideSprite = trainSprites.Sprites[1];
                    trainText.text = "Справа дата и время в циферном представлении. Их тоже можно менять. Зачем? Кто знает. Но в сегоднешнее время такой формат наиболее понятет людям";
                    break;
                case 4:
                    trainImage.overrideSprite = trainSprites.Sprites[1];
                    trainText.text = "Ну и внизу 2 кнопки: Save и Cancle. Нажми на Save чтобы сохранить изменения и на Cancle чтобы вернуться к последнему сохранненому времени";
                    break;
                case 5:
                    trainImage.overrideSprite = trainSprites.Sprites[3];
                    trainText.text = "Вот и всё. Не скучай! Хотя время вообще штука скучная. И зачем ты это открыл я не понимаю. \n Но думаю ты найдешь способ как себя развлечь";
                    break;
                default:
                    _slideNumber = 0;
                    Show();
                    break;
            }
        }
        else
        {
            switch (i)
            {
                case 0:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[0];
                    trainText.text = "Приветствую в Часах! \n Странно да? Это же всего лишь часы. Чего на них смотреть? \n Тем более летом!";
                    break;
                case 1:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[1];
                    trainText.text = "Надпись EDIT отвечает за редактирование времени. Нажми на неё, когда захочешь изменить время. \n Или просто наслаждайся тем как медленно течёт время";
                    break;
                case 2:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[2];
                    trainText.text = "Слева мои любимые аналоговые часы! Стрелочки так прикольно двигаются. А ещё в режиме EDIT их можно двигатть, чтобы изменять время!";
                    break;
                case 3:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[1];
                    trainText.text = "Справа дата и время в циферном представлении. Их тоже можно менять";
                    break;
                case 4:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[1];
                    trainText.text = "Ну и внизу 2 кнопки: Save и Cancle. Нажми на Save чтобы сохранить изменения и на Cancle чтобы вернуться к последнему сохранненому времени";
                    break;
                case 5:
                    trainImage.overrideSprite = sunTrainSprites.Sprites[3];
                    trainText.text = "Вот и всё. Не скучай! Ведь когда ещё можно так веселиться как не летом?";
                    break;
                default:
                    _slideNumber = 0;
                    Show();
                    break;
            }
        }
    }
}


