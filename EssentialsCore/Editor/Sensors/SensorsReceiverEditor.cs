using Essentials.Core.Sensors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Essentials.Internal.Sensors
{
    /// <summary>
    /// Custom editor for the SensorsReceiver component.
    /// </summary>
    [CustomEditor(typeof(SensorsReceiver))]
    [CanEditMultipleObjects]
    public class SensorsReceiverEditor : Editor
    {
        private SensorsReceiver _target;

        private PropertyField _callbackTypeField;
        private VisualElement _eventsCategory;
        private PropertyField _isDetectedField;

        /// <summary>
        /// Creates the custom inspector GUI for the SensorsReceiver component.
        /// </summary>
        /// <returns>The root VisualElement for the inspector.</returns>
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

        /// <summary>
        /// Gets references to the relevant UI elements.
        /// </summary>
        /// <param name="root">The root VisualElement of the inspector.</param>
        private void GetProperties(VisualElement root)
        {
            _callbackTypeField = root.Q<PropertyField>("CallbackTypeField");
            _eventsCategory = root.Q<VisualElement>("EventsCategory");
            _isDetectedField = root.Q<PropertyField>("IsDetectedField");
        }

        /// <summary>
        /// Binds callback events to property changes.
        /// </summary>
        private void BindPropertyEvents()
        {
            _callbackTypeField.RegisterValueChangeCallback(OnPropertyChanged);
        }

        /// <summary>
        /// Callback method invoked when a serialized property changes.
        /// Updates the visibility of the events category based on the callback type.
        /// </summary>
        /// <param name="evt">The property change event.</param>
        private void OnPropertyChanged(SerializedPropertyChangeEvent evt)
        {
            _eventsCategory.style.display = _target.callbackType == SensorsReceiver.CallbackType.DisableRenderer ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}