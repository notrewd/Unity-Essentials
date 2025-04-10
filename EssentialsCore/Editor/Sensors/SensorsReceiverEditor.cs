using Essentials.Core.Sensors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Essentials.Internal.Sensors
{
    [CustomEditor(typeof(SensorsReceiver))]
    [CanEditMultipleObjects]
    public class SensorsReceiverEditor : Editor
    {
        private SensorsReceiver _target;

        private PropertyField _callbackTypeField;
        private VisualElement _eventsCategory;
        private PropertyField _isDetectedField;

        public override VisualElement CreateInspectorGUI()
        {
            _target = (SensorsReceiver)target;

            VisualElement root = new VisualElement();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Sensors/SensorsReceiverEditorDocument.uxml");
            visualTree.CloneTree(root);

            GetProperties(root);
            BindPropertyEvents();

            _isDetectedField.SetEnabled(false);

            return root;
        }

        private void GetProperties(VisualElement root)
        {
            _callbackTypeField = root.Q<PropertyField>("CallbackTypeField");
            _eventsCategory = root.Q<VisualElement>("EventsCategory");
            _isDetectedField = root.Q<PropertyField>("IsDetectedField");
        }

        private void BindPropertyEvents()
        {
            _callbackTypeField.RegisterValueChangeCallback(OnPropertyChanged);
        }

        private void OnPropertyChanged(SerializedPropertyChangeEvent evt)
        {
            _eventsCategory.style.display = _target.callbackType == SensorsReceiver.CallbackType.DisableRenderer ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}