using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CodeQuality // Should be file-scoped namespace (IDE0161)
{
    // Multiple classes in one file (violates SA1402)
    public class user_service // Wrong naming convention (SA1300, CA1707)
    {
        // Missing documentation (SA1600)
        // Hungarian notation (SA1305)
        private string strUserName;
        private string user_password; // Underscore in field name (SA1310)
        private int m_userId; // Hungarian notation (SA1305)
        private List<string> unusedField; // Unused field (CA1823)
        
        // Missing 'this.' prefix (SA1101)
        public string UserName 
        {
            get 
            {
                return strUserName; // Should use 'this.' (SA1101)
            }
            set 
            {
                strUserName = value;
                // Should use 'this.' (SA1101)
            }
        }

        // Constructor with too many parameters (bad practice)
        public user_service(string userName, string password, int userId, string email, DateTime createdDate, bool isActive, string role, string department)
        {
            // Not using 'this.' prefix (SA1101)
            strUserName = userName;
            user_password = password;
            m_userId = userId;
        }

        // Async method without 'Async' suffix (VSTHRD200)
        public async Task<string> GetUser()
        {
            // Not using ConfigureAwait(false) (CA2007)
            var result = await Task.FromResult(ProcessUserData());
            return result;
        }

        // Method with too many parameters and complexity (CA1502)
        public string ProcessUserData(string param1 = null, string param2 = null, string param3 = null)
        {
            // Excessive complexity and nesting (CA1505)
            if (strUserName != null)
            {
                if (user_password != null)
                {
                    if (m_userId > 0)
                    {
                        if (param1 != null)
                        {
                            if (param2 != null)
                            {
                                if (param3 != null)
                                {
                                    return "Complex logic";
                                }
                                else
                                {
                                    return "Default";
                                }
                            }
                            else
                            {
                                return "Another default";
                            }
                        }
                        else
                        {
                            return "Yet another default";
                        }
                    }
                    else
                    {
                        return "Invalid user";
                    }
                }
                else
                {
                    return "No password";
                }
            }
            else
            {
                return "No username";
            }
        }

        // SQL injection vulnerability (CA2100, CA3001)
        public List<string> GetUsersByName(string name)
        {
            var connectionString = "Server=localhost;Database=TestDB;Integrated Security=true;";
            var results = new List<string>();
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // SQL injection vulnerability - directly concatenating user input
                var query = "SELECT * FROM Users WHERE Name = '" + name + "'";
                var command = new SqlCommand(query, connection);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader["Name"].ToString());
                    }
                }
            }
            
            return results;
        }

        // Insecure deserialization (CA2321)
        public object DeserializeFromUser(string jsonData)
        {
            // Using insecure deserialization with type information
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            
            return JsonConvert.DeserializeObject(jsonData, settings);
        }

        // Weak cryptography (CA5350)
        public string HashPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = md5.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Method that should be static (CA1822)
        public string GetConstantValue()
        {
            return "This is a constant value";
        }

        // Catching general exceptions (CA1031)
        public void ProcessData()
        {
            try
            {
                // Some risky operation
                var data = File.ReadAllText("nonexistent.txt");
            }
            catch (Exception ex) // Too broad exception handling
            {
                // Re-throwing exception incorrectly (CA2200)
                throw ex; // Should be just 'throw;'
            }
        }

        // Unused parameter (CA1801, IDE0060)
        public void DoSomething(string unusedParam)
        {
            Console.WriteLine("Doing something");
        }

        // Method with 'out' parameter (CA1045)
        public bool TryGetUser(int id, out string userName)
        {
            userName = strUserName;
            return true;
        }

        // Async void method (VSTHRD100)
        public async void ProcessUserAsync()
        {
            await Task.Delay(1000);
        }

        // Not using modern C# features
        public string GetUserStatus()
        {
            // Should use switch expression (IDE0066)
            switch (m_userId)
            {
                case 1:
                    return "Active";
                case 2:
                    return "Inactive";
                case 3:
                    return "Pending";
                default:
                    return "Unknown";
            }
        }

        // Not using pattern matching (IDE0019)
        public bool IsValidUser(object user)
        {
            if (user.GetType() == typeof(user_service))
            {
                user_service userService = (user_service)user;
                return userService.m_userId > 0;
            }
            return false;
        }

        // Not using null-conditional operator (IDE0031)
        public string GetUserNameSafely()
        {
            if (strUserName != null)
            {
                return strUserName.ToUpper();
            }
            return null;
        }

        // Using inefficient LINQ (CA1827)
        public bool HasUsers(List<string> users)
        {
            return users.Count() > 0; // Should use Any()
        }

        // Using wrong array initialization (CA1825)
        public string[] GetEmptyArray()
        {
            return new string[0]; // Should use Array.Empty<string>()
        }

        // Missing 'async' with 'await' (should be async method)
        public Task<string> GetUserAsync()
        {
            return Task.FromResult("User");
        }

        // Not disposing properly (CA1816)
        ~user_service()
        {
            // Finalizer without calling GC.SuppressFinalize
        }
    }

    // Another class in same file (violates SA1402)
    public class DatabaseHelper
    {
        // Static field not initialized inline (CA1810)
        public static string ConnectionString;
        
        static DatabaseHelper()
        {
            ConnectionString = "Server=localhost;Database=TestDB;Integrated Security=true;";
        }

        // Method with file path injection vulnerability (CA3003)
        public string ReadUserFile(string fileName)
        {
            // Potential path traversal attack
            var filePath = Path.Combine("C:\\Users\\", fileName);
            return File.ReadAllText(filePath);
        }

        // Command injection vulnerability (CA3006)
        public void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + command, // Direct command injection
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            
            Process.Start(processInfo);
        }

        // Regex injection vulnerability (CA3012)
        public bool IsValidInput(string input, string pattern)
        {
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            return regex.IsMatch(input);
        }
    }

    // Class with generic constraint violations
    public class GenericHelper<T>
    {
        // Inefficient generic method (should be constrained)
        public void ProcessItem(T item)
        {
            // Inefficient boxing/unboxing
            if (item.Equals(default(T)))
            {
                Console.WriteLine("Default value");
            }
        }

        // Method that could be static (CA1822)
        public string GetTypeName()
        {
            return typeof(T).Name;
        }
    }

    // Sealed class with virtual method (design issue)
    public sealed class SealedClass
    {
        public void VirtualMethod() // Removed virtual keyword since it's sealed
        {
            Console.WriteLine("This shouldn't be virtual");
        }
    }

    // Abstract class with constructor (CA1012)
    public abstract class AbstractClass
    {
        // Abstract classes shouldn't have public constructors
        public AbstractClass()
        {
            Console.WriteLine("Abstract constructor");
        }
        
        public abstract void AbstractMethod();
    }

    // Struct that should be readonly (IDE0250)
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        // Not overriding Equals/GetHashCode (CA1815)
        public void Move(int deltaX, int deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }
    }

    // Main program with poor practices
    public class Program
    {
        // Entry point with poor design
        public static async Task Main(string[] args)
        {
            // Not using top-level statements (IDE0210)
            // Not using using declaration (IDE0063)
            var userService = new user_service("test", "password", 1, "test@test.com", DateTime.Now, true, "Admin", "IT");
            
            // Not using var when type is obvious (IDE0007)
            user_service service = new user_service("test", "password", 1, "test@test.com", DateTime.Now, true, "Admin", "IT");
            
            // Inefficient string concatenation (CA1834)
            string result = "";
            for (int i = 0; i < 1000; i++)
            {
                result += "test"; // Should use StringBuilder
            }
            
            // Using wrong collection initialization (IDE0028)
            var list = new List<string>();
            list.Add("item1");
            list.Add("item2");
            list.Add("item3");
            
            // Not using null coalescing (IDE0029)
            string value = null;
            string result2;
            if (value != null)
            {
                result2 = value;
            }
            else
            {
                result2 = "default";
            }
            
            // Not using pattern matching (IDE0078)
            object obj = "test";
            if (obj is string)
            {
                string str = (string)obj;
                Console.WriteLine(str);
            }
            
            // Ignoring async result (VSTHRD110)
            _ = DoAsyncWork();
            
            // Calling async method synchronously (VSTHRD103)
            var task = DoAsyncWork();
            var result3 = task.Result; // Should use await
            
            // Not using collection expression (IDE0300)
            var numbers = new int[] { 1, 2, 3, 4, 5 };
            
            // Not using nameof (IDE0005)
            if (args.Length > 0)
            {
                throw new ArgumentException("Invalid argument", "args");
            }
            
            // Potential null reference (CS8602)
            string nullString = null;
            Console.WriteLine(nullString.Length); // Will throw NullReferenceException
        }
        
        // Async method without proper naming (VSTHRD200)
        public static async Task<string> DoAsyncWork()
        {
            // Not using ConfigureAwait(false) (CA2007)
            await Task.Delay(1000);
            return "Done";
        }
    }
}
