using UnityEngine;
using System.Collections.Generic;

namespace MagicLeap
{
    /// <summary>
    /// This class is responsible for creating the planet, moving the planet, and destroying the planet
    /// as well as creating the bubbles
    /// </summary>
    public class MultiBubbleVisualizer : MonoBehaviour
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

        [Header("Bubble prefab")]
        [SerializeField, Tooltip("Prefab of the Chat bubble")]
        private GameObject _bubblePrefab = null;
        private Transform _bubbleInstance = null;

        // Testing out multiple chat bubbles at a time
        private LinkedList<Transform> list = new LinkedList<Transform>();

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
            if (null == _bubblePrefab)
            {
                Debug.LogError("Error: DeepSpaceExplorerLauncher._bubblePrefab is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Creates an instance of the chat bubble
        /// </summary>
        void OnEnable()
        {
            //_planetInstance = Instantiate(_planetPrefabAnimator.transform, GetPosition(), Quaternion.identity);
            for (int i = 0; i < 5; i++)
            {
                _bubbleInstance = Instantiate(_bubblePrefab.transform, GetPosition(), Quaternion.identity);
            }
        }

        /// <summary>
        /// Destroys the planet instance
        /// </summary>
        void OnDisable()
        {
            if (null != _bubbleInstance)
            {
                Destroy(_bubbleInstance.gameObject, 1.1f);
                _bubbleInstance = null;
                list.Clear();
            }
        }

        /// <summary>
        /// Update planet position and launch bubbles
        /// </summary>
        void Update()
        {
            Vector3 position = GetPosition();

            // Update planet position
            for (int i = 0; i < list.Count; i++)
            {
                _bubbleInstance.position = Vector3.SmoothDamp(_bubbleInstance.position, position, ref _planetVel, 1.0f);
                _bubbleInstance.LookAt(m_Camera.transform);
                _bubbleInstance.RotateAround(_bubbleInstance.position, _bubbleInstance.up, 180f);
            }
            /*
            // Launch bubbles
            if (Time.time - _timeInterval > _timeLastLaunch)
            {
                _timeLastLaunch = Time.time;
                GameObject bubble = Instantiate(_bubblePrefab, position, Random.rotation);
                DeepSpaceExplorerController bubbleController = bubble.GetComponent<DeepSpaceExplorerController>();
                if (bubbleController)
                {
                    bubbleController.OrbitRadius = Random.Range(_minOrbitRadius, _maxOrbitRadius);
                }
            }
            */
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculate and return the position which the bubbles should look at
        /// </summary>
        /// <returns>The absolute position of the new target</returns>
        private Vector3 GetPosition()
        {
            return transform.position + transform.TransformDirection(_positionOffset);
        }
        #endregion
    }
}
