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

###App UI
The app’s main screen makes use of the Pivot Control: 

  * The Pivot Control contains two individual PivotItem controls, each contains a listbox. Data is populated to the ‘All Tasks’ listbox from the Cloud Databases (see more information on the Cloud Section below).
    * The first PivotItem is the Notifications, which updates notifications to remind users of events based on time and/or location.
    * The second PivotItem is All Tasks, which lists all the upcoming to-do items for the users, with information such as preview icon, task name, location, deadline date and time.
  * On the bottom of the screen, the App implements the Application Bar class for easy access and controls of the App. There are two quick Button Icons: Add and Refresh. (More details below). Pressing the ‘...’ on the right of the Application Bar will bring up a more detailed Menu, where there are 3 Options:
    * Clear All Notifications: simply remove all current notifications on the list
    * Set My Location: set the user’s GPS/Location options
    * Set My Profile: set the user’s ID
  * The default GPS option is on, for the Demo purpose, we can set it to off and choose a preset location such as On Campus, Washington Ave St Paul or Airport to test Location-based notifications (without having to physically move to these places to test)
  * My Profile set a User ID for the app, to differentiate users on the Cloud Database, the ideal choice is to use the unique Device ID. (using GetDeviceUniqueID()). Since this is not possible on the Windows Phone Emulator, the current option is to manually set the user’s ID. The default value is 100, to test multi-users functionality, set a different ID for each user.
  * The Task Lists refreshes itself automatically, but the user can manually refresh with the Refresh icon on the application bar to instantly check the most up-to-date list. Users can add new task to the Task List by tapping on the Add icon on the application bar at the bottom of the screen. 
  * The Add icon button takes users to another screen, where they can add a new task with all the necessary information. The date and time can be selected using two components: <toolkit: DatePicker> and <toolkit: TimePicker>
  * On the main Tasks screen, users can also select a task, tap and hold the item on the list, a menu will be displayed using the ContextMenu from the same toolkit:
<toolkit: ContextMenuService.ContextMenu>. This menu allows the users to Delete: simply remove the task completely from the list.
  * All actions will be updated to the cloud databases instantly

