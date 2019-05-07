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

        /*
        [Header("Planet")]
        [SerializeField, Tooltip("Prefab of the bubble")]
        private Animator _planetPrefabAnimator = null;
        private Transform _planetInstance = null;
        */
        private Vector3 _planetVel = Vector3.zero;

        [Header("Explorer")]
        [SerializeField, Tooltip("Prefab of the Chat bubble")]
        private GameObject _explorerPrefab = null;
        private Transform _explorerInstance = null;

        /*
        private float _timeLastLaunch = 0;
        [SerializeField, Tooltip("Time interval between instances (seconds)")]
        private float _timeInterval = 0.5f;

        [SerializeField, Tooltip("Minimum distance from the center of the planet")]
        private float _minOrbitRadius = 0.1f;
        [SerializeField, Tooltip("Maximum distance from the center of the planet")]
        private float _maxOrbitRadius = 0.2f;
        */
        #endregion

        #region Unity Methods
        /// <summary>
        /// Validates input variables
        /// </summary>
        void Awake()
        {
            /*
            if (null == _planetPrefabAnimator)
            {
                Debug.LogError("Error: DeepSpaceExplorerLauncher._planetPrefabAnimator is not set, disabling script.");
                enabled = false;
                return;
            }
            */
            if (null == _explorerPrefab)
            {
                Debug.LogError("Error: DeepSpaceExplorerLauncher._explorerPrefab is not set, disabling script.");
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
            if (null != _explorerInstance)
            {
                _explorerInstance.gameObject.SetActive(true);
            }
            else
            {
                _explorerInstance = Instantiate(_explorerPrefab.transform, GetPosition(), Quaternion.identity);
                _explorerInstance.SetParent(this.transform.parent.parent);
            }
        }

        /// <summary>
        /// Destroys the planet instance
        /// </summary>
        void OnDisable()
        {
            if (null != _explorerInstance)
            {
                //_planetInstance.GetComponent<Animator>().Play("EarthShrinking");
                _explorerInstance.gameObject.SetActive(false);
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

            // Update planet position
            _explorerInstance.position = Vector3.SmoothDamp(_explorerInstance.position, position, ref _planetVel, 1.0f);
            _explorerInstance.LookAt(m_Camera.transform);
            _explorerInstance.RotateAround(_explorerInstance.position, _explorerInstance.up, 180f);

            /*
            // Launch explorers
            if (Time.time - _timeInterval > _timeLastLaunch)
            {
                _timeLastLaunch = Time.time;
                GameObject explorer = Instantiate(_explorerPrefab, position, Random.rotation);
                DeepSpaceExplorerController explorerController = explorer.GetComponent<DeepSpaceExplorerController>();
                if (explorerController)
                {
                    explorerController.OrbitRadius = Random.Range(_minOrbitRadius, _maxOrbitRadius);
                }
            }
            */
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate and return the position which the explorers should look at
        /// </summary>
        /// <returns>The absolute position of the new target</returns>
        private Vector3 GetPosition()
        {
            return transform.position + transform.TransformDirection(_positionOffset);
        }
        #endregion
    }
}
