using System;
using System.Reflection;
using Rage;
using Rage.Attributes;
using RAGENativeUI;

[assembly: Rage.Attributes.Plugin("BreakReminder", Description = "A plugin that reminds you to"
    + " take breaks once in a while.", Author = "Galactipod"
)]

namespace BreakReminder {
    public static class EntryPoint {
        public const string AUTHOR = "Galactipod";
        public static bool timeNotificationsEnabled = true;
        public static bool twelveHourClockEnabled = true;
        public static bool breakNotificationsEnabled = true;
        public static bool stopPlayingRequestsEnabled = true;
        public static int secondsPerNotification = 600;
        public static int secondsPerBreakReminder = 1800;
        public static int secondsPerStopPlayingRequest = 10800;
        public static string timeNotificationText = "~b~BreakReminder:~s~ The current time is ~b~{0}~s~.";
        public static string breakReminderText = "~b~BreakReminder:~s~ You have been playing for ~y~{0} minutes.~s~ Please remember to get up and move around once in a while.";
        public static string breakRequestText = "~b~BreakReminder:~y~ You have been playing for ~r~{0} hours.~s~ You should probably stop playing for a while and do something else.";
        public static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string pluginName = typeof(EntryPoint).Namespace.ToString();

    public static void Main() {
            GetBreakReminderSettings();
            Game.LogTrivial(String.Format("Plugin {0} v{1} by {2} has been initialized.",
                pluginName, version, AUTHOR));
            Game.DisplayNotification(String.Format("~b~{0}~s~ beta {1} by ~g~{2}~s~ has been loaded.",
                pluginName, version, AUTHOR));
            TimeNotifLoop();
        }

        [ConsoleCommand("Re-reads config without reloading plugin.")]
        public static void GetBreakReminderSettings() {
            InitializationFile iniFile = new InitializationFile("plugins/"+ pluginName + ".ini");

            timeNotificationsEnabled = iniFile.ReadBoolean("Settings","timeNotificationsEnabled", timeNotificationsEnabled);
            twelveHourClockEnabled = iniFile.ReadBoolean("Settings", "twelveHourClockEnabled", twelveHourClockEnabled);
            secondsPerNotification = iniFile.ReadInt32("Settings", "secondsPerTimeNotification", secondsPerNotification);
            breakNotificationsEnabled = iniFile.ReadBoolean("Settings", "breakRemindersEnabled", breakNotificationsEnabled);
            secondsPerBreakReminder = iniFile.ReadInt32("Settings", "secondsPerBreakReminder", secondsPerBreakReminder);
            stopPlayingRequestsEnabled = iniFile.ReadBoolean("Settings", "stopPlayingRequestsEnabled", stopPlayingRequestsEnabled);
            secondsPerStopPlayingRequest = iniFile.ReadInt32("Settings", "secondsPerStopPlayingRequest", secondsPerStopPlayingRequest);

            timeNotificationText = iniFile.ReadString("Settings", "timeNotificationText", timeNotificationText);
            breakReminderText = iniFile.ReadString("Settings", "breakReminderText", breakReminderText);
            breakRequestText = iniFile.ReadString("Settings", "breakRequestText", breakRequestText);
        }

        public static void TimeNotifLoop() {
            var startTime = DateTime.Now;
            DateTime Now;
            int secondsSinceMidnight, intTotalSpanSeconds;
            string clockTime;
            while (true) {
                Now = DateTime.Now;
                TimeSpan span = Now - startTime;
                TimeSpan sinceMidnight = Now - DateTime.Today;
                secondsSinceMidnight = Convert.ToInt32(sinceMidnight.TotalSeconds);
                intTotalSpanSeconds = Convert.ToInt32(span.TotalSeconds);

                if (twelveHourClockEnabled) {
                    clockTime = Now.ToString("hh:mm tt");
                } else {
                    clockTime = Now.ToString("HH:mm");
                }

                if (timeNotificationsEnabled && (secondsSinceMidnight % secondsPerNotification  == 0)) {
                    Game.DisplayNotification(String.Format(timeNotificationText,  clockTime));
                    Game.LogTrivial("Notified the player about the current time: " + clockTime);
                }
                if (breakNotificationsEnabled && (intTotalSpanSeconds != 0) && intTotalSpanSeconds
                        % secondsPerBreakReminder == 0) {
                    Game.DisplayNotification(String.Format(breakReminderText, Math.Round(span.TotalMinutes)));
                    Game.LogTrivial("Reminded the player to take a break.");
                }
                if (stopPlayingRequestsEnabled && (intTotalSpanSeconds != 0) && intTotalSpanSeconds % secondsPerStopPlayingRequest == 0) {
                    Game.DisplayNotification(String.Format(breakRequestText, Math.Round(span.TotalHours, 1)));
                    Game.LogTrivial("Asked the player to stop playing for a while.");
                }
                GameFiber.Sleep(1000);
            }
        }
    }
}
