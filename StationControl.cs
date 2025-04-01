using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LogisticsSystem
{
    public class StationControl
    {
        private string stationName;
        private List<Package> outgoingPackages;
        private List<Package> incomingPackages;
        private int totalReceivedPackages;
        public static readonly int CIRCLE_SIZE = 30;
        private const int FONT_SIZE = 9;
        private const int COUNTER_SIZE = 20;
        public Point Location { get; set; }
        public Panel stationPanel { get; private set; }
        private Label nameLabel;
        private Label outgoingLabel;
        private Label incomingLabel;
        private Label totalLabel;

        public StationControl(string name)
        {
            stationName = name;
            outgoingPackages = new List<Package>();
            incomingPackages = new List<Package>();
            totalReceivedPackages = 0;
            InitializeControls();
        }

        private void InitializeControls()
        {
            int totalSize = CIRCLE_SIZE * 2; 

            stationPanel = new Panel
            {
                Width = totalSize,
                Height = totalSize,
                BackColor = Color.Red,
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, totalSize + 1, totalSize + 1, totalSize, totalSize))
            };

            // Station name label
            nameLabel = new Label
            {
                Text = stationName,
                Font = new Font("Arial", FONT_SIZE, FontStyle.Bold),
                AutoSize = false,
                Width = totalSize - 4,  
                Height = FONT_SIZE + 4,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                Location = new Point(2, totalSize/2 - (FONT_SIZE + 4)/2)  
            };

            outgoingLabel = new Label
            {
                Width = COUNTER_SIZE,
                Height = COUNTER_SIZE,
                BackColor = Color.LightGreen,
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, COUNTER_SIZE + 1, COUNTER_SIZE + 1, COUNTER_SIZE, COUNTER_SIZE)),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", FONT_SIZE - 2),
                Location = new Point(2, 2)  
            };

            incomingLabel = new Label
            {
                Width = COUNTER_SIZE,
                Height = COUNTER_SIZE,
                BackColor = Color.LightBlue,
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, COUNTER_SIZE + 1, COUNTER_SIZE + 1, COUNTER_SIZE, COUNTER_SIZE)),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", FONT_SIZE - 2),
                Location = new Point(totalSize - COUNTER_SIZE - 2, 2)  
            };

            totalLabel = new Label
            {
                Width = COUNTER_SIZE,
                Height = COUNTER_SIZE,
                BackColor = Color.LightYellow,
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, COUNTER_SIZE + 1, COUNTER_SIZE + 1, COUNTER_SIZE, COUNTER_SIZE)),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", FONT_SIZE - 2),
                Location = new Point(totalSize/2 - COUNTER_SIZE/2, totalSize - COUNTER_SIZE - 2)  
            };

            stationPanel.Controls.Add(nameLabel);
            stationPanel.Controls.Add(outgoingLabel);
            stationPanel.Controls.Add(incomingLabel);
            stationPanel.Controls.Add(totalLabel);
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public void Draw(Graphics g)
        {
            stationPanel.Location = new Point(
                Location.X - (stationPanel.Width / 2),
                Location.Y - (stationPanel.Width / 2)
            );
        }

        public void UpdateCounters()
        {
            var outgoingCount = outgoingPackages.Count(p => !p.IsDelivered);
            outgoingLabel.Text = outgoingCount > 0 ? outgoingCount.ToString() : "";
            outgoingLabel.Visible = outgoingCount > 0;

            var incomingCount = incomingPackages.Count;
            incomingLabel.Text = incomingCount > 0 ? incomingCount.ToString() : "";
            incomingLabel.Visible = incomingCount > 0;

            totalLabel.Text = totalReceivedPackages > 0 ? totalReceivedPackages.ToString() : "";
            totalLabel.Visible = totalReceivedPackages > 0;
        }

        public void AddOutgoingPackage(string destinationStation)
        {
            outgoingPackages.Add(new Package(stationName, destinationStation));
            UpdateCounters();  
        }

        public void AddIncomingPackage(Package package)
        {
            incomingPackages.Add(package);
            totalReceivedPackages++;  
            UpdateCounters();  
        }

        public List<Package> CollectOutgoingPackages()
        {
            var packages = outgoingPackages.Where(p => !p.IsDelivered).ToList();
            foreach (var package in packages)
            {
                package.IsDelivered = true;
            }
            UpdateCounters(); 
            return packages;
        }

        public string StationName => stationName;
        public int PackagesToPickup => outgoingPackages.Count(p => !p.IsDelivered);
        public int PackagesToDeliver => incomingPackages.Count;
        public int Width => CIRCLE_SIZE;
        public int Height => CIRCLE_SIZE;

        public List<Package> GetOutgoingPackages()
        {
            return outgoingPackages;
        }

        public List<Package> GetIncomingPackages()
        {
            return incomingPackages;
        }

        public void ClearPackages()
        {
            outgoingPackages.Clear();
            incomingPackages.Clear();
            totalReceivedPackages = 0;  
            UpdateCounters();  
        }

        public void RemovePackages(List<Package> packagesToRemove)
        {
            outgoingPackages.RemoveAll(p => packagesToRemove.Contains(p));
            incomingPackages.RemoveAll(p => packagesToRemove.Contains(p));
            UpdateCounters();  
        }

        public void RemoveIncomingPackage(Package package)
        {
            if (incomingPackages.Remove(package))
            {
                UpdateCounters();  
            }
        }
    }
} 