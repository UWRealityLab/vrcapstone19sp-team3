using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using WebSocketSharp;
using System;

namespace MagicLeap
{
    [RequireComponent(typeof(ControllerConnectionHandler))]
    public class ControllerSelection : MonoBehaviour
    {
        #region Public Variables
        //Needs access to list of things from 
        public LanguageScrollList lsl;
        public ChatList cl;
        public CanvasGroup languageMenuOpacity;
        public CanvasGroup chatLogOpacity;
        public Text instructions;
        public Text tracking;
        public Text tracking2; 
        #endregion

        #region Private Variables
        private ControllerConnectionHandler _controllerConnectionHandler;

        private int _lastLEDindex = -1;
        public float leftIndex;
        private float selection;
        public float rightIndex = 0;
        private LanguageToggle currTog;
        private Text currText;

        #endregion

        #region Const Variables
        private const float TRIGGER_DOWN_MIN_VALUE = 0.4f;
        // TODO: Change this to make the scroll easier to use;
        private const float SCROLL_LENGTH = 0.3f;
        private const float L_SCROLL_LENGTH = 0.1f;

        // UpdateLED - Constants
        private const float HALF_HOUR_IN_DEGREES = 15.0f;
        private const float DEGREES_PER_HOUR = 12.0f / 360.0f;

