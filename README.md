##Dynamic Event Scheduler repository

A planner and reminder system that is aware of and responds dynamically to the user's current location and time of day.

###Design

The system includes two primary components: the Mobile Client App and the Backend Cloud service. The app is responsible for all interactions with the user, and will run on a Windows Phone. User interactions include reminder displays and the entering of new appointments into the system. The cloud service holds the user's data and sends that data to the mobile device on request.

###Use cases
1. Reminder of a chore based on a specific time. For instance, if the user has to pick up a friend at 4:00 PM and has set the final reminder range to be 30 minutes, then the app will remind the user at 3:30 PM.

2. Reminder of a chore based on a specific location. For instance, the user wants to be reminded the next time she is in the office to scan and fax some documents. The app monitors the user's location and pops up a reminder when she enters the office.

3. Reminder of a chore based on a more general location, and optionally time. For instance, the user wants to be reminded to buy cat litter. The app monitors the user's location and pops up a reminder when he is near any of several pet supply stores. The reminder is not shown when the store in question is closed, or when the user has an appointment scheduled soon.

4. Reminder based on both location and time. For instance, the user has guests coming for dinner at 6:00 PM. If the user is still in the office at 5:15 PM, the app will remind the user to leave the office, as the app can calculate the distance between home and office to figure out that it approximately takes 45 minutes to travel.

5. Dismissal of reminders. The user will need to tell the system whether they have done the chore, so that the system knows whether to mark it as done.

6. Entering and editing appointments and reminders. The user will have an interface for entering appointments and chores into the system, and can specify time or times, location or locations, urgency, and so forth. The interface can include a map for entering location.

7. Display of pending reminders. The app will show the user what chores and appointments are scheduled, their times, and their locations.

8. Communication between family and roommates. Many chores can be done by any member of the household, but the household will not want them to be done twice. If each
member of the household has a copy of this app on their smartphone, the apps can be aware of this and only remind one person at a time. Chores such as buying kitty litter will be marked as done from all schedules for the household instead of just one.