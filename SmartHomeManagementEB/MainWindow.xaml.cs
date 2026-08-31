using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SmartHomeManagementEB
{
    public partial class MainWindow : Window
    {
        // the actual storage — everything in the UI is just a view onto this
        private readonly CustomDictionary<string, SmartDevice> _devices = new();

        // bound to the DataGrid; refreshed from _devices after every change
        private readonly ObservableCollection<SmartDevice> _deviceView = new();

        public MainWindow()
        {
            InitializeComponent();
            DeviceGrid.ItemsSource = _deviceView;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "";

            if (!TryReadInputs(out string id, out string name, out string type, out string status))
                return;

            if (_devices.ContainsKey(id))
            {
                StatusMessage.Text = $"A device with ID '{id}' already exists. Use Update instead.";
                return;
            }

            _devices.Add(id, new SmartDevice(id, name, type, status));
            RefreshGrid();
            ClearInputs();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "";

            if (!TryReadInputs(out string id, out string name, out string type, out string status))
                return;

            if (!_devices.ContainsKey(id))
            {
                StatusMessage.Text = $"No device with ID '{id}' exists yet. Use Add instead.";
                return;
            }

            _devices.Update(id, new SmartDevice(id, name, type, status));
            RefreshGrid();
            ClearInputs();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "";

            string id = DeviceIdBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage.Text = "Enter a Device ID to remove.";
                return;
            }

            if (!_devices.Remove(id))
            {
                StatusMessage.Text = $"No device with ID '{id}' was found.";
                return;
            }

            RefreshGrid();
            ClearInputs();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "";
            ClearInputs();
        }

        // populate the input fields when a row is clicked, so Update/Remove
        // is a one-click round trip instead of retyping the ID
        private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceGrid.SelectedItem is not SmartDevice selected) return;

            DeviceIdBox.Text = selected.DeviceId;
            NameBox.Text = selected.Name;
            SetComboBoxValue(TypeBox, selected.Type);
            SetComboBoxValue(StatusBox, selected.Status);
        }

        private bool TryReadInputs(out string id, out string name, out string type, out string status)
        {
            id = DeviceIdBox.Text.Trim();
            name = NameBox.Text.Trim();
            type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            status = (StatusBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(status))
            {
                StatusMessage.Text = "Please fill in Device ID, Name, Type, and Status.";
                return false;
            }

            return true;
        }

        private void RefreshGrid()
        {
            _deviceView.Clear();
            foreach (var kvp in _devices.GetAll())
            {
                _deviceView.Add(kvp.Value);
            }
        }

        private void ClearInputs()
        {
            DeviceIdBox.Text = "";
            NameBox.Text = "";
            TypeBox.SelectedItem = null;
            StatusBox.SelectedItem = null;
        }

        private static void SetComboBoxValue(ComboBox box, string value)
        {
            foreach (ComboBoxItem item in box.Items)
            {
                if (item.Content?.ToString() == value)
                {
                    box.SelectedItem = item;
                    return;
                }
            }
        }
    }
}