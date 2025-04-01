# Logistics System

A Windows Forms application simulating a logistics network with multiple delivery stations and vehicles. The system allows for managing package deliveries between different stations in the Czech Republic.

## Features

- **Interactive Map Interface**: Visual representation of delivery stations across the Czech Republic
- **Real-time Package Management**: Add, track, and manage packages between stations
- **Vehicle Simulation**: Automated vehicles that transport packages between stations
- **Station Management**: Individual control over each station's outgoing and incoming packages
- **Time-based Departures**: Set specific departure times for vehicles
- **Package Tracking**: Monitor outgoing, incoming, and total packages at each station

## System Components

### Stations
- **Central Dispatch**: Main hub for package distribution
- **Regional Stations**: 
  - Praha
  - Brno
  - Ostrava
  - Plzeň
  - České Budějovice
  - Hradec Králové
  - Liberec
  - Olomouc
  - Zlín

### Vehicle System
- Each station has its own dedicated vehicle
- Vehicles automatically:
  - Move to central dispatch when needed
  - Wait for loading (30 seconds)
  - Wait for packages in transit
  - Return to their home stations
  - Handle package collection and delivery

## User Interface

### Main Controls
- **Start/Reset Simulation**: Controls the simulation state
- **Departure Time Picker**: Set when vehicles should start moving
- **Pause Button**: Temporarily pause the simulation
- **Manage Button**: Access station management
- **Random Packages**: Generate random package distribution

### Station Display
Each station shows:
- Station name
- Outgoing packages (green circle)
- Incoming packages (blue circle)
- Total received packages (yellow circle)

### Package Management
- Add outgoing packages to any station
- Track package status
- Remove delivered packages
- View package statistics

## Technical Details

### Requirements
- Windows operating system
- .NET 6.0 or later
- Visual Studio 2022 (recommended)

### Dependencies
- Windows Forms
- System.Drawing
- System.Windows.Forms

### Key Classes
- `MainForm`: Main application window and simulation control
- `StationControl`: Manages individual station display and package handling
- `Vehicle`: Handles vehicle movement and package transportation
- `Package`: Represents individual packages in the system

## Usage

1. **Starting the Application**
   - Run the application
   - The map will display with all stations and vehicles

2. **Managing Packages**
   - Click on any station to open its management window
   - Add outgoing packages by selecting destination
   - View and manage incoming packages

3. **Running the Simulation**
   - Set desired departure time for vehicles
   - Click "Start Simulation" to begin
   - Use "Pause" to temporarily stop the simulation
   - Click "Reset" to return vehicles to their stations

4. **Random Package Generation**
   - Click "Random Packages" to generate a random distribution
   - Each station will receive 1-5 random outgoing packages

## Notes

- The simulation runs at 60x real-time speed
- Vehicles wait at central dispatch for 30 seconds to load
- Vehicles will wait for packages in transit before leaving central dispatch
- The system maintains a count of total packages ever received at each station 