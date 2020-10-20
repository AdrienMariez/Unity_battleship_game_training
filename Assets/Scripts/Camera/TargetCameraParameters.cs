using System;
using UnityEngine;


    public class TargetCameraParameters : MonoBehaviour
    {
        // This script is designed to be placed on each possible camera target to tweak the camera distance.
        private float CameraDistance = 24;  // x
        private float CameraHeight = 2;    // y
        private float CameraLateralOffset = 0;   // z

        public void SetCameraDistance(float cameraDistance) {CameraDistance = cameraDistance; }
        public void SetCameraHeight(float cameraHeight) {CameraHeight = cameraHeight; }
        public void SetCameraLateralOffset(float cameraLateralOffset) {CameraLateralOffset = cameraLateralOffset; }
        public float GetCameraDistance(){ return CameraDistance; }
        public float GetCameraHeight(){ return CameraHeight; }
        public float GetCameraLateralOffset(){ return CameraLateralOffset; }
    }
