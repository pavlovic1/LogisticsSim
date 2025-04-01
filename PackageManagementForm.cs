using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace LogisticsSystem
{
    public class PackageManagementForm : Form
    {
        private StationControl station;
        private ComboBox destinationComboBox;
        private ListBox outgoingListBox;
        private ListBox incomingListBox;
        private List<StationControl> stations;

        public PackageManagementForm(StationControl station, List<StationControl> allStations)
        {
            this.station = station;
            this.stations = allStations;
            this.Text = $"Správa balíků - {station.StationName}";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = $"Správa balíků - {station.StationName}";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var destinationLabel = new Label
            {
                Text = "Cílová stanice:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            destinationComboBox = new ComboBox
            {
                Location = new Point(10, 30),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            foreach (var s in stations)
            {
                if (s != station)
                {
                    destinationComboBox.Items.Add(s.StationName);
                }
            }

            var addButton = new Button
            {
                Text = "Přidat balík",
                Location = new Point(220, 28),
                Width = 100
            };
            addButton.Click += AddButton_Click;

            var outgoingLabel = new Label
            {
                Text = "Odchozí balíky:",
                Location = new Point(10, 70),
                AutoSize = true
            };

            outgoingListBox = new ListBox
            {
                Location = new Point(10, 90),
                Width = 360,
                Height = 150
            };

            var incomingLabel = new Label
            {
                Text = "Příchozí balíky:",
                Location = new Point(10, 260),
                AutoSize = true
            };

            incomingListBox = new ListBox
            {
                Location = new Point(10, 280),
                Width = 360,
                Height = 150
            };

            var retrieveButton = new Button
            {
                Text = "Vyzvednout balík",
                Location = new Point(10, 440),
                Width = 120
            };
            retrieveButton.Click += RetrieveButton_Click;

            var saveButton = new Button
            {
                Text = "Uložit",
                Location = new Point(250, 440),
                Width = 120
            };
            saveButton.Click += SaveButton_Click;

            this.Controls.AddRange(new Control[] {
                destinationLabel, destinationComboBox, addButton,
                outgoingLabel, outgoingListBox,
                incomingLabel, incomingListBox,
                retrieveButton, saveButton
            });

            LoadPackages();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (destinationComboBox.SelectedItem != null)
            {
                station.AddOutgoingPackage(destinationComboBox.SelectedItem.ToString());
                LoadPackages();
            }
        }

        private void RetrieveButton_Click(object sender, EventArgs e)
        {
            if (incomingListBox.SelectedItem is Package selectedPackage)
            {
                station.RemoveIncomingPackage(selectedPackage);
                LoadPackages(); 
            }
        }

        private void LoadPackages()
        {
            outgoingListBox.Items.Clear();
            incomingListBox.Items.Clear();

            outgoingListBox.DisplayMember = null;
            incomingListBox.DisplayMember = null;

            foreach (var package in station.GetOutgoingPackages())
            {
                outgoingListBox.Items.Add(package);
            }

            foreach (var package in station.GetIncomingPackages())
            {
                incomingListBox.Items.Add(package);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Implementation of SaveButton_Click method
        }
    }
} 