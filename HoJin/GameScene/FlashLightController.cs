using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Events;
using HoJin.GameScene;

namespace HoJin.GameScene
{
    [Serializable]
    public struct FlashLightState
    {
        [SerializeField] private float intensity;
        [SerializeField] private float inAngle;
        [SerializeField] private float outAngle;

        public float Intensity { get => intensity; }
        public float InAngle { get => inAngle; }
        public float OutAngle { get => outAngle; }
        
        public FlashLightState(float intensity, float inAngle, float outAngle)
        {
            this.intensity = intensity;
            this.inAngle = inAngle;
            this.outAngle = outAngle;
        }

        public void SetLightSetting(Light light)
        {
            light.intensity = intensity;
            light.innerSpotAngle = inAngle;
            light.spotAngle = outAngle;
        }
    }

    public class FlashLightController : GameDirector
    {
        #region 필드
        private new Light light;
        private int currentFlashState;
        private bool isUsable;

        [SerializeField] private AudioSource switchSound;
        [SerializeField] private FlashLightState[] flashLightStates;
        #endregion

        #region 속성
        public Light GetLight()
        {
            return light;
        }

        public int GetCurrentFlashState()
        {
            return currentFlashState;
        }

        public bool IsUsable { get => isUsable; set => isUsable = value; }
        #endregion



        private void Awake()
        {
            TryGetComponent(out light);
            currentFlashState = -1;
            isUsable = true;
        }

        private void Update()
        {
            if (GameManager.Instance.InputFlashLight())
            {
                if (isUsable == true)
                {
                    ChangeFlashLight();
                }
            }
        }



        public void ChangeFlashLight()
        {
            try
            {
                flashLightStates[++currentFlashState].SetLightSetting(light);
                light.enabled = true;
            }
            catch (IndexOutOfRangeException)
            {
                currentFlashState = -1;
                light.enabled = false;
            }

            switchSound.Play();
        }
    }
}