using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using VARLab.Velcro;

namespace Tests.PlayMode
{
    public class NotificationIntegrationTests
    {
        private int sceneCounter = 0;

        private GameObject notificationObj;
        private UIDocument notificationDoc;
        private Notification Notification;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "NotificationScene");

            //Set up <Notification> UI
            notificationObj = new GameObject("Notification");
            notificationDoc = notificationObj.AddComponent<UIDocument>();
            Notification = notificationObj.AddComponent<Notification>();

            //Load required assets from project files
            VisualTreeAsset notificationUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Notifications/Notification.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");

            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(notificationDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = notificationUXML;
            so.ApplyModifiedProperties();

            notificationUXML.CloneTree(notificationDoc.rootVisualElement);
            yield return null;
        }

        [Test, Order(1)]
        [Category("BuildServer")]
        public void Start_SetsRootToDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Assert
            Assert.AreEqual(expectedStyle, notificationDoc.rootVisualElement.style.display);
        }

        [Test, Order(2)]
        [Category("BuildServer")]
        public void Show_SetsDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            Notification.Show();

            //Assert
            Assert.AreEqual(expectedStyle, notificationDoc.rootVisualElement.style.display);
        }

        [Test, Order(3)]
        [Category("BuildServer")]
        public void Hide_SetsDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            Notification.Show();
            Notification.Hide();

            //Assert
            Assert.AreEqual(expectedStyle, notificationDoc.rootVisualElement.style.display);
        }
    }
}
