using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace LogisticsSystem
{
    public partial class MainForm : Form
    {
        private PictureBox mapPictureBox;
        private List<StationControl> stations;
        private StationControl centralDispatch;
        private System.Windows.Forms.Timer simulationTimer;
        private DateTime simulationStartTime;
        private List<Vehicle> vehicles;
        private Button startSimulationButton;
        private DateTimePicker simulationTimePicker;
        private Button manageButton;
        private Button randomPackagesButton;
        private Button pauseButton;
        private StationControl selectedStation;
        private bool isSimulationPaused = false;
        private bool isSimulationRunning = false;
        private DateTime currentSimulationTime;
        private const int SIMULATION_SPEED = 60; 
        private Label timeLabel;
        private Panel controlsPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Name = "MainForm";
            this.Text = "Logistický Systém";
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Set up the form
            this.Text = "Logistický Systém";
            this.Size = new Size(1200, 800);

            // Create control panel
            var controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            var timePickerLabel = new Label
            {
                Text = "Čas odjezdu:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            simulationTimePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                Location = new Point(100, 10),
                Width = 100
            };
            simulationTimePicker.ValueChanged += (s, e) => 
            {
                foreach (var vehicle in vehicles)
                {
                    vehicle.SetDepartureTime(simulationTimePicker.Value.TimeOfDay);
                }
            };

            timeLabel = new Label
            {
                Location = new Point(210, 10),
                Width = 150,
                Text = "00:00:00"
            };

            startSimulationButton = new Button
            {
                Text = "Spustit simulaci",
                Location = new Point(370, 10),
                Width = 100
            };
            startSimulationButton.Click += StartSimulationButton_Click;

            manageButton = new Button
            {
                Text = "Spravovat",
                Location = new Point(480, 10),
                Width = 100
            };
            manageButton.Click += ManageButton_Click;

            randomPackagesButton = new Button
            {
                Text = "Náhodné balíky",
                Location = new Point(590, 10),
                Width = 100
            };
            randomPackagesButton.Click += RandomPackagesButton_Click;

            pauseButton = new Button
            {
                Text = "Pauza",
                Location = new Point(700, 10),
                Width = 100,
                Enabled = false
            };
            pauseButton.Click += PauseButton_Click;

            controlPanel.Controls.Add(timePickerLabel);
            controlPanel.Controls.Add(simulationTimePicker);
            controlPanel.Controls.Add(timeLabel);
            controlPanel.Controls.Add(startSimulationButton);
            controlPanel.Controls.Add(manageButton);
            controlPanel.Controls.Add(randomPackagesButton);
            controlPanel.Controls.Add(pauseButton);
            this.Controls.Add(controlPanel);

            // Create map picture box
            mapPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            mapPictureBox.MouseClick += MapPictureBox_MouseClick;

            // Load map image
            try
            {
                mapPictureBox.Image = Image.FromFile("map.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání mapy: {ex.Message}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Add map to form
            this.Controls.Add(mapPictureBox);

            // Initialize collections
            stations = new List<StationControl>();
            vehicles = new List<Vehicle>();

            // Initialize simulation timer
            simulationTimer = new System.Windows.Forms.Timer
            {
                Interval = 50 // 50ms update interval for smoother animation
            };
            simulationTimer.Tick += SimulationTimer_Tick;

            // Create stations and vehicles
            CreateStations();
            CreateVehicles();

            // Add all station and vehicle panels directly to the map PictureBox
            foreach (var station in stations.Concat(new[] { centralDispatch }))
            {
                mapPictureBox.Controls.Add(station.stationPanel);
                station.Draw(null);
            }

            foreach (var vehicle in vehicles)
            {
                mapPictureBox.Controls.Add(vehicle.vehiclePanel);
                vehicle.Draw(null);
            }
        }

        private void CreateStations()
        {
            // Create central dispatch station
            centralDispatch = new StationControl("MAIN");
            centralDispatch.Location = new Point(550, 350);

            // Create other stations with their actual positions based on the map
            var stationData = new[]
            {
                ("PRG", new Point(415, 305)),
                ("Brno", new Point(780, 545)),
                ("OST", new Point(1060, 370)),
                ("PLZ", new Point(210, 390)),
                ("ČB", new Point(400, 600)),
                ("HK", new Point(645, 270)),
                ("LIB", new Point(520, 120)),
                ("OLO", new Point(900, 450)),
                ("Zlín", new Point(980, 540))
            };

            foreach (var (name, location) in stationData)
            {
                var station = new StationControl(name);
                station.Location = location;
                stations.Add(station);
            }
        }

        private void CreateVehicles()
        {
            vehicles.Clear();
            foreach (var station in stations)
            {
                var vehicle = new Vehicle(station, centralDispatch, vehicles);
                vehicles.Add(vehicle);
            }
        }

        private void MapPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if clicked on a station
            foreach (var station in stations.Concat(new[] { centralDispatch }))
            {
                var stationRect = new Rectangle(
                    station.Location.X - StationControl.CIRCLE_SIZE/2,
                    station.Location.Y - StationControl.CIRCLE_SIZE/2,
                    StationControl.CIRCLE_SIZE,
                    StationControl.CIRCLE_SIZE
                );

                if (stationRect.Contains(e.Location))
                {
                    selectedStation = station;
                    using (var form = new PackageManagementForm(station, stations))
                    {
                        form.ShowDialog();
                        mapPictureBox.Invalidate();
                    }
                    break;
                }
            }
        }

        private void StartSimulationButton_Click(object sender, EventArgs e)
        {
            if (!isSimulationRunning)
            {
                // Start simulation
                isSimulationRunning = true;
                isSimulationPaused = false;
                simulationStartTime = DateTime.Today;  // Start from midnight
                currentSimulationTime = simulationStartTime;
                startSimulationButton.Text = "Reset";
                startSimulationButton.BackColor = Color.Red;
                pauseButton.Enabled = true;
                pauseButton.Text = "Pauza";
                pauseButton.BackColor = SystemColors.Control;
                simulationTimer.Start();
            }
            else
            {
                ResetSimulation();
            }
        }

        private void ManageButton_Click(object sender, EventArgs e)
        {
            using (var form = new StationSelectionForm(stations))
            {
                if (form.ShowDialog() == DialogResult.OK && form.SelectedStation != null)
                {
                    using (var packageForm = new PackageManagementForm(form.SelectedStation, stations))
                    {
                        packageForm.ShowDialog();
                        mapPictureBox.Invalidate();
                    }
                }
            }
        }

        private void RandomPackagesButton_Click(object sender, EventArgs e)
        {
            var random = new Random();
            
            // Clear existing packages
            foreach (var station in stations.Concat(new[] { centralDispatch }))
            {
                station.ClearPackages();
            }

            // Add random packages to each station
            foreach (var sourceStation in stations)
            {
                // Random number of packages (1-5)
                int numPackages = random.Next(1, 6);
                
                for (int i = 0; i < numPackages; i++)
                {
                    // Random destination (excluding the source station)
                    var possibleDestinations = stations.Where(s => s != sourceStation).ToList();
                    var destination = possibleDestinations[random.Next(possibleDestinations.Count)];
                    
                    sourceStation.AddOutgoingPackage(destination.StationName);
                }
            }

            mapPictureBox.Invalidate();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (isSimulationRunning)
            {
                isSimulationPaused = !isSimulationPaused;
                pauseButton.Text = isSimulationPaused ? "Pokračovat" : "Pauza";
                pauseButton.BackColor = isSimulationPaused ? Color.Yellow : SystemColors.Control;
            }
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            if (!isSimulationPaused)  // Only update if not paused
            {
                currentSimulationTime = currentSimulationTime.AddSeconds(SIMULATION_SPEED);
                timeLabel.Text = currentSimulationTime.ToString("HH:mm:ss");

                // Update vehicle positions
                foreach (var vehicle in vehicles)
                {
                    vehicle.UpdatePosition(currentSimulationTime);
                }
            }
        }

        private void ResetSimulation()
        {
            isSimulationRunning = false;
            isSimulationPaused = false;
            startSimulationButton.Text = "Spustit simulaci";
            startSimulationButton.BackColor = SystemColors.Control;
            pauseButton.Text = "Pauza";
            pauseButton.BackColor = SystemColors.Control;
            pauseButton.Enabled = false;
            simulationTimer.Stop();

            // Reset time display
            currentSimulationTime = simulationStartTime;
            timeLabel.Text = currentSimulationTime.ToString("HH:mm:ss");

            // Clear all packages from all stations
            foreach (var station in stations.Concat(new[] { centralDispatch }))
            {
                station.ClearPackages();
            }

            // Reset all vehicles to their home stations
            foreach (var vehicle in vehicles)
            {
                vehicle.ResetToHome();
            }

            mapPictureBox.Invalidate();
        }
    }
} 