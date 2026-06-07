using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHelper : MonoBehaviour
{
    [SerializeField] private Image _tutorialOverlay;
    [SerializeField] private Canvas _tutorialCanvas;
    [SerializeField] private TMP_Text _tutorialStepsText;
    [SerializeField] private HandPointer _handPointer;
    [SerializeField] private Button _tapZoneButon;
    [SerializeField] private GameObject _targetGridSetup;

    private int _tutorialStep;
    public int TutorialStep => _tutorialStep;

    public HandPointer HandPointer => _handPointer;

    private GameSettings _gameSettings;

    public void Init(EssentialConfigData essentialConfigData)
    {
        _gameSettings = essentialConfigData.AccessConfig<GameSettings>();
        _handPointer.Init(_gameSettings);

        UpdateTutorialCanvas(false);
    }

    public void UpdateTutorialCanvas(object obj)
    {
        _tutorialCanvas.enabled = (bool)obj;
    }

    private void OnEnable()
    {
        GameHelper.Instance.StartListening(GameConstants.TutorialStep, UpdateTutorialStep);
        GameHelper.Instance.StartListening(GameConstants.OnUpdateTutorialCanvas, UpdateTutorialCanvas);
    }

    private void OnDisable()
    {
        GameHelper.Instance.StopListening(GameConstants.TutorialStep, UpdateTutorialStep);
        GameHelper.Instance.StopListening(GameConstants.OnUpdateTutorialCanvas, UpdateTutorialCanvas);
    }

    private void UpdateTutorialStep(object obj)
    {
        object[] dataObjects = (object[])obj;
        _tutorialStep = (int)dataObjects[1];

        _tutorialStepsText.text = (string)dataObjects[0];
        _tutorialOverlay.enabled = _tutorialStep == 2 ? true : false;
        _targetGridSetup.SetActive(_tutorialStep == 2 ? true : false);
        HandleTapZoneButton(_tutorialStep == 2 ? true : false);
    }

    private void HandleTapZoneButton(bool isInteractible)
    {
        _tapZoneButon.enabled = isInteractible;
        _tapZoneButon.gameObject.GetComponent<Image>().enabled = isInteractible;
    }

    public void OnTapZone()
    {
        _tutorialStep = 3;
    }
}
