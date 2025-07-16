using TMPro;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class PatientMonitorUI : MonoBehaviour
    {
        [Header("Waveform Labels")]
        [SerializeField] private TextMeshProUGUI heartRate;
        [SerializeField] private TextMeshProUGUI resp;
        [SerializeField] private TextMeshProUGUI spo2;
        [SerializeField] private TextMeshProUGUI co2;
        [SerializeField] private TextMeshProUGUI bloodPressure;
        [SerializeField] private TextMeshProUGUI pulseRate;

        [Header("Line Values")]
        [SerializeField] private GameObject mainLine;
        [SerializeField] private Transform lineStartPosition;
        [SerializeField] private Transform lineEndPosition;
        [SerializeField] private float scrollSpeed;

        private void Update()
        {
            mainLine.transform.localPosition += new Vector3(scrollSpeed, 0, 0);
            if (scrollSpeed > 0 && mainLine.transform.localPosition.x > lineEndPosition.localPosition.x)
            {
                mainLine.transform.localPosition = lineEndPosition.localPosition;
            }
            else if (scrollSpeed < 0 && mainLine.transform.localPosition.x < lineEndPosition.localPosition.x)
            {
                mainLine.transform.localPosition = lineStartPosition.localPosition;
            }
        }
        
        public void UpdateHeartRateWaveform(int heartRateValue)
        {
            heartRate.text = heartRateValue.ToString();
        }

        public void UpdateRESPWaveform(int respValue)
        {
            resp.text = respValue.ToString();
        }

        public void UpdateSpO2Waveform(int spO2Value)
        {
            spo2.text = spO2Value.ToString();
        }

        public void UpdateCO2Waveform(int co2Value)
        {
            co2.text = co2Value.ToString();
        }

        //TODO: figure out if pulse rate is needed/pulse rate values
        public void UpdateBloodPressure(int firstBPValue, int secondBPValue, int pulseRateValue)
        {
            bloodPressure.text = firstBPValue.ToString() + "/" + secondBPValue.ToString();
            pulseRate.text = "(" + pulseRateValue.ToString() + ")";
        }
    }
}