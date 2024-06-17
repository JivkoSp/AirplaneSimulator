# AirplaneSimulator

**AirplaneSimulator** simulates the lifecycle of airfields and planes. It is a console application designed to provide a simplified view of airfields, dispatcher decision-making, and their impact on planes. Users can also influence plane speeds at specific airfields to observe how it affects the simulation.

## 1. How the app looks

### Short view of plane movement and overall look of the app:

![Short view](https://user-images.githubusercontent.com/97282923/185492810-b2045db1-3737-47f3-8f08-9754d5a91fcd.mp4)

### Airfield TakeOff color:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185493440-bff55f3f-b76a-4e77-95ac-af7d27512b70.png" alt="TakeOff" width="100">
</div>

### Airfield Landing color:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185493765-ff88be98-5fc4-4c0d-bc82-482d8c6932ce.png" alt="Landing" width="100">
</div>

### Airfield Re-Routing color:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185493646-29ffa751-5e16-45b4-9ba5-e0ef0adfacc7.png" alt="Re-Routing" width="100">
</div>

### Airfield crash color:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185494159-6af2c49c-2d05-4788-b2a7-191bd90bc0aa.png" alt="Crash" width="100">
</div>

### Pause simulation:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185498047-fc9464c6-2245-4057-813e-9870565f00f5.png" alt="Pause" width="100">
</div>

### User interface:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185498261-7582f1af-a301-4b53-a4b6-5a9fb7437fec.png" alt="User Interface" width="500">
</div>

### User interface - change plane speed of certain airfield:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185498355-f6835a0c-c9b0-4baf-8577-aa05e2ccf41c.png" alt="Change Plane Speed" width="500">
</div>

### User interface - change plane speed of certain airfield - select option:
<div style="text-align: center;">
  <img src="https://user-images.githubusercontent.com/97282923/185498619-838f55bc-50a1-4ad5-a651-bcdf83be5340.png" alt="Select Option" width="500">
</div>

## 2. How the app works

To allow multiple objects to perform different tasks simultaneously, the app uses the Task Parallel Library. Here are the main components of the program:

1. **Airfield class:** Manages airfield behavior.
2. **Abstract Plane class:** Includes concrete plane child classes for flying and landing.
3. **Airfield Dispatcher class:** Makes decisions for each airfield.
4. **CollisionScanner class:** Detects various types of collisions.
5. **Map class:** Creates, displays, and places airfield objects on the map; finds flying paths.
6. **Simulator class:** Selects airfields at random to initiate takeoff; handles simulation data display.
7. **SimulatorData class:** Displays info during pause, real-time data, and user interface.
8. **MinHeap class:** Implements the min-heap data structure for sorting planes by maintenance time.

When the simulation starts, the Simulator class randomly selects an airfield and invokes its TakeOff method. Here's a brief overview:

- **TakeOff method:** Chooses the plane with the smallest maintenance time from the min-heap, selects a random target, and uses the Map's FindPath method to find the flight path.
- **Dispatcher's TakeOff method:** Determines if the plane can take off based on airspace conditions and runway availability.
- **Observer pattern:** Notifies the target airfield's dispatcher when a plane is near using the OnLanding event.

### Collision detection

- **Sphere collision:** Detects plane collisions using distance calculations.
- **Sphere to rectangle collision:** Detects plane to airfield collisions.
- **Predict collision:** Anticipates collisions using larger detection radii.

### Colors

Each airfield is color-coded based on dispatcher decisions:

- Red: Plane is taking off.
- Blue: Plane is landing.
- Dark: Plane has crashed.
- Dark green: Permission denied, alternative route found.
- Blue dot: Public planes.
- Green dot: Cargo planes.
- Red dot: Plane has less than 10% fuel.
