        private const int MIN_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock12);
        private const int MAX_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock6And12);
        private const int LED_INDEX_DELTA = MAX_LED_INDEX - MIN_LED_INDEX;
        bool reset = true;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("DOES THIS EVER CALL START?");
            _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();
            MLInput.OnControllerButtonUp += HandleOnButtonUp;
            MLInput.OnControllerButtonDown += HandleOnButtonDown;
            MLInput.OnTriggerDown += HandleOnTriggerDown;
            currText = null;
            currTog = lsl.toggles[(int)leftIndex]; // TODO: Possibly change this so that it isn't 0 initially;
            //Debug.Log(lsl.toggles.Count);
            //Debug.Log("DOES THIS EVER CALL START?");
            currTog.languageName.fontStyle = FontStyle.Bold;
            selection = 0.0f;
            leftIndex = 1.0f;

            // Makes sure that the WebSocket is initialized
            WebSocketManager.initialize();
        }

        float prevDist = 0.0f;
        // Update is called once per frame
        void Update()
        {
            UpdateLED();
            // TODO: Look at ControllerStatsText for how to get direction of motion
            // Then after that, MLInputController -> MLInputControllerTouchpadGesture -> MLInputControllerTouchpadGestureDirection

            if (_controllerConnectionHandler.IsControllerValid())
            {
                MLInputController controller = _controllerConnectionHandler.ConnectedController;
                Debug.Log("distance " + controller.TouchpadGesture.Distance + " prevDist " + prevDist + " direction " + controller.TouchpadGesture.Direction
                     + " leftindex " + leftIndex + " rightindex " + rightIndex);
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

                    // Get the direction
                    MLInputControllerTouchpadGestureDirection dir = controller.TouchpadGesture.Direction;

                    float scroll_len = Math.Abs(controller.TouchpadGesture.Distance - prevDist);
                    float speed = Math.Abs(controller.TouchpadGesture.Speed) * 0.1f;
                    prevDist = controller.TouchpadGesture.Distance;

                    // Changes opacity depending on which is selected
                    if (selection > 0.0f) languageMenuOpacity.alpha = languageMenuOpacity.alpha > 0.7f ? languageMenuOpacity.alpha - 0.1f : 0.7f;
                    else languageMenuOpacity.alpha += 0.2f;

                    if (selection <= 0.0f) chatLogOpacity.alpha = chatLogOpacity.alpha > 0.7f ? chatLogOpacity.alpha - 0.1f : 0.7f;
                    else chatLogOpacity.alpha += 0.2f;

                    // If the controller isn't active, just return right away
                    if (!controller.Touch1Active) return;

                    // Checks if clockwise and otherwise
                    if (dir == MLInputControllerTouchpadGestureDirection.Clockwise)
                    {
                        // Move down list
                        if (selection <= 0.0f) {
                            currTog.languageName.fontStyle = FontStyle.Normal;
                            if (leftIndex + scroll_len < lsl.toggles.Count) leftIndex += scroll_len;
                            currTog = lsl.toggles[(int)leftIndex];
                            currTog.languageName.fontStyle = FontStyle.Bold;
                        }

                        //if (!languageSelection && rightIndex > 0) rightIndex--;
                        //ShowInLanguageScroll((int)leftIndex);
                        if (selection > 0.0f)
                        {
                            if (currText != null) currText.fontStyle = FontStyle.Normal;
                            if (rightIndex + scroll_len < cl.list.Count) rightIndex += scroll_len;
                            if (rightIndex > 0 && (int)rightIndex < cl.list.Count) currText = cl.list[(int)rightIndex];
                            if (currText != null) currText.fontStyle = FontStyle.Bold;
                        }
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.CounterClockwise)
                    {
                        // move up list
                        if (selection <= 0.0f)
                        {
                            currTog.languageName.fontStyle = FontStyle.Normal;
                            if (leftIndex - scroll_len > -0.5f) leftIndex -= scroll_len;
                            if (leftIndex - scroll_len <= -0.5f) leftIndex = 0.0f;
                            currTog = lsl.toggles[(int)leftIndex];
                            currTog.languageName.fontStyle = FontStyle.Bold;
                        }
                        // if (!languageSelection && rightIndex < csl.log.Count - 1) rightIndex++;
                        // Maybe need scrollbar incrementer 
                        //ShowInLanguageScroll((int)leftIndex);
                        if (selection > 0.0f)
                        {
                            if (currText != null) currText.fontStyle = FontStyle.Normal;
                            if (rightIndex > cl.list.Count) rightIndex = cl.list.Count - 1;
                            if (rightIndex - scroll_len > -0.5f) rightIndex -= scroll_len;
                            if (rightIndex - scroll_len <= -0.5f) rightIndex = 0.0f;
                            //Debug.Log(rightIndex);
                            if (rightIndex > 0 && (int)rightIndex < cl.list.Count) currText = cl.list[(int)rightIndex];
                            if (currText != null) currText.fontStyle = FontStyle.Bold;
                        }
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Up)
                    {

                        if (selection > 0.0f)
                        {
                            if (currText != null) currText.fontStyle = FontStyle.Normal;
                            if (rightIndex > cl.list.Count) rightIndex = cl.list.Count - 1;
                            if (rightIndex - speed > -0.5f) rightIndex -= speed;
                            if (rightIndex - speed <= -0.5f) rightIndex = 0.0f;
                            //Debug.Log(rightIndex);
                            if (rightIndex > 0 && (int)rightIndex < cl.list.Count) currText = cl.list[(int)rightIndex];
                            if (currText != null) currText.fontStyle = FontStyle.Bold;
                        }
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Down)
                    {
                        if (selection > 0.0f)
                        {
                            if (currText != null) currText.fontStyle = FontStyle.Normal;
                            if (rightIndex + speed < cl.list.Count) rightIndex += speed;
                            if (rightIndex > 0 && (int)rightIndex < cl.list.Count) currText = cl.list[(int)rightIndex];
                            if (currText != null) currText.fontStyle = FontStyle.Bold;
                        }
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Left)
                    {
                        // move to left menu
                        // TODO: Maybe check for two consecutive lefts or rights before switching
                        if (selection > -1.0f) selection -= scroll_len * 2;
                    }
                    else if (dir == MLInputControllerTouchpadGestureDirection.Right)
                    {
                        // move to right menu
                        if (selection < 1.0f) selection += scroll_len * 2;
                        else if (selection + 0.1f > 1.0f)
                        {
                            if (currText != null) currText.fontStyle = FontStyle.Normal;
                            if ((int)rightIndex < cl.list.Count)
                            {
                                rightIndex = cl.list.Count - 0.5f;
                                currText = cl.list[(int)rightIndex];
                            }
                            if (currText != null) currText.fontStyle = FontStyle.Bold;
                            selection = 0.6f;
                        }
                    }

                }
            }
            if (reset)
            {
                leftIndex = 0.0f;
                reset = false;
            }
        }

        /// <summary>
        /// Stop input api and unregister callbacks.
        /// </summary>
        void OnDestroy()
        {
            if (MLInput.IsStarted)
            {
                MLInput.OnTriggerDown -= HandleOnTriggerDown;
                MLInput.OnControllerButtonDown -= HandleOnButtonDown;
                MLInput.OnControllerButtonUp -= HandleOnButtonUp;
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
                instructions.gameObject.SetActive(!instructions.IsActive());
                tracking.gameObject.SetActive(!tracking.IsActive());
                tracking2.gameObject.SetActive(!tracking2.IsActive());
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
        //int count = 0;
        private void HandleOnTriggerDown(byte controllerId, float value)
        {
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller != null && controller.Id == controllerId)
            {
                // TODO: CHANGE INTENSITY to just be a slight buzz.
                MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(value * 1.0f));
                controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, intensity);
                if (selection <= 0.0f)
                {
                    lsl.SetLanguage((int)leftIndex);
                    WebSocketManager.send(lsl.GetLanguageCode());
                }
                //if (selection > 0.0f)
                //{
                //    for (int i = 0; i < 10; i++)
                //        addText(count++ + "");
                //}
            }
        }

        public void addText(string text)
        {
            cl.AddChatBox(text);
            if ((int)rightIndex == cl.list.Count - 2) rightIndex++;
            if ((int)rightIndex > cl.list.Count) rightIndex = cl.list.Count - 1;
            if (currText != null) currText.fontStyle = FontStyle.Normal;
            if (rightIndex > 0 && (int)rightIndex < cl.list.Count)
            {
                currText = cl.list[(int)rightIndex];
            }
            if (currText != null) currText.fontStyle = FontStyle.Bold;
        }
        #endregion
    }
}