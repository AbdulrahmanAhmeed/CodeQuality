using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQuality
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    // Violates: Names rules - not descriptive, uses encoding
    public class clsUserMgr
    {
        // Violates: Magic numbers, not configurable at high level
        private static int MAX_USERS = 100;
        private static string CONNECTION_STRING = "Server=localhost;Database=MyDB;";

        // Violates: Should be small, too many instance variables
        public List<object> users;
        public List<object> admins;
        public List<object> guests;
        public bool isConnected;
        public string lastError;
        public DateTime lastUpdate;
        public int userCount;
        public int adminCount;
        public string currentUser;
        public bool debugMode;

        // Violates: Needless repetition, not DRY
        public clsUserMgr()
        {
            users = new List<object>();
            admins = new List<object>();
            guests = new List<object>();
            isConnected = false;
            lastError = "";
            lastUpdate = DateTime.Now;
            userCount = 0;
            adminCount = 0;
            currentUser = "";
            debugMode = false;
        }

        // Violates: Function rules - too big, does multiple things, side effects, flag arguments
        public bool ProcessUser(string n, string e, int a, bool isAdmin, bool shouldValidate, bool createIfNotExists)
        {
            // Violates: Don't comment out code
            // Console.WriteLine("Processing user: " + n);

            // Violates: Negative conditionals
            if (!shouldValidate)
            {
                // Violates: Redundant comments
                // Set the user name
                currentUser = n;
            }
            else
            {
                // Violates: Boundary conditions not encapsulated, primitive obsession
                if (n.Length < 3 || n.Length > 50 || a < 18 || a > 120)
                {
                    lastError = "Invalid user data";
                    return false;
                }
            }

            // Violates: Logical dependency - depends on currentUser being set
            if (currentUser != "")
            {
                // Violates: Magic numbers everywhere
                if (isAdmin && adminCount >= 10)
                {
                    lastError = "Too many admins";
                    return false;
                }

                if (!isAdmin && userCount >= 90)
                {
                    lastError = "Too many users";
                    return false;
                }
            }

            // Violates: Should prefer polymorphism over if/else chains
            if (isAdmin)
            {
                // Violates: Hybrids structures (half object, half data)
                var adminUser = new { Name = n, Email = e, Age = a, Type = "Admin", Permissions = new List<string> { "Read", "Write", "Delete" } };
                admins.Add(adminUser);
                adminCount++;
            }
            else
            {
                var regularUser = new { Name = n, Email = e, Age = a, Type = "User", Permissions = new List<string> { "Read" } };
                users.Add(regularUser);
                userCount++;
            }

            // Violates: Multi-threading code not separated
            Thread.Sleep(100); // Simulate database operation

            lastUpdate = DateTime.Now;

            if (createIfNotExists)
            {
                // Violates: Needless complexity
                SaveToDatabase(n, e, a, isAdmin ? "Admin" : "User");
            }

            return true;
        } // Violates: Closing brace comments would be here

        // Violates: Function is too long, does multiple things
        public string GetUserInfo(string username, bool includeDetails, bool formatAsJson)
        {
            // Violates: Variables declared far from usage
            string result = "";
            bool found = false;
            object foundUser = null;

            // Violates: Obvious noise comments
            // Loop through users to find the matching one
            foreach (var user in users)
            {
                // Violates: Unsafe casting, no proper encapsulation
                dynamic u = user;
                if (u.Name == username)
                {
                    found = true;
                    foundUser = user;
                    break;
                }
            }

            // Violates: Repeated code (should be extracted)
            if (!found)
            {
                foreach (var admin in admins)
                {
                    dynamic a = admin;
                    if (a.Name == username)
                    {
                        found = true;
                        foundUser = admin;
                        break;
                    }
                }
            }

            if (found)
            {
                dynamic user = foundUser;

                // Violates: Flag arguments should be avoided
                if (formatAsJson)
                {
                    if (includeDetails)
                    {
                        result = $"{{\"name\":\"{user.Name}\",\"email\":\"{user.Email}\",\"age\":{user.Age},\"type\":\"{user.Type}\"}}";
                    }
                    else
                    {
                        result = $"{{\"name\":\"{user.Name}\"}}";
                    }
                }
                else
                {
                    if (includeDetails)
                    {
                        result = $"Name: {user.Name}, Email: {user.Email}, Age: {user.Age}, Type: {user.Type}";
                    }
                    else
                    {
                        result = $"Name: {user.Name}";
                    }
                }
            }
            else
            {
                result = "User not found";
            }

            return result;
        }

        // Violates: Static methods when non-static would be better
        public static void SaveToDatabase(string name, string email, int age, string type)
        {
            // Violates: Hardcoded values, not configurable
            if (name.Length > 255) throw new Exception("Name too long");

            // Violates: Exception handling missing
            // Simulate database save
            Console.WriteLine($"Saving {name} to database...");
        }

        // Violates: Method name doesn't explain what it does
        public void DoStuff()
        {
            // Violates: Meaningless variable names
            int x = 0;
            int y = 1;
            bool flag = true;

            // Violates: Unreachable code, poor logic
            while (flag)
            {
                x++;
                if (x > 5)
                {
                    flag = false;
                }

                // Violates: Side effects in unexpected places
                Console.WriteLine("Processing...");

                if (x == 3)
                {
                    // Violates: Modifying state in unexpected ways
                    userCount = userCount + y;
                }
            }
        }

        // Violates: Long parameter list, unclear purpose
        public bool UpdateUserData(string oldName, string newName, string newEmail, int newAge, bool isActive, bool sendNotification, string notificationMessage, bool logChanges, string logLevel)
        {
            // This method violates multiple rules:
            // - Too many parameters
            // - Unclear what it's supposed to do
            // - Side effects everywhere

            lastError = ""; // Side effect

            // Violates: Nested conditionals, complex logic
            if (oldName != null && newName != null)
            {
                if (oldName != newName)
                {
                    if (isActive)
                    {
                        if (sendNotification)
                        {
                            if (notificationMessage != null && notificationMessage.Length > 0)
                            {
                                Console.WriteLine($"Sending notification: {notificationMessage}");
                            }
                        }
                    }
                }
            }

            return true; // Always returns true, making the boolean return type meaningless
        }
    }
}
