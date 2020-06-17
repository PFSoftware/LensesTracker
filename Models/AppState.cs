using PFSoftware.Extensions;
using PFSoftware.Extensions.Enums;
using PFSoftware.LensesTracker.Models.Database;
using PFSoftware.LensesTracker.Views;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.LensesTracker.Models
{
    public static class AppState
    {
        public static readonly SQLiteDatabaseInteraction DatabaseInteraction = new SQLiteDatabaseInteraction();
        internal static List<Contact> AllContacts = new List<Contact>();
        public static bool Loaded = false;

        #region Navigation

        /// <summary>Instance of MainWindow currently loaded</summary>
        public static MainWindow MainWindow { get; set; }

        /// <summary>Navigates to selected Page.</summary>
        /// <param name="newPage">Page to navigate to.</param>
        public static void Navigate(Page newPage) => MainWindow.MainFrame.Navigate(newPage);

        /// <summary>Navigates to the previous Page.</summary>
        public static void GoBack()
        {
            if (MainWindow.MainFrame.CanGoBack)
                MainWindow.MainFrame.GoBack();
        }

        #endregion Navigation

        /// <summary>Handles verification of required files.</summary>
        internal static async Task FileManagement()
        {
            if (!Directory.Exists(AppData.Location))
                Directory.CreateDirectory(AppData.Location);
            DatabaseInteraction.VerifyDatabaseIntegrity();
            AllContacts = await DatabaseInteraction.LoadContacts();
        }

        #region Contact Lens Manipulation

        /// <summary>Adds a new <see cref="Contact"/> insertion to the database.</summary>
        /// <param name="newContact"><see cref="Contact"/> insertion to be added</param>
        public static async Task<bool> AddContact(Contact newContact)
        {
            if (await DatabaseInteraction.AddContact(newContact))
            {
                AllContacts.Add(newContact);
                return true;
            }
            return false;
        }

        /// <summary>Modifies an existing contact in the database.</summary>
        /// <param name="originalContact">Contact to be modified</param>
        /// <param name="newContact">Contact with modifications</param>
        public static async Task<bool> ModifyContact(Contact originalContact, Contact newContact)
        {
            if (await DatabaseInteraction.ModifyContact(originalContact, newContact))
            {
                AllContacts.Replace(originalContact, newContact);
                return true;
            }
            return false;
        }

        /// <summary>Removes a contact from the database.</summary>
        /// <param name="removeContact">Contact to be removed</param>
        public static async Task<bool> RemoveContact(Contact removeContact)
        {
            if (await DatabaseInteraction.RemoveContact(removeContact))
            {
                AllContacts.Remove(removeContact);
                return true;
            }
            return false;
        }

        #endregion Contact Lens Manipulation

        #region Notification Management

        /// <summary>Displays a new Notification in a thread-safe way.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        internal static void DisplayNotification(string message, string title) => Application.Current.Dispatcher.Invoke(
            () => { new Notification(message, title, NotificationButton.OK, MainWindow).ShowDialog(); });

        /// <summary>Displays a new Notification in a thread-safe way and retrieves a boolean result upon its closing.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        /// <returns>Returns value of clicked button on Notification.</returns>
        internal static bool YesNoNotification(string message, string title)
        {
            bool result = false;
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (new Notification(message, title, NotificationButton.YesNo, MainWindow).ShowDialog() == true)
                    result = true;
            });
            return result;
        }

        #endregion Notification Management
    }
}