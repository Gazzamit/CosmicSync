using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;

    //score text and value
    private TextMeshProUGUI _scoreText;
    private int _score = 0;

    //target text and value
    private TextMeshProUGUI _targetsRemainingText;
    private int _targetsRemaining = 0;

    //Target Holder to track how many targets
    private GameObject _targetParent; //holds the targets
    public static bool _finalTargetDestroyed;

    //Score Dictionary
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
    }

    public void AddPoints(string _tapTypeString)
    {
        //add points by taptype
        int _points = _pointsByTapType[_tapTypeString];
        _score += _points;
        _scoreText.text = _score.ToString();
    }

    public void TargetDestroyed()
    {
        _targetsRemaining -= 1;
        _targetsRemainingText.text = _targetsRemaining.ToString();

        //All targets destroyed
        if (_targetsRemaining == 0) _finalTargetDestroyed = true;
    }

}
