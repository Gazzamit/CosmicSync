using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;

    //targets, score text and value
    private TextMeshProUGUI _targetsRemainingText, _scoreText, _multiplierTextL, _multiplierTextR, _velocityBonusText, _velocity, _countdownPoints, _countdownText;

    [SerializeField] private Color[] _Colours;

    [SerializeField] private int _streakCountForMultiply = 8;

    [SerializeField] private int _countDown, _countDownMultiplier;

    [SerializeField] Transform _scorePointsLocation, _velocityLocation;

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
    //private RectTransform _multiplierRect, _xRect;

    //Score Dictionary - tyoe if tap, points
    private Dictionary<string, int> _pointsByType = new Dictionary<string, int>();

    //after final target is destroyed, stop adding points
    private bool _stopAddingPoints = false, _freezeVelocityOnBonus = false, _addTimeBonusToScore = false;

    //slow update of velocity
    [SerializeField] private float _frequencyOfVelUpdate = 0.2f;
    private float _frequencyOfVelUpdateStart;
    private int _pointsBoost = 0;
    void Awake()
    {
        _instance = this;

        // Initialize the point values for each tap type
        _pointsByType.Add("miss", -10);
        _pointsByType.Add("poor", 1);
        _pointsByType.Add("good", 5);
        _pointsByType.Add("perfect", 10);
        _pointsByType.Add("portal", 0);
        _pointsByType.Add("laserRightOrder", 0);
        _pointsByType.Add("laserWrongOrder", 0);


        //find objects by tag
        _scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        _multiplierTextR = GameObject.FindGameObjectWithTag("MultiplierR").GetComponent<TextMeshProUGUI>();
        _multiplierTextL = GameObject.FindGameObjectWithTag("MultiplierL").GetComponent<TextMeshProUGUI>();
        _targetsRemainingText = GameObject.FindGameObjectWithTag("TargetsRemaining").GetComponent<TextMeshProUGUI>();
        _velocityBonusText = GameObject.FindGameObjectWithTag("SpeedBonusText").GetComponent<TextMeshProUGUI>();
        _velocity = GameObject.FindGameObjectWithTag("Velocity").GetComponent<TextMeshProUGUI>();
        _countdownText = GameObject.FindGameObjectWithTag("CountdownText").GetComponent<TextMeshProUGUI>();
        _countdownPoints = GameObject.FindGameObjectWithTag("CountdownPoints").GetComponent<TextMeshProUGUI>();

        //set alpha of speed bonus text to zero
        _velocityBonusText.alpha = 0;
        _countdownText.alpha = 0;
        _countdownPoints.alpha = 0;
        _velocity.text = "0";

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
        //_multiplierRect = _multiplierText.GetComponent<RectTransform>();
        //_xRect = _multiplierXText.GetComponent<RectTransform>();

        //show multiplier
        _multiplierTextL.text = "<size=30><sup>X</sup></size>" + _multiplier.ToString();
        _multiplierTextR.text = "<size=30><sup>X</sup></size>" + _multiplier.ToString();

        //set 'x' to left of multiplier text
        //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

        //set countdown text
        _countdownPoints.text = (_countDown * _countDownMultiplier).ToString();

        //get initial serialized value for resetting timer
        _frequencyOfVelUpdateStart = _frequencyOfVelUpdate;

        //for end of level testing - tap when in game to trigger
        //_finalTargetDestroyed = true;
    }

    void Update()
    {
        //allow slower update of speed
        _frequencyOfVelUpdate -= Time.deltaTime;
        if (_frequencyOfVelUpdate <= 0)
        {
            ShowSpeed();
            _frequencyOfVelUpdate = _frequencyOfVelUpdateStart;
        }

        //Set true at end of AddPoints, if finalTargetDestroyed = true
        if (_stopAddingPoints == true && _addTimeBonusToScore == false)
        {
            _addTimeBonusToScore = true;
            StartCoroutine(AddTimeBonusToScore()); //add bonus time to score at end of game
            return;
        }


    }

    //allow slower update of speed
    private void ShowSpeed()
    {
        if (_freezeVelocityOnBonus == true)
        {
            return;
        }
        else
        {
            _velocity.text = SpaceshipControls._magnitude.ToString() + "<size=8><sup>m/s</sup></size>";
        }
    }

    //trigger by pther script
    public void AddPoints(string _stringTypeForPoints)
    {
        Debug.Log("AddPoints Trigeered");

        //Set true at end of AddPoints, if finalTargetDestroyed = truew
        if (_stopAddingPoints == true)
        {
            return;
        }

        //increment perfect counter
        if (_stringTypeForPoints == "perfect")
        {
            Debug.Log("Increment streak & perfect counter");
            _perfectCounter++;
            _perfectStreak++;
        }
        else if (_stringTypeForPoints == "good" || _stringTypeForPoints == "poor")
        {
            Debug.Log("Reset streak & perfect counter. Multiplier -1");
            _perfectCounter = 0;
            _perfectStreak = 0;
            //reduce multiplier by one if bad hit
            if (_multiplier > 1) _multiplier--;
        }
        //set multiplier
        if (_perfectStreak == _streakCountForMultiply)
        {
            Debug.Log("Increment Multiplier");
            _multiplier++;
            _perfectStreak = 0;
            StartCoroutine(ScaleMultiplierText());
        }

        //set multiplier clour
        if (_multiplier > 1)
        {
            _multiplierTextL.color = _Colours[1];
            _multiplierTextR.color = _Colours[1];
        }
        else
        {
            _multiplierTextL.color = _Colours[0];
            _multiplierTextR.color = _Colours[0];

        }

        //show multiplier
        _multiplierTextL.text = "<size=30><sup>X</sup></size>" + _multiplier.ToString();
        _multiplierTextR.text = "<size=30><sup>X</sup></size>" + _multiplier.ToString();

        //set 'x' to left of multiplier text
        //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

        //minus point for miss (anti-constant tap)
        if (_stringTypeForPoints == "miss" && _score >= 10)
        {
            _score += _pointsByType[_stringTypeForPoints]; // -10
        }



        //add points by taptype * multiplier (only for taps)
        if ( _stringTypeForPoints == "poor" || _stringTypeForPoints == "good" || _stringTypeForPoints == "perfect")
        {
            int _points = _pointsByType[_stringTypeForPoints];
            _score += _points * _multiplier;
        }

        //default extra points set to zero for all these
        //could be added as a 'bonus' set of points seperate to the 'velocity' bonus'.
        //However, velocity bonus is the most important.
        if (_stringTypeForPoints == "laserRightOrder" || _stringTypeForPoints == "laserWrongOrder" || _stringTypeForPoints == "portal")
        {
            _pointsBoost = (int)(SpaceshipControls._magnitude + _pointsByType[_stringTypeForPoints]);
            Debug.Log("Show Bonus Points: " + _pointsBoost);
            StartCoroutine(ShowVelocityBonus());
            _freezeVelocityOnBonus = true;
            _score += _pointsBoost;
        }

        //show score
        _scoreText.text = _score.ToString();

        if (_finalTargetDestroyed == true)
        {
            _stopAddingPoints = true;
        }
    }

    //trigger by other collider scripts (laser / portal)
    public void TargetDestroyed()
    {
        _targetsRemaining -= 1;
        _targetsRemainingText.text = _targetsRemaining.ToString();

        //All targets destroyed - END OF LEVEL
        //This trigger InputMapSwitch to wait 10s then call switch back to menu via HUDAnimations
        if (_targetsRemaining == 0) _finalTargetDestroyed = true;
    }

    private IEnumerator ScaleMultiplierText()
    {
        for (int i = 0; i < 5; i++)
        {
            // Scale up the text
            _multiplierTextL.transform.DOScale(1.4f, 0.1f);
            _multiplierTextR.transform.DOScale(1.4f, 0.1f);

            //set 'x' to left of increased multiplier text
            //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);
            yield return new WaitForSeconds(0.1f);

            // Scale down the text using DOTween
            _multiplierTextL.transform.DOScale(1f, 0.1f);
            _multiplierTextR.transform.DOScale(1f, 0.1f);

            yield return new WaitForSeconds(0.1f);
        }
        //reset sizes
        _multiplierTextL.transform.localScale = Vector3.one;
        _multiplierTextL.transform.localScale = Vector3.one;
        _multiplierTextR.transform.localScale = Vector3.one;
        _multiplierTextR.transform.localScale = Vector3.one;
        //set 'x' to left of original multiplier text
        //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

    }

    private IEnumerator ShowVelocityBonus()
    {
        //velocity temporarily show velocity as points boost on laser/portal hit         
        _velocity.text = "+" + _pointsBoost.ToString();  //reset automatically to velocity via update()
        yield return new WaitForSeconds(0.5f);
        //fade + scale at same time
        _velocityBonusText.DOFade(1f, .2f);
        _velocityBonusText.transform.localScale = Vector3.zero;
        _velocity.transform.localScale = Vector3.zero;
        _velocityBonusText.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack);
        _velocity.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack);
        //wait
        yield return new WaitForSeconds(1f);
        //fade + scale back down
        _velocityBonusText.transform.DOScale(1f, 0.2f);
        _velocity.transform.DOScale(1f, 0.2f);
        _velocityBonusText.DOFade(0f, .2f);

        _freezeVelocityOnBonus = false;
    }

    //trigger by other script
    public void TriggerCountdownCoroutine()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        Debug.Log("Started Countdown: " + _countDown);
        yield return new WaitForSeconds(2);
        _countdownPoints.DOFade(1f, 1f);
        _countdownText.DOFade(1f, 1f);
        //wait for fade-up
        yield return new WaitForSeconds(1);
        while (_countDown > 0 && _finalTargetDestroyed == false)
        {
            _countDown--;
            if (_countDown < 10)
            {
                _countdownPoints.text = (_countDown * _countDownMultiplier).ToString();
                _countdownPoints.transform.DOScale(1.4f, 0.1f);
                yield return new WaitForSeconds(0.5f);
                _countdownPoints.transform.DOScale(1f, 0.1f);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                _countdownPoints.text = (_countDown * _countDownMultiplier).ToString();
            }
        }
        if (_countDown == 0)
        {
            _countdownPoints.color = _Colours[2]; //red
            _countdownText.color = _Colours[2]; //red
        }
    }

    private IEnumerator AddTimeBonusToScore()
    {
        //scaling tends towards 1 + 1/_scale factor (1 adds 1, 2 adds 0.5 etc)
        float _scaleFactor = 1f;
        int _limit = _countDown;
        //increase time bonus text size
        _countdownPoints.transform.DOScale(1f + 1f / _scaleFactor, 2f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(2f);
        //move and fade final bonus time score toward actual score text
        _countdownPoints.GetComponent<RectTransform>().DOMove(_scorePointsLocation.position, 6f);
        _countdownPoints.DOFade(0, 4f);
        for (int i = 0; i < _limit; i++)
        {
            _score += _countDownMultiplier;
            _countDown--;
            //increase score text while decreasing time bonus text
            _scoreText.transform.DOScale(1 + (float)(i / _limit) / _scaleFactor, 0.01f);
            _countdownPoints.transform.DOScale((1f + 1f / _scaleFactor) - (float)(i / _limit) / _scaleFactor, 0.01f);
            _scoreText.text = _score.ToString();
            _countdownPoints.text = (_countDown * _countDownMultiplier).ToString();
            yield return new WaitForSeconds(0.01f);
            //Debug.Log("CountDown Value: " + _countDown);
            //Debug.Log("i Value: " + i);
        }
        _countdownPoints.transform.DOScale(1f, 0.5f);
        _scoreText.transform.DOScale(1f, 0.5f);
        _countdownPoints.color = _Colours[0]; //blue
        _countdownText.color = _Colours[0]; //blue
        //pulse the score till screen removed automatically by end of game control
        StartCoroutine(PulseFinalScore());
    }

    private IEnumerator PulseFinalScore()
    {
        while (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            // Scale the image up to pulseScale over a duration of pulseDuration
            yield return _scoreText.transform.DOScale(1.1f, 0.3f).WaitForCompletion();

            // Scale the image back down to 1.0 over a duration of pulseDuration
            yield return _scoreText.transform.DOScale(1f, 0.3f).WaitForCompletion();
        }
    }
}
