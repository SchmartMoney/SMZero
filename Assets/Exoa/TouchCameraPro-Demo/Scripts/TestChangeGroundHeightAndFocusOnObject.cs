using Exoa.Cameras;
using Exoa.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exoa.Cameras.Demos
{
    [AddComponentMenu("Exoa/Demo/ChangeGroundHeight")]
    public class TestChangeGroundHeightAndFocusOnObject : MonoBehaviour
    {
        private CameraBase cb;
        public Transform transparentGrid;
        public Transform objLevel2;
        public Transform objLevel1;
        public Transform objLevel0;
        public Button level1Btn;
        public Button level2Btn;
        public Button level3Btn;
        private float targetGroundHeight;
        private float groundHeight;
        public bool changeGroundHeight = true;

        void Start()
        {
            cb = GetComponent<CameraBase>();

            groundHeight = targetGroundHeight = cb.groundHeight;
            level1Btn.onClick.AddListener(() => ChangeLevel(objLevel0));
            level2Btn.onClick.AddListener(() => ChangeLevel(objLevel1));
            level3Btn.onClick.AddListener(() => ChangeLevel(objLevel2));

            CameraEvents.OnFocusComplete += OnFocusComplete;
        }

        private void ChangeLevel(Transform targetObj)
        {
            targetGroundHeight = targetObj.position.y;
            cb.FocusCamera(targetObj.position);
        }

        void OnFocusComplete(GameObject obj)
        {
            if (changeGroundHeight)
            {
                cb.SetGroundHeight(targetGroundHeight);
                transparentGrid.position = transparentGrid.position.SetY(targetGroundHeight + .01f);
            }
        }
    }
}
