using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// This class is responsible for creating the planet, moving the planet, and destroying the planet
    /// as well as creating the explorers
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
        /// Creates an instance of the planet
        /// </summary>
        void OnEnable()
        {
            //_planetInstance = Instantiate(_planetPrefabAnimator.transform, GetPosition(), Quaternion.identity);
            if (null != _bubbleInstance)
            {
                _bubbleInstance.gameObject.SetActive(true);
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
                _bubbleInstance.gameObject.SetActive(false);
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
            // Update planet position
            _bubbleInstance.position = Vector3.SmoothDamp(_bubbleInstance.position, position, ref _bubbleVel, 1.0f);
            _bubbleInstance.LookAt(m_Camera.transform);
            _bubbleInstance.RotateAround(_bubbleInstance.position, _bubbleInstance.up, 180f);

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
