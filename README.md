# AirplaneSimulator

AirplaneSimulator is simulation of the lifetime of airfields and planes.
It is console app with the purpose of viewing simplified version of airfields, how dispathers take certain desicions 
and how this impacts the planes. 
All of this affect how the simulation will play out. 
In addition the user can impact the speed of the planes of certain airfield and see how thoes this affect the simulation.

1. How the app looks:

Short view of plane movement and overall look of the app:
https://user-images.githubusercontent.com/97282923/185492810-b2045db1-3737-47f3-8f08-9754d5a91fcd.mp4

Airfield TakeOff color:
![image](https://user-images.githubusercontent.com/97282923/185493440-bff55f3f-b76a-4e77-95ac-af7d27512b70.png)

Airfield Landing color:
![image](https://user-images.githubusercontent.com/97282923/185493765-ff88be98-5fc4-4c0d-bc82-482d8c6932ce.png)

Airfield Re-Routing color:
![image](https://user-images.githubusercontent.com/97282923/185493646-29ffa751-5e16-45b4-9ba5-e0ef0adfacc7.png)

Airfield crash color:
![image](https://user-images.githubusercontent.com/97282923/185494159-6af2c49c-2d05-4788-b2a7-191bd90bc0aa.png)

Pause simulation:
![image](https://user-images.githubusercontent.com/97282923/185498047-fc9464c6-2245-4057-813e-9870565f00f5.png)

User interface:
![image](https://user-images.githubusercontent.com/97282923/185498261-7582f1af-a301-4b53-a4b6-5a9fb7437fec.png)

User interface - change plane speed of certain airfield:
![image](https://user-images.githubusercontent.com/97282923/185498355-f6835a0c-c9b0-4baf-8577-aa05e2ccf41c.png)

User interface - change plane speed of certain airfield - select option:
![image](https://user-images.githubusercontent.com/97282923/185498619-838f55bc-50a1-4ad5-a651-bcdf83be5340.png)

2. How the app works:

To be able to have multiple objects doing different stuff at the same time independently of each other i have used task parallel library.
The program is divided in several parts namely:

1. Airfield class responsible for airfield behaviour
2. Abstract Plane class with conrete plane child classes responsible for plane flying and landing
3. Airfield Dispatcher class responsible for decision making for each airfield
4. CollisionScanner class responsible for detecting different kind of collisions
5. Map class responsible for creating, displaing, placing airfield objects on the map and finding flying paths on the map (everything that concerns the map)
6. Simulator class responsible for choosing airfield between some range of time, finding all neighbours for each airfield (used for re-routing) before simulation begins
   and runing SimulatorData class for displaing different data about the simulation
7. SimulatorData class used for displaing info when paused, real time info and user interface
8. MinHeap class used for implementing min-heap data structure with purpose of storing plane object in airfield sorted by maintenance time

When the simulation begins the Simulator class chooses random airfield and invokes its TakeOff method.
In the Airfield's TakeOff method the first plane of min-heap is choosen (the plane with the smallest maintenance time), then
random target is choosen and Map's FindPath method is invoked. This method searches the target X,Y coordinates 
between X-width/2 X+width/2 and Y-height/2 Y+height/2 (the airfield target) with bfs. 
Then Dispacher's TakeOff method is invoked to decide if the plane can take off or not. If there is no plane in airfield airspace 
and the track is not in use in the moment permission is given and the plane flies off.
When given plane is near the target airfield (in the simulation "near" means steps-4 that is => if target is 10 steps from point A then 
at point 6 message is send to the dispatcher at target airfield).

Patterns used
-------------
Observer pattern => If Dispatcher has given permission for flying before that the target's dispather subscribes to the plane's OnLanding event.
So when airfield is "near" to the target this event is invoked and in turn the dispather is notified.


If the airfield has no other planes in its airspace, the track is clear, there is space for this plane, the airfield can take this kind of plane
and the plane is not late the permission is given to the plane. If eny of the above is not true then alternative route will be searched.
For this purpose before the simulation begins all airfield neigbours are searched for each airfield and their paths are saved.
The path searching begins with the closest neighbours first. If the plane has fuel to given neigbour the plane is re-routed
else if all neighbours are searched the plane crashes.

Collision detection
-------------------
Sphere collision is used for plane detection => dist = sqr(dx^2+dy^2) if dist < r1+r2 - collision
Spere to rectangle is used for plane to airfield collision => it is the same formula as the above except for dx and dy 
insead of x1-x2 and y1-y2 the sphere i.e the plane need to know which side of the airfield is on, that is for left/right 
=> dx - X < targetX-Width/2 or X > targetX+width/2 for dy is the same with y.
Predict collision same as the sphere collision except the radious is bigger (to detect earlier)

Colors
-------
Each airfield have different colors depending on dispatcher decisions:
1. Red if plane is flying off
2. Blue if plane is landing
3. Dark if plane has crashed
4. Dark green if plane has not given permition but alternative route is found
5. Blue dot - public planes
6. Green dot - gargo planes
7. Red dot - plane has < 10% fuel in the tank

















