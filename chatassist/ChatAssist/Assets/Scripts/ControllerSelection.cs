using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    [RequireComponent(typeof(ControllerConnectionHandler))]
    public class ControllerSelection : MonoBehaviour
    {
        #region Public Variables
        //Needs access to list of things from 
        public LanguageScrollList lsl;
        //public ChatScrollList csl;


        #endregion

        #region Private Variables
        private ControllerConnectionHandler _controllerConnectionHandler;

        private int _lastLEDindex = -1;
        private int leftIndex = 0;
        private bool languageSelection = true;
        private int rightIndex = 0;
        #endregion

        #region Const Variables
        private const float TRIGGER_DOWN_MIN_VALUE = 0.2f;

        // UpdateLED - Constants
        private const float HALF_HOUR_IN_DEGREES = 15.0f;
        private const float DEGREES_PER_HOUR = 12.0f / 360.0f;

        private const int MIN_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock12);
        private const int MAX_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock6And12);
        private const int LED_INDEX_DELTA = MAX_LED_INDEX - MIN_LED_INDEX;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();

            MLInput.OnControllerButtonUp += HandleOnButtonUp;
            MLInput.OnControllerButtonDown += HandleOnButtonDown;
            MLInput.OnTriggerDown += HandleOnTriggerDown;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateLED();
            // TODO: Look at ControllerStatsText for how to get direction of motion
            // Then after that, MLInputController -> MLInputControllerTouchpadGesture -> MLInputControllerTouchpadGestureDirection
            if (_controllerConnectionHandler.IsControllerValid())
            {
                MLInputController controller = _controllerConnectionHandler.ConnectedController;
                if (controller.Type == MLInputControllerType.Control)
                {
                    // None = 0,
                    // Up = 1,
                    // Down = 2,
                    // Left = 3,
                    // Right = 4,
                    // In = 5,
                    // Out = 6,
                    // Clockwise = 7,
                    // CounterClockwise = 8
                    // controller.TouchpadGesture.Direction
                    MLInputControllerTouchpadGestureDirection dir = controller.TouchpadGesture.Direction;
                    if (dir == MLInputControllerTouchpadGestureDirection.Down)
                    {
                        // Move down list
                        if (languageSelection && leftIndex > 0) leftIndex--;
                        //if (!languageSelection && rightIndex > 0) rightIndex--;
                        // unbold previous (current) text, update current text, bold current text. 
                        // If it goes above or below the screen, need to update the LanguageScrollList view thing to capture the change
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Up)
                    {
                        // move up list
                        if (languageSelection && leftIndex < lsl.toggles.Count - 1) leftIndex++;
                        // if (!languageSelection && rightIndex < csl.log.Count - 1) rightIndex++;
                        // unbold previous (current) text, update current text, bold current text. 
                        // If it goes above or below the screen, need to update the LanguageScrollList view thing to capture the change
                        // Maybe need scrollbar incrementer 
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Left)
                    {
                        // move to left menu
                        // TODO: Maybe check for two consecutive lefts or rights before switching
                        if (!languageSelection) languageSelection = true;
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Right)
                    {
                        // move to right menu
                        if (languageSelection) languageSelection = false;
                    }
                    
                }
            }
        }


        /// <summary>
        /// Updates LED on the physical controller based on touch pad input.
        /// </summary>
        private void UpdateLED()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                return;
            }

            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller.Touch1Active)
            {
                // Get angle of touchpad position.
                float angle = -Vector2.SignedAngle(Vector2.up, controller.Touch1PosAndForce);
                if (angle < 0.0f)
                {
                    angle += 360.0f;
                }

                // Get the correct hour and map it to [0,6]
                int index = (int)((angle + HALF_HOUR_IN_DEGREES) * DEGREES_PER_HOUR) % LED_INDEX_DELTA;

                // Pass from hour to MLInputControllerFeedbackPatternLED index  [0,6] -> [MAX_LED_INDEX, MIN_LED_INDEX + 1, ..., MAX_LED_INDEX - 1]
                index = (MAX_LED_INDEX + index > MAX_LED_INDEX) ? MIN_LED_INDEX + index : MAX_LED_INDEX;

                if (_lastLEDindex != index)
                {
                    // a duration of 0 means leave it on indefinitely
                    controller.StartFeedbackPatternLED((MLInputControllerFeedbackPatternLED)index, MLInputControllerFeedbackColorLED.BrightCosmicPurple, 0);
                    _lastLEDindex = index;
                }
            }
            else if (_lastLEDindex != -1)
            {
                controller.StopFeedbackPatternLED();
                _lastLEDindex = -1;
            }
        }

        #region Event Handlers
        /// <summary>
        /// Handles the event for button down.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="button">The button that is being pressed.</param>
        private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller != null && controller.Id == controllerId &&
                button == MLInputControllerButton.Bumper)
            {
                // Demonstrate haptics using callbacks.
                controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceDown, MLInputControllerFeedbackIntensity.Medium);
                // Toggle UseCFUIDTransforms
                controller.UseCFUIDTransforms = !controller.UseCFUIDTransforms;
            }
        }

        /// <summary>
        /// Handles the event for button up.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="button">The button that is being released.</param>
        private void HandleOnButtonUp(byte controllerId, MLInputControllerButton button)
        {
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller != null && controller.Id == controllerId &&
                button == MLInputControllerButton.Bumper)
            {
                // Demonstrate haptics using callbacks.
                controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.Medium);
            }
        }

        /// <summary>
        /// Handles the event for trigger down.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="value">The value of the trigger button.</param>
        private void HandleOnTriggerDown(byte controllerId, float value)
        {
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller != null && controller.Id == controllerId)
            {
                // TODO: CHANGE INTENSITY to just be a slight buzz.
                MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(value * 2.0f));
                controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, intensity);
            }
        }
        #endregion
    }
}