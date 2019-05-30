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
        /// Destroys the planet instance
        /// </summary>
        void OnDisable()
        {
            if (null != _bubbleInstance)
            {
                //_bubbleInstance.gameObject.SetActive(false);
                //_bubbleInstance.GetComponent<CanvasGroup>().alpha = 0.0f;
            }
        }

        /// <summary>
        /// Update planet position and launch explorers
        /// </summary>
        void Update()
        {
            Vector3 position = GetPosition();

            /*
            if (Vector3.Distance(Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 0.5f), m_Camera.transform.position) < 0.9)
            {
                //Debug.Log("changing bubbles local scale");
                _bubbleInstance.localScale = Vector3.Scale(initialScale, new Vector3(.5f, .5f, .5f));
            } else
            {
                _bubbleInstance.localScale = initialScale;
            }
            */
            Debug.Log("Transform position: " + transform.position);
            if (Vector3.Distance(_bubbleInstance.position, position) > 0.15)
            {
                //Debug.Log("Updating position from " + _bubbleInstance.position);
                //Debug.Log("To new position: " + Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 0.5f));
                //Debug.Log("---------------------------------");
                Debug.Log("Bubble velocity: " + _bubbleVel);
                float distanceTraveled = Vector3.Distance(_bubbleInstance.position, position) / BASE_TRAVEL_DISTANCE;
                //_bubbleVel *= distanceTraveled;
                _bubbleInstance.position = Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 0.85f);
                _bubbleInstance.LookAt(m_Camera.transform);
                _bubbleInstance.RotateAround(_bubbleInstance.position, _bubbleInstance.up, 180f);
                float distanceFromImageToCamera = Vector3.Distance(transform.position, m_Camera.transform.position) / BASE_DISTANCE;
                _bubbleInstance.localScale = initialScale * distanceFromImageToCamera;
                Debug.Log("Bubble instance position: " + _bubbleInstance.position);
                //Debug.Log("Bubble instance initial scale: " + initialScale);
                //Debug.Log("Bubble instance current scale: " + _bubbleInstance.localScale);
            }

            // Change local scale of bubble with respect to the camera, should be compared to original bubble size, not current
            /*
            float distFromBubbleToCamera = Vector3.Distance(m_Camera.transform.position, _bubbleInstance.position);
            distFromBubbleToCamera /= 1;
            Debug.Log("distance from bubble to camera: " + distFromBubbleToCamera);
            */
            //Debug.Log(Vector3.Distance(_bubbleInstance.position, m_Camera.transform.position));
            //Debug.Log("position offset: " +_positionOffset);
            /*
            if (Vector3.Distance(_bubbleInstance.position, m_Camera.transform.position) < 0.9)
            {
                Debug.Log("changing bubbles local scale");
                _bubbleInstance.localScale = Vector3.Scale(initialScale, new Vector3(.5f, .5f, .5f));
            }
            */


            // Debug.Log("Updating position to " + _bubbleInstance.position);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate and return the position which the bubble should be at
        /// </summary>
        /// <returns>The absolute position of the new target</returns>
        private Vector3 GetPosition()
        {

            //Debug.Log("Getting current position: " + transform.position);
            //Debug.Log("Getting local direction position: " + transform.TransformDirection(_positionOffset));
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
