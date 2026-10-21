using System.Collections.Generic;
using Deckbuilder.Cards;
using Deckbuilder.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Deckbuilder.Combat
{
    public class TargetingController : MonoBehaviour
    {
        #region Variables
        
        public bool IsTargeting => _currentSelectedCard != null;

        private Camera _mainCamera;
        private CombatantBase _currentTarget;
        private PlayerCard _currentSelectedCard;
        
        private const int TargetingCurvePoints = 20;

        [Header("Line Renderer")] [SerializeField]
        private LineRenderer targetingLine;
        [SerializeField] private float textureScale = 1.0f;
        [SerializeField] private Color normalTargetingColor = Color.white;
        [SerializeField] private Color validTargetColor = Color.red;

        #endregion


        #region Unity Lifecycle

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            GameEvents.CardSelected += OnCardSelected;
        }

        private void Start()
        {
            ConfigureLineRenderer();

            ManageTargetingArrowVisibility(false);
            HideTargetingLine();
        }

        private void Update()
        {
            if (!_currentSelectedCard) return;

            CombatantBase target = GetTargetUnderMouse();

            if (!IsValidTarget(target))
            {
                target = null;
            }

            UpdateTargetingLine(target);

            if (!Input.GetMouseButtonUp(0)) return;
            
            if (target)
            {
                ConfirmTarget(target);
            }
            else
            {
                CancelTargeting();
            }
        }

        private void OnDisable()
        {
            GameEvents.CardSelected -= OnCardSelected;
        }

        #endregion


        #region Card Selection

        private void OnCardSelected(PlayerCard card)
        {
            ShowTargetingLine();
            _currentSelectedCard = card;
        }

        private CombatantBase GetTargetUnderMouse()
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (!hit) return null;

            return hit.GetComponent<CombatantBase>();
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane combatPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));

            return combatPlane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
        }

        private void ConfirmTarget(CombatantBase target)
        {
            List<CombatantBase> targets = new()
            {
                target
            };

            if (_currentSelectedCard.CardData is AttackCardData attackCardData)
            {
                GameManager.Instance.Player.ExecuteAttackVisual(attackCardData.ActionId);
            }

            _currentSelectedCard.PlayCard(targets);

            HideTargetingLine();
            _currentSelectedCard = null;
        }

        private void CancelTargeting()
        {
            _currentSelectedCard.ResetCardVisuals();
            HideTargetingLine();
            _currentSelectedCard = null;
        }

        private bool IsValidTarget(CombatantBase target)
        {
            if (!target) return false;

            return target is Enemy && target.IsAlive;
        }

        #endregion


        #region LineRenderer

        private void ShowTargetingLine()
        {
            targetingLine.positionCount = TargetingCurvePoints;
            targetingLine.enabled = true;
            
            ManageTargetingArrowVisibility(true);
        }

        private void HideTargetingLine()
        {
            targetingLine.enabled = false;
            targetingLine.positionCount = 0;
            
            ManageTargetingArrowVisibility(false);
        }

        private void UpdateTargetingLine(CombatantBase target)
        {
            Vector3 startPosition = GetCardTargetingPosition();
            Vector3 endPosition = GetMouseWorldPosition();

            Vector3 controlPoint = GetTargetingControlPoint(startPosition, endPosition);
            Vector3 arrowPosition = EvaluateQuadraticBezier(startPosition, controlPoint, endPosition, arrowT);
            Vector3 nextPosition = EvaluateQuadraticBezier(startPosition, controlPoint, endPosition, arrowT + 0.01f);

            Vector3 direction = (nextPosition - arrowPosition).normalized;
            Vector3 lineEnd = arrowPosition - direction * lineArrowGap;

            UpdateArrow(arrowPosition, direction);
            DrawTargetingCurve(startPosition, controlPoint, lineEnd);

            SetColor(IsValidTarget(target) ? validTargetColor : normalTargetingColor);
        }

        private void SetColor(Color color)
        {
            targetingLine.startColor = color;
            targetingLine.endColor = color;
            targetingArrow.color = color;
        }

        private void ConfigureLineRenderer()
        {
            targetingLine.textureMode = LineTextureMode.RepeatPerSegment;
            targetingLine.textureScale = Vector2.one * textureScale;
            targetingLine.positionCount = TargetingCurvePoints;
            targetingLine.startWidth = 0.03f;
            targetingLine.endWidth = 0.03f;
        }
        
        private Vector3 GetCardTargetingPosition()
        {
            if (!_currentSelectedCard)
            {
                return Vector3.zero;
            }

            RectTransform rectTransform = _currentSelectedCard.GetComponent<RectTransform>();
            return rectTransform.TransformPoint(new Vector3(rectTransform.rect.center.x, rectTransform.rect.yMax, 0f));
        }

        #endregion


        #region Bezier Curve

        [Header("Bezier Curve")]
        [SerializeField] private float curveHeight = 1.5f;
        [SerializeField] private float maxCurveHeight = 3f;

        private Vector3 GetTargetingControlPoint(Vector3 start, Vector3 end)
        {
            Vector3 midpoint = Vector3.Lerp(start, end, 0.5f);
            float distance = Vector3.Distance(start, end);

            float height = Mathf.Min(distance * curveHeight, maxCurveHeight);
            midpoint.y += height;
            return midpoint;
        }
        
        private Vector3 EvaluateQuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
        {
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * control + t * t * end;
        }

        private void DrawTargetingCurve(Vector3 start, Vector3 control, Vector3 end)
        {
            for (int i = 0; i < TargetingCurvePoints; i++)
            {
                float t = i / (float)(TargetingCurvePoints - 1);

                Vector3 position = EvaluateQuadraticBezier(start, control, end, t);
                targetingLine.SetPosition(i, position);
            }
        }
        
        #endregion
        
        
        #region Targeting

        [Header("Targeting Arrow Setup")]
        [SerializeField] private RectTransform targetingArrowRect;
        [SerializeField] private Image targetingArrow;
        [SerializeField] private float arrowT = 0.98f;
        [SerializeField] private float lineArrowGap = 0.2f;

        private void ManageTargetingArrowVisibility(bool activeStatus)
        {
            targetingArrow.enabled = activeStatus;
        }

        private void UpdateArrow(Vector3 position, Vector3 direction)
        {
            targetingArrowRect.position = position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetingArrowRect.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        #endregion
    }
}