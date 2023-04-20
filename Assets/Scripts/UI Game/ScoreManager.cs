using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;

    //targets, score text and value
    private TextMeshProUGUI _targetsRemainingText, _scoreText, _multiplierTextL, _multiplierTextR, _velocityBonusText, _velocityText, _countdownText, _countdownTitleText, _bonusPointMultiplierText, _levelCompleted;

    [SerializeField] private Color[] _Colours;

    [SerializeField] private int _streakCountForMultiply = 8;

    [SerializeField] private float _countDown;
    [SerializeField] private int _countDownMultiplier;

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
    private bool _stopAddingPoints = false, _freezeVelocityOnEOL = false, _addTimeBonusToScore = false;

    //slow update of velocity
    [SerializeField] private float _frequencyOfVelUpdate = 0.2f;
    private float _frequencyOfVelUpdateStart;
    private int _pointsBoost = 0;
    void Awake()
    {
        _instance = this;

        // Initialize the point values for each tap type
        _pointsByType.Add("miss", -10);
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
        _velocityText = GameObject.FindGameObjectWithTag("Velocity").GetComponent<TextMeshProUGUI>();
        _countdownTitleText = GameObject.FindGameObjectWithTag("CountdownText").GetComponent<TextMeshProUGUI>();
        _countdownText = GameObject.FindGameObjectWithTag("CountdownPoints").GetComponent<TextMeshProUGUI>();
        _bonusPointMultiplierText = GameObject.FindGameObjectWithTag("CountDownMultiplierText").GetComponent<TextMeshProUGUI>();
        _levelCompleted = GameObject.FindGameObjectWithTag("LevelCompleted").GetComponent<TextMeshProUGUI>();

        //set countdown multiplier text fade out as not using
        _bonusPointMultiplierText.DOFade(0, 0);

        //set alpha text to zero
        _velocityBonusText.alpha = 0;
        _countdownTitleText.alpha = 0;
        _countdownText.alpha = 0;
        _levelCompleted.alpha = 0;
        _velocityText.text = "0";

        //Calc number of targets
        _targetParent = GameObject.FindGameObjectWithTag("TargetHolder");
        _targetsRemaining = _targetParent.transform.childCount;
        _finalTargetDestroyed = false;

        if (GameManagerDDOL._doWelcome == true)
        {
            Debug.Log("SM - doWelcome set score / target numbers");
            _score = 3258;
            _targetsRemaining = 1;
            _scoreText.text = _score.ToString();
            _targetsRemainingText.text = _targetsRemaining.ToString();
        }
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
        _multiplierTextL.text = "<size=40><sup>x</sup></size>" + _multiplier.ToString();
        _multiplierTextR.text = "<size=40><sup>x</sup></size>" + _multiplier.ToString();

        //set 'x' to left of multiplier text
        //_xRect.anchoredPosition = new Vector2(_multiplierRect.anchoredPosition.x - _multiplierText.preferredWidth / 2 - _multiplierXText.preferredWidth / 2, _xRect.anchoredPosition.y);

        //set countdown text
        _countdownText.text = _countDown.ToString();

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
        if (_finalTargetDestroyed == true && _addTimeBonusToScore == false)
        {
            Debug.Log("SM - Add Countdown Bonus to Score");
            _addTimeBonusToScore = true;
            StartCoroutine(AddTimeBonusToScore()); //add bonus time to score at end of game
        }


    }

    //allow slower update of speed
    private void ShowSpeed()
    {
        if (_freezeVelocityOnEOL == true)
        {
            return;
        }
        else
        {
            _velocityText.text = SpaceshipControls._magnitude.ToString() + "<size=8><sup>m/s</sup></size>";
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
            //_score += _pointsByType[_stringTypeForPoints]; // -10
        }



        //add points by taptype * multiplier (only for taps)
        if (_stringTypeForPoints == "poor" || _stringTypeForPoints == "good" || _stringTypeForPoints == "perfect")
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
            _freezeVelocityOnEOL = true;
            _score += _pointsBoost;
        }

        //show score
        _scoreText.text = _score.ToString();

        if (_finalTargetDestroyed == true)
        {
            Debug.Log("SM - Stop Adding Points");
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
        if (_targetsRemaining == 0)
        {
            _finalTargetDestroyed = true;
            Debug.Log("SM - Final Target Destroyed");
        }
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
        _velocityText.text = "+" + _pointsBoost.ToString();  //reset automatically to velocity via update()
        yield return new WaitForSeconds(0.5f);
        //fade + scale at same time
        _velocityBonusText.DOFade(1f, .2f);
        _velocityBonusText.transform.localScale = Vector3.zero;
        _velocityText.transform.localScale = Vector3.zero;
        _velocityBonusText.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack);
        _velocityText.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack);
        //wait
        yield return new WaitForSeconds(1f);
        //fade + scale back down
        _velocityBonusText.transform.DOScale(1f, 0.2f);
        _velocityText.transform.DOScale(1f, 0.2f);
        _velocityBonusText.DOFade(0f, .2f);

        _freezeVelocityOnEOL = false;
    }

    //trigger by other script
    public void TriggerCountdownCoroutine()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("SM - Started Countdown: " + _countDown);
        //highlight countdown value to user
        //_countdownText.color = _Colours[1];//grn
        _countdownText.DOFade(1f, 1f);
        //_countdownTitleText.DOFade(1f, 1f);
        //_countdownText.transform.DOScale(2.5f, .4f);
        //_countdownText.transform.DOScale(1f, .2f).SetDelay(.4f);
        //_countdownText.DOColor(_Colours[0], 4f); // gradually change colour to blue

        //decrement countdown
        while (_countDown > 0 && _finalTargetDestroyed == false)
        {
            _countDown -= Time.deltaTime;
            _countdownText.text = ((int)_countDown + "s").ToString();
            yield return null;
        }

        //run out of time
        if (_countDown == 0)
        {
            _countdownText.color = _Colours[2]; //red
            //_countdownTitleText.color = _Colours[2]; //red
        }

        //_countdownText.text = ((int)_countDown).ToString(); //set to just number with no 's'
    }

    IEnumerator AddTimeBonusToScore()
    {
        if (GameManagerDDOL._doWelcome == false)
        {
            //pulse tiime, then show level completed
            StartCoroutine(PulseFinalTime());
            yield return new WaitForSeconds(2f);
            StartCoroutine(LevelCompleted());

            //show bonus points
            _countdownTitleText.transform.DOScale(0.1f, 0.1f).WaitForCompletion();
            _countdownTitleText.text = "BONUS POINTS" + "\n" + (((int)_countDown) * _countDownMultiplier).ToString();
            _countdownTitleText.DOFade(1.5f, 0.2f);
            //move text down a little over same 0.2
            _countdownTitleText.GetComponent<RectTransform>().DOMove(_countdownTitleText.transform.position + new Vector3(0, -1.4f, 0), 0.2f).SetEase(Ease.InOutSine);
            _countdownTitleText.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack);

            //see the time and multiplier values
            yield return new WaitForSeconds(3f);

            //move title text to score
            _countdownTitleText.GetComponent<RectTransform>().DOMove(_scorePointsLocation.position, 3f).SetEase(Ease.InOutSine);
            _countdownTitleText.DOFade(0, 2.5f);
            _countdownTitleText.transform.DOScale(0.8f, 3f);

            //increase score text while reseting to 1 the timer countdown text
            _scoreText.transform.DOScale(1.5f, 3f);
            _countdownText.transform.DOScale(1f, 3f);

            //_limit is what is left of countdown timer
            int _limit = ((int)_countDown);

            for (int i = 0; i < _limit; i++)
            {
                _score += _countDownMultiplier; //add count down multiplier until i < limit
                _scoreText.text = _score.ToString();
                _countdownTitleText.text = "BONUS POINTS" + "\n" + (((int)_countDown - i) * _countDownMultiplier).ToString();
                yield return new WaitForSeconds(0.01f);
            }
            _countdownText.transform.DOScale(1f, 0.5f);
            _countdownText.color = _Colours[0]; //blue
            _countdownTitleText.color = _Colours[0]; //blue

            //pulse text till screen removed automatically by end of game control
            StartCoroutine(PulseFinalScore());
        }
    }

    private IEnumerator PulseFinalScore()
    {
        //reset size
        _scoreText.transform.DOScale(1f, 1f);
        yield return new WaitForSeconds(1f);
        while (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            // Scale the image up 
            yield return _scoreText.transform.DOScale(1.1f, 0.3f).WaitForCompletion();

            // Scale the image back down
            yield return _scoreText.transform.DOScale(1f, 0.3f).WaitForCompletion();
        }
    }

    private IEnumerator PulseFinalTime()
    {
        //reset size
        _countdownText.transform.DOScale(1f, 1f);
        yield return new WaitForSeconds(1f);
        while (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            // Scale the image up 
            yield return _countdownText.transform.DOScale(1.05f, 0.3f).WaitForCompletion();

            // Scale the image back down
            yield return _countdownText.transform.DOScale(1f, 0.3f).WaitForCompletion();
        }
    }

    private IEnumerator LevelCompleted()
    {
        _levelCompleted.DOFade(1, 4f);

        yield return new WaitForSeconds(1f);
        while (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            // Scale the image up
            yield return _levelCompleted.transform.DOScale(1.02f, 0.3f).WaitForCompletion();

            // Scale the image back down 
            yield return _levelCompleted.transform.DOScale(0.99f, 0.3f).WaitForCompletion();
        }

    }
}
