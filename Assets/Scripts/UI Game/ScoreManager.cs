using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;

    //targets, score text and value
    private TextMeshProUGUI _targetsRemainingText, _scoreText, _multiplierXText, _multiplierText;

    [SerializeField] private Color[] _flashColours;

    //score value
    private int _score = 0;

    //target left value
    private int _targetsRemaining = 0;

    //Target Holder to track how many targets
    private GameObject _targetParent; //holds the targets
    public static bool _finalTargetDestroyed;

    //multiplier ints
    private int _perfectCounter = 0, _perfectStreak = 0, _multiplier = 1;
    //multiplier rect transforms to offest x to left of multiplier
    private RectTransform _multiplierRect, _xRect;

    //Score Dictionary - tyoe if tap, points
    private Dictionary<string, int> _pointsByTapType = new Dictionary<string, int>();
    void Awake()
    {
        _instance = this;

        // Initialize the point values for each tap type
        _pointsByTapType.Add("miss", 0);
        _pointsByTapType.Add("poor", 1);
        _pointsByTapType.Add("good", 5);
        _pointsByTapType.Add("perfect", 10);

        //find objects by tag
        _scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        _multiplierText = GameObject.FindGameObjectWithTag("Multiplier").GetComponent<TextMeshProUGUI>();
        _multiplierXText = GameObject.FindGameObjectWithTag("MultiplierX").GetComponent<TextMeshProUGUI>();
        _targetsRemainingText = GameObject.FindGameObjectWithTag("TargetsRemaining").GetComponent<TextMeshProUGUI>();

        //Calc number of targets
        _targetParent = GameObject.FindGameObjectWithTag("TargetHolder");
        _targetsRemaining = _targetParent.transform.childCount;
        _finalTargetDestroyed = false;
    }

    void Start()
    {
        //init display values
        _targetsRemainingText.text = _targetsRemaining.ToString();
        _scoreText.text = _score.ToString();

        //get rect of multiplier text so that x can offset to the left of it
        _multiplierRect = _multiplierText.GetComponent<RectTransform>();
        _xRect = _multiplierXText.GetComponent<RectTransform>();

        //show multiplier
        _multiplierText.text = _multiplier.ToString();
        //set 'x' to left of multiplier text
        _xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);


    }

    public void AddPoints(string _tapTypeString)
    {
        Debug.Log("AddPoints Trigeered");

        //increment perfect counter
        if (_tapTypeString == "perfect")
        {
            Debug.Log("Increment streak & perfect counter");
            _perfectCounter++;
            _perfectStreak++;
        }
        else if (_tapTypeString == "good" || _tapTypeString == "poor")
        {
            Debug.Log("Reset streak & perfect counter. Multiplier -1");
            _perfectCounter = 0;
            _perfectStreak = 0;
            //reduce multiplier by one if bad hit
            if (_multiplier > 1) _multiplier--;
        }
        //set multiplier
        if (_perfectStreak == 2)
        {
            Debug.Log("Increment Multiplier");
            _multiplier++;
            _perfectStreak = 0;
            StartCoroutine(FlashMultiplierText());
        }

        //show multiplier
        _multiplierText.text = _multiplier.ToString();
        //set 'x' to left of multiplier text
        _xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

        //add points by taptype
        int _points = _pointsByTapType[_tapTypeString];
        _score += _points * _multiplier;
        _scoreText.text = _score.ToString();
    }

    public void TargetDestroyed()
    {
        _targetsRemaining -= 1;
        _targetsRemainingText.text = _targetsRemaining.ToString();

        //All targets destroyed
        if (_targetsRemaining == 0) _finalTargetDestroyed = true;
    }
    IEnumerator FlashMultiplierText()
    {
        for (int i = 0; i < 3; i++)
        {
            _multiplierText.color = _flashColours[1];
            _multiplierXText.color = _flashColours[1];
            // Scale up the text
            _multiplierText.transform.DOScale(1.2f, 0.1f);
            //set 'x' to left of increased multiplier text
            //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);
            yield return new WaitForSeconds(0.1f);

            _multiplierText.color = _flashColours[0];
            _multiplierXText.color = _flashColours[0];
            // Scale down the text using DOTween
            _multiplierText.transform.DOScale(1f, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        //reset sizes
        _multiplierText.transform.localScale = Vector3.one;
        _multiplierXText.transform.localScale = Vector3.one;
        //set 'x' to left of original multiplier text
        _xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

    }
}
