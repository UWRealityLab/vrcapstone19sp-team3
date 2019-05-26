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
            }
        }

        /// <summary>
        /// Destroys the planet instance
        /// </summary>
        void OnDisable()
        {
            if (null != _bubbleInstance)
            {
                //_planetInstance.GetComponent<Animator>().Play("EarthShrinking");
                //_bubbleInstance.gameObject.SetActive(false);
                //_bubbleInstance.GetComponent<CanvasGroup>().alpha = 0.0f;
                //Destroy(_explorerInstance.gameObject, 1.1f);
                //_explorerInstance = null;
            }
        }

        /// <summary>
        /// Update planet position and launch explorers
        /// </summary>
        void Update()
        {
            Vector3 position = GetPosition();
            //Debug.Log("Updating position from " + _bubbleInstance.position);
            _bubbleInstance.position = Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 0.2f);
            _bubbleInstance.LookAt(m_Camera.transform);
            _bubbleInstance.RotateAround(_bubbleInstance.position, _bubbleInstance.up, 180f);

            // Change local scale of bubble with respect to the camera, should be compared to original bubble size, not current
            /*
            float distFromBubbleToCamera = Vector3.Distance(m_Camera.transform.position, _bubbleInstance.position);
            distFromBubbleToCamera /= 1;
            Debug.Log("distance from bubble to camera: " + distFromBubbleToCamera);
            _bubbleInstance.localScale = Vector3.Scale(_bubbleInstance.localScale, new Vector3(distFromBubbleToCamera, distFromBubbleToCamera, distFromBubbleToCamera));
            */

            // Debug.Log("Updating position to " + _bubbleInstance.position);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate and return the position which the explorers should look at
        /// </summary>
        /// <returns>The absolute position of the new target</returns>
        private Vector3 GetPosition()
        {
            /*
            Debug.Log("Getting current position: " + transform.position);
            Debug.Log("Getting position: " + transform.TransformDirection(_positionOffset));
            Debug.Log("Getting position offset: " + _positionOffset);
            */
            return transform.position + transform.TransformDirection(_positionOffset);
        }
        #endregion
    }
}
