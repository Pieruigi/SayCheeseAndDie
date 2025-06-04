using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SCAD
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text speedField;

        [SerializeField]
        TMP_Text noiseField;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void LateUpdate()
        {
            speedField.text = $"{PlayerController.Instance.Speed:F2}";
            noiseField.text = $"{PlayerController.Instance.NoiseRange:F2}";
        }
    }
    
}
