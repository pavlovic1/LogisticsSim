using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LogisticsSystem
{
    public class StationSelectionForm : Form
    {
        private ListBox stationList;
        private Button selectButton;
        private Button cancelButton;
        public StationControl SelectedStation { get; private set; }

        public StationSelectionForm(List<StationControl> stations)
        {
            this.Text = "Vyberte stanici";
            this.Size = new Size(300, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create station list
            stationList = new ListBox
            {
                Location = new Point(10, 10),
                Size = new Size(265, 300)
            };

            // Add stations to list
            foreach (var station in stations)
            {
                stationList.Items.Add(station);
            }
            stationList.DisplayMember = "StationName";

            // Create buttons
            selectButton = new Button
            {
                Text = "Vybrat",
                DialogResult = DialogResult.OK,
                Location = new Point(10, 320),
                Size = new Size(100, 30)
            };
            selectButton.Click += SelectButton_Click;

            cancelButton = new Button
            {
                Text = "Zrušit",
                DialogResult = DialogResult.Cancel,
                Location = new Point(175, 320),
                Size = new Size(100, 30)
            };

            this.Controls.AddRange(new Control[] { stationList, selectButton, cancelButton });
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if (stationList.SelectedItem != null)
            {
                SelectedStation = (StationControl)stationList.SelectedItem;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Prosím vyberte stanici.", "Upozornění", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
} 