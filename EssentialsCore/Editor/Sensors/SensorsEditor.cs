using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Essentials.Internal.Sensors
{
    /// <summary>
    /// Custom editor for the Sensors component.
    /// </summary>
    [CustomEditor(typeof(Core.Sensors.Sensors))]
    [CanEditMultipleObjects]
    public class SensorsEditor : Editor
    {
        private Core.Sensors.Sensors _target;

        private PropertyField _scanMethodField;
        private PropertyField _sensorsOrientationField;
        private PropertyField _scanAngleAmplitudeField;
        private PropertyField _scanAngleFrequencyField;
        private PropertyField _randomScanTypeField;
        private PropertyField _verticalRandomizationField;
        private PropertyField _horizontalRandomizationField;

        private PropertyField _sensorsCountField;
        private PropertyField _sensorsRowsField;
        private PropertyField _sensorsAngleField;
        private PropertyField _sensorsHorizontalAngleField;
        private PropertyField _sensorsVerticalAngleField;

        private PropertyField _showSensorsField;
        private PropertyField _showSensorHitsField;

        /// <summary>
        /// Creates the custom inspector GUI for the Sensors component.
        /// </summary>
        /// <returns>The root VisualElement for the inspector.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            _target = (Core.Sensors.Sensors)target;

            VisualElement root = new VisualElement();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/Sensors/SensorsEditorDocument.uxml");
            visualTree.CloneTree(root);

            GetProperties(root);
            BindPropertyEvents();

            return root;
        }

        /// <summary>
        /// Gets references to the PropertyField elements in the UI.
        /// </summary>
        /// <param name="root">The root VisualElement of the inspector.</param>
        private void GetProperties(VisualElement root)
        {
            _scanMethodField = root.Q<PropertyField>("ScanMethodField");
            _sensorsOrientationField = root.Q<PropertyField>("SensorsOrientationField");
            _scanAngleAmplitudeField = root.Q<PropertyField>("ScanAngleAmplitudeField");
            _scanAngleFrequencyField = root.Q<PropertyField>("ScanAngleFrequencyField");
            _randomScanTypeField = root.Q<PropertyField>("RandomScanTypeField");
            _verticalRandomizationField = root.Q<PropertyField>("VerticalRandomizationField");
            _horizontalRandomizationField = root.Q<PropertyField>("HorizontalRandomizationField");

            _sensorsCountField = root.Q<PropertyField>("SensorsCountField");
            _sensorsRowsField = root.Q<PropertyField>("SensorsRowsField");
            _sensorsAngleField = root.Q<PropertyField>("SensorsAngleField");
            _sensorsHorizontalAngleField = root.Q<PropertyField>("SensorsHorizontalAngleField");
            _sensorsVerticalAngleField = root.Q<PropertyField>("SensorsVerticalAngleField");

            _showSensorsField = root.Q<PropertyField>("ShowSensorsField");
            _showSensorHitsField = root.Q<PropertyField>("ShowSensorHitsField");
        }

        /// <summary>
        /// Binds callback events to property changes.
        /// </summary>
        private void BindPropertyEvents()
        {
            _scanMethodField.RegisterValueChangeCallback(OnPropertyChanged);
            _sensorsOrientationField.RegisterValueChangeCallback(OnPropertyChanged);
            _randomScanTypeField.RegisterValueChangeCallback(OnPropertyChanged);
            _showSensorsField.RegisterValueChangeCallback(OnPropertyChanged);
        }

        /// <summary>
        /// Callback method invoked when a serialized property changes.
        /// Updates the visibility of related fields based on the changed property.
        /// </summary>
        /// <param name="evt">The property change event.</param>
        private void OnPropertyChanged(SerializedPropertyChangeEvent evt)
        {
            ShowField(_scanAngleAmplitudeField, _target.scanMethod is Core.Sensors.Sensors.ScanMethod.Vertical or Core.Sensors.Sensors.ScanMethod.Horizontal);
            ShowField(_scanAngleFrequencyField, _target.scanMethod is Core.Sensors.Sensors.ScanMethod.Vertical or Core.Sensors.Sensors.ScanMethod.Horizontal);
            ShowField(_randomScanTypeField, _target.scanMethod is Core.Sensors.Sensors.ScanMethod.Random);
            ShowField(_verticalRandomizationField, _target.scanMethod is Core.Sensors.Sensors.ScanMethod.Random && _target.randomScanType is Core.Sensors.Sensors.RandomScanType.Vertical or Core.Sensors.Sensors.RandomScanType.Both);
            ShowField(_horizontalRandomizationField, _target.scanMethod is Core.Sensors.Sensors.ScanMethod.Random && _target.randomScanType is Core.Sensors.Sensors.RandomScanType.Horizontal or Core.Sensors.Sensors.RandomScanType.Both);
            ShowField(_sensorsRowsField, _target.sensorsOrientation is Core.Sensors.Sensors.SensorsOrientation.Both);
            ShowField(_sensorsAngleField, _target.sensorsOrientation is not Core.Sensors.Sensors.SensorsOrientation.Both);
            ShowField(_sensorsHorizontalAngleField, _target.sensorsOrientation is Core.Sensors.Sensors.SensorsOrientation.Both);
            ShowField(_sensorsVerticalAngleField, _target.sensorsOrientation is Core.Sensors.Sensors.SensorsOrientation.Both);
            ShowField(_showSensorHitsField, _target.showSensors);
        }

        /// <summary>
        /// Shows or hides a PropertyField based on a boolean condition.
        /// </summary>
        /// <param name="field">The PropertyField to show or hide.</param>
        /// <param name="show">True to show the field, false to hide it.</param>
        private void ShowField(PropertyField field, bool show) => field.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
