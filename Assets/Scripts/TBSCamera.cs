using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TbsFramework.Cells;
using UnityEngine;

public class TBSCamera : MonoBehaviour
{
    private static TBSCamera _instance;
    public static  TBSCamera Instance => _instance;

    [HideInInspector]
    public bool didFocusOnIncomingArmy;

    public Vector3 lastCamPlayerPosition { get; set; }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public float cameraSpeed = 1f, edgeSize = 10f, focusSpeed = .5f;

    public Ease focusEase;

    private Vector3 _cameraPosition, _clampedCameraPos;

    private readonly string _horizontalAxis = "Horizontal", _verticalAxis = "Vertical";

    [SerializeField] private bool _overrideValues;

    [SerializeField] private float _minXPos, _maxXPos, _minYPos, _maxYPos, _clampedValueFactor = 8f;

    private float _horizontalValue, _verticalValue;

    public bool enableEdgeScrolling;

    public void SetMaxYPos(int value) { _maxYPos = value; }

    private void Start()
    {
        if (_overrideValues == false)
        {
            _minXPos = 100f;
            _minYPos = 100f;

            foreach (Cell cell in ObjectHolder.Instance.cellGrid.Cells)
            {
                if (cell.GetComponent<HexagonTile>().groundType == GroundType.Land)
                {
                    if (cell.transform.position.x < _minXPos) _minXPos = cell.transform.position.x;
                    if (cell.transform.position.x > _maxXPos) _maxXPos = cell.transform.position.x;
                    if (cell.transform.position.y < _minYPos) _minYPos = cell.transform.position.y;
                    if (cell.transform.position.y > _maxYPos) _maxYPos = cell.transform.position.y;
                }
            }
        }
    }

    private void Update()
    {
        _horizontalValue = Input.GetAxis(_verticalAxis);
        _verticalValue   = Input.GetAxis(_horizontalAxis);

        _cameraPosition = new Vector3(_horizontalValue, -_verticalValue);

        transform.position += _cameraPosition * cameraSpeed * Time.deltaTime;

        if (enableEdgeScrolling)
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
                transform.position += new Vector3(0f, -cameraSpeed * Time.deltaTime, 0f);
            if (Input.mousePosition.x < edgeSize)
                transform.position += new Vector3(0f, cameraSpeed * Time.deltaTime, 0f);
            if (Input.mousePosition.y > Screen.height - edgeSize)
                transform.position += new Vector3(cameraSpeed * Time.deltaTime, 0f, 0f);
            if (Input.mousePosition.y < edgeSize)
                transform.position += new Vector3(-cameraSpeed * Time.deltaTime, 0f, 0f);
        }

        _clampedCameraPos.z = transform.position.z;

        _clampedCameraPos.x = Mathf.Clamp(transform.position.x, _minXPos + _clampedValueFactor / 2f,
                                          _maxXPos                       - _clampedValueFactor / 2f);
        _clampedCameraPos.y = Mathf.Clamp(transform.position.y, _minYPos + _clampedValueFactor,
                                          _maxYPos                       - _clampedValueFactor);

        transform.position = _clampedCameraPos;
    }

    public void FocusOn(Transform destination)
    {
        DOTween.Kill(transform);

        transform.DOLocalMove(new Vector3(
                                          destination.position.x,
                                          destination.position.y,
                                          transform.position.z),
                              focusSpeed).SetEase(focusEase);
    }

    public void FocusOn(Vector3 destinationPos)
    {
        DOTween.Kill(transform);

        transform.DOLocalMove(new Vector3(
                                          destinationPos.x,
                                          destinationPos.y,
                                          transform.position.z),
                              focusSpeed).SetEase(focusEase);

        didFocusOnIncomingArmy = true;
    }

    public void ResetToPlayerPos()
    {
        if (AITurnSpeedController.Instance             != null &&
            AITurnSpeedController.Instance.aiTurnSpeed == AITurnSpeedController.AITurnSpeed.Fast)
            return;
        
        transform.DOLocalMove(new Vector3(
                                          lastCamPlayerPosition.x,
                                          lastCamPlayerPosition.y,
                                          transform.position.z),
                              focusSpeed).SetEase(focusEase);
    }
}