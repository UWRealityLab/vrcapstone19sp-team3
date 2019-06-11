using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// This class is responsible for creating the bubble, moving the bubble, and destroying the bubble
    /// </summary>
    public class BubbleVisualizer : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("Position offset of the bubbles's target relative to Reference Transform")]
        private Vector3 _positionOffset = Vector3.zero;

        public Camera m_Camera;

        private Vector3 _bubbleVel = Vector3.zero;

        [Header("Chat Bubble")]
        [SerializeField, Tooltip("Prefab of the Chat bubble")]
        private GameObject _bubblePrefab = null;
        private Transform _bubbleInstance = null;
        private Vector3 initialScale;

        private float BASE_DISTANCE = 1.75f;
        private float BASE_TRAVEL_DISTANCE = 0.5f;

        #endregion

        #region Unity Methods
        /// <summary>
        /// Validates input variables
        /// </summary>
        void Awake()
        {
            if (null == _bubblePrefab)
            {
                Debug.LogError("Error: ChatBubbleLauncher._bubblePrefab is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Creates an instance of the bubble
        /// </summary>
        void OnEnable()
        {
            if (null != _bubbleInstance)
            {
                _bubbleInstance.GetComponent<CanvasGroup>().alpha = 1.0f;
            }
            else
            {
                _bubbleInstance = Instantiate(_bubblePrefab.transform, GetPosition(), Quaternion.identity);
                _bubbleInstance.SetParent(this.transform.parent.parent);
                initialScale = _bubbleInstance.localScale;
            }
        }

        /// <summary>
        /// Update bubble position and move it to new location
        /// </summary>
        void Update()
        {
            Vector3 position = GetPosition();
            if (Vector3.Distance(_bubbleInstance.position, position) > 0.15)
            {
                float distanceTraveled = Vector3.Distance(_bubbleInstance.position, position) / BASE_TRAVEL_DISTANCE;
                _bubbleInstance.position = Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 0.85f);
                _bubbleInstance.LookAt(m_Camera.transform);
                _bubbleInstance.RotateAround(_bubbleInstance.position, _bubbleInstance.up, 180f);
                float distanceFromImageToCamera = Vector3.Distance(transform.position, m_Camera.transform.position) / BASE_DISTANCE;
                _bubbleInstance.localScale = initialScale * distanceFromImageToCamera;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate and return the position which the bubble should be at
        /// </summary>
        /// <returns>The absolute position of the new target</returns>
        private Vector3 GetPosition()
        {
            if (_bubbleInstance != null)
            {
                float distanceFromImageToCamera = Vector3.Distance(transform.position, m_Camera.transform.position) / BASE_DISTANCE;
                return transform.position + transform.TransformVector(_positionOffset * distanceFromImageToCamera);
            }
            return transform.position + transform.TransformVector(_positionOffset);
        }
        #endregion
    }
}
