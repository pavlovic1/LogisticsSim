using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace LogisticsSystem
{
    public class Vehicle
    {
        private StationControl homeStation;
        private StationControl centralDispatch;
        private PointF currentPosition;
        private Point targetPosition;
        private const int VEHICLE_SIZE = 10;
        private const double SPEED = 2.0; 
        private bool isMovingToCentral;
        private bool isWaitingForLoading;
        private DateTime arrivalTime;
        private const int LOADING_TIME_SECONDS = 30;
        private bool isMoving;
        private List<Package> carriedPackages;
        private List<Vehicle> allVehicles; 
        private TimeSpan departureTime; 
        private bool isWaitingForDepartureTime = false;
        public Panel vehiclePanel { get; private set; }

        public Vehicle(StationControl homeStation, StationControl centralDispatch, List<Vehicle> allVehicles)
        {
            this.homeStation = homeStation;
            this.centralDispatch = centralDispatch;
            this.allVehicles = allVehicles;
            this.currentPosition = new PointF(
                homeStation.Location.X,
                homeStation.Location.Y
            );
            this.targetPosition = new Point(
                (int)currentPosition.X,
                (int)currentPosition.Y
            );
            this.isMovingToCentral = false;
            this.isWaitingForLoading = false;
            this.isMoving = false;
            this.carriedPackages = new List<Package>();
            this.departureTime = TimeSpan.Zero;  
            InitializeControls();
        }

        private void InitializeControls()
        {
            vehiclePanel = new Panel
            {
                Width = VEHICLE_SIZE,
                Height = VEHICLE_SIZE,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        public void SetDepartureTime(TimeSpan time)
        {
            departureTime = time;
            isWaitingForDepartureTime = true;
        }

        public void UpdatePosition(DateTime currentTime)
        {
            if (isWaitingForDepartureTime)
            {
                TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
                
                if (currentTimeOfDay >= departureTime)
                {
                    isWaitingForDepartureTime = false;
                }
                else
                {
                    return; 
                }
            }

            if (!isMoving && !isWaitingForLoading)
            {
                if (homeStation.PackagesToPickup > 0 || HasIncomingPackagesAtCentral())
                {
                    StartMovingToCentral();
                }
            }
            else if (isMoving)
            {
                MoveTowardsTarget();
                if (HasReachedTarget())
                {
                    if (isMovingToCentral)
                    {
                        ArriveAtCentral();
                    }
                    else
                    {
                        ArriveAtHome();
                    }
                }
            }
            else if (isWaitingForLoading)
            {
                if (ArePackagesInTransit())
                {
                    // Reset the arrival time to keep waiting
                    arrivalTime = DateTime.Now;
                }
                else if ((DateTime.Now - arrivalTime).TotalSeconds >= LOADING_TIME_SECONDS)
                {
                    StartMovingHome();
                }
            }

            // Update vehicle panel position
            vehiclePanel.Location = new Point(
                (int)currentPosition.X - VEHICLE_SIZE/2,
                (int)currentPosition.Y - VEHICLE_SIZE/2
            );
        }

        private bool HasIncomingPackagesAtCentral()
        {
            return centralDispatch.GetIncomingPackages()
                .Any(p => p.DestinationStation == homeStation.StationName);
        }

        private bool ArePackagesInTransit()
        {
            // Get all packages destined for this vehicle's home station
            var packagesForThisStation = centralDispatch.GetIncomingPackages()
                .Where(p => p.DestinationStation == homeStation.StationName)
                .ToList();

            // Check if any of these packages are being transported by other vehicles
            foreach (var vehicle in allVehicles)
            {
                if (vehicle != this && vehicle.carriedPackages.Any(p => p.DestinationStation == homeStation.StationName))
                {
                    return true;
                }
            }

            return false;
        }

        private void StartMovingToCentral()
        {
            isMovingToCentral = true;
            isMoving = true;
            isWaitingForLoading = false;
            targetPosition = new Point(
                centralDispatch.Location.X,
                centralDispatch.Location.Y
            );
            carriedPackages = homeStation.CollectOutgoingPackages();
        }

        private void StartMovingHome()
        {
            isMovingToCentral = false;
            isMoving = true;
            isWaitingForLoading = false;
            targetPosition = new Point(
                homeStation.Location.X,
                homeStation.Location.Y
            );
        }

        private void ArriveAtCentral()
        {
            isMovingToCentral = true;
            isMoving = false;
            isWaitingForLoading = true;
            arrivalTime = DateTime.Now;

            foreach (var package in carriedPackages)
            {
                centralDispatch.AddIncomingPackage(package);
            }
            carriedPackages.Clear();
        }

        private void ArriveAtHome()
        {
            isMovingToCentral = false;
            isMoving = false;
            isWaitingForLoading = false;

            // Get packages for this station from central dispatch
            var packagesForThisStation = centralDispatch.GetIncomingPackages()
                .FindAll(p => p.DestinationStation == homeStation.StationName);

            if (packagesForThisStation.Any())
            {
                // Add packages to home station
                foreach (var package in packagesForThisStation)
                {
                    homeStation.AddIncomingPackage(package);
                }

                // Remove the packages from central dispatch
                centralDispatch.RemovePackages(packagesForThisStation);
            }
        }

        private void MoveTowardsTarget()
        {
            double dx = targetPosition.X - currentPosition.X;
            double dy = targetPosition.Y - currentPosition.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > SPEED)
            {
                // Calculate the unit vector (direction) and multiply by speed
                double unitX = dx / distance;
                double unitY = dy / distance;
                
                currentPosition.X += (float)(unitX * SPEED);
                currentPosition.Y += (float)(unitY * SPEED);
            }
            else
            {
                currentPosition = new PointF(targetPosition.X, targetPosition.Y);
            }
        }

        private bool HasReachedTarget()
        {
            return Math.Abs(currentPosition.X - targetPosition.X) < 1 &&
                   Math.Abs(currentPosition.Y - targetPosition.Y) < 1;
        }

        public void Draw(Graphics g)
        {
            vehiclePanel.Location = new Point(
                (int)currentPosition.X - VEHICLE_SIZE/2,
                (int)currentPosition.Y - VEHICLE_SIZE/2
            );
        }

        public void ResetToHome()
        {
            // Reset position to home station
            currentPosition = new PointF(
                homeStation.Location.X,
                homeStation.Location.Y
            );
            targetPosition = new Point(
                (int)currentPosition.X,
                (int)currentPosition.Y
            );

            // Reset all state flags
            isMovingToCentral = false;
            isWaitingForLoading = false;
            isMoving = false;
            isWaitingForDepartureTime = false;
            departureTime = TimeSpan.Zero;
            arrivalTime = DateTime.MinValue;

            carriedPackages.Clear();

            // Update vehicle panel position
            Draw(null);
        }
    }
} 