using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using TMPro;
using VARLab.RespiratoryTherapy;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.PlayMode
{
    public class PatientMonitorUIIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        GameObject line;
        Transform lineStart;
        Transform lineEnd;

        TextMeshProUGUI heartRate;
        TextMeshProUGUI resp;
        TextMeshProUGUI spO2;
        TextMeshProUGUI co2;
        TextMeshProUGUI bloodPressure;
        TextMeshProUGUI pulseRate;

        PatientMonitorUI patientMonitor;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign values
            line = GameObject.Find("Line");
            lineStart = GameObject.Find("LineStartPosition").transform;
            lineEnd = GameObject.Find("LineEndPosition").transform;

            heartRate = GameObject.Find("Heartrate").GetComponent<TextMeshProUGUI>();
            resp = GameObject.Find("Respiration Rate").GetComponent<TextMeshProUGUI>();
            spO2 = GameObject.Find("Oxygen Saturation").GetComponent<TextMeshProUGUI>();
            co2 = GameObject.Find("Carbon Dioxide").GetComponent<TextMeshProUGUI>();
            bloodPressure = GameObject.Find("Blood Pressure").GetComponent<TextMeshProUGUI>();
            pulseRate = GameObject.Find("Pulse Rate").GetComponent<TextMeshProUGUI>();

            patientMonitor = FindFirstObjectByType<PatientMonitorUI>();
        }

        [UnityTearDown]
        public void TearDown()
        {
            line = null;
            lineStart = null;
            lineEnd = null;

            heartRate = null;
            resp = null;
            spO2 = null;
            co2 = null;
            bloodPressure = null;
            pulseRate = null;

            patientMonitor = null;
        }

        [UnityTest]
        public IEnumerator UpdateHeartRateWaveform_ShouldSetHeartRateLabel()
        {
            //Act
            patientMonitor.UpdateHeartRateWaveform(50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(heartRate.text, "50");
        }

        [UnityTest]
        public IEnumerator UpdateRESPWaveform_ShouldSetRESPLabel()
        {
            //Act
            patientMonitor.UpdateRESPWaveform(50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(resp.text, "50");
        }

        [UnityTest]
        public IEnumerator UpdateSpO2Waveform_ShouldSetSpO2Label()
        {
            //Act
            patientMonitor.UpdateSpO2Waveform(50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(spO2.text, "50");
        }

        [UnityTest]
        public IEnumerator UpdateCO2Waveform_ShouldSetCO2Label()
        {
            //Act
            patientMonitor.UpdateCO2Waveform(50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(co2.text, "50");
        }

        [UnityTest]
        public IEnumerator UpdateBloodPressureWaveform_ShouldSetBloodPressureLabel()
        {
            //Act
            patientMonitor.UpdateBloodPressure(50, 50, 50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(bloodPressure.text, "50/50");
        }

        [UnityTest]
        public IEnumerator UpdateBloodPressureWaveform_ShouldSetPulseRateLabel()
        {
            //Act
            patientMonitor.UpdateBloodPressure(50, 50, 50);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(pulseRate.text, "(50)");
        }

        [UnityTest]
        public IEnumerator Update_LineInStartPosition_ShouldMoveLineRight()
        {
            //Arrange
            line.transform.localPosition = lineStart.localPosition;

            //Act
            yield return null; //Wait for one frame so line can move

            //Arrange
            Assert.IsTrue(line.transform.localPosition.x < lineStart.localPosition.x);
        }

        [UnityTest]
        public IEnumerator Update_LineInEndPosition_ShouldMoveLineToStartPosition()
        {
            //Arrange
            line.transform.localPosition = lineEnd.localPosition;

            //Act
            yield return null; //Wait for one frame so line can move

            //Arrange
            Assert.IsTrue(line.transform.localPosition == lineStart.localPosition);
        }
    }
}
