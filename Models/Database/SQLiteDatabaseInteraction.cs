using PFSoftware.Extensions;
using PFSoftware.Extensions.DatabaseHelp;
using PFSoftware.Extensions.DataTypeHelpers;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PFSoftware.LensesTracker.Models.Database
{
    /// <summary>Represents database interaction covered by SQLite.</summary>
    public class SQLiteDatabaseInteraction
    {
        private const string _DATABASENAME = "LensesTracker.sqlite";
        private readonly string _con = $"Data Source = {Path.Combine(AppData.Location, _DATABASENAME)}; foreign keys = TRUE; Version = 3;";

        /// <summary>Verifies that the requested database exists and that its file size is greater than zero. If not, it extracts the embedded database file to the local output folder.</summary>
        public void VerifyDatabaseIntegrity() => Functions.VerifyFileIntegrity(
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"PFSoftware.LensesTracker.{_DATABASENAME}"), _DATABASENAME, AppData.Location);

        #region Lenses Management

        #region Contact Lens Manipulation

        /// <summary>Adds a new <see cref="Contact"/> insertion to the database.</summary>
        /// <param name="newContact"><see cref="Contact"/> insertion to be added</param>
        public Task<bool> AddContact(Contact newContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "INSERT INTO Contacts([Date], [Side], [ReplacementDate])" +
                              "VALUES(@date, @side, @replacementDate)"
            };
            cmd.Parameters.AddWithValue("@date", newContact.DateToString);
            cmd.Parameters.AddWithValue("@side", newContact.SideToString);
            cmd.Parameters.AddWithValue("@replacementDate", newContact.ReplacementDateToString);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        /// <summary>Loads all <see cref="Contact"/> insertions from the database.</summary>
        /// <returns>All <see cref="Contact"/> insertions</returns>
        public async Task<List<Contact>> LoadContacts()
        {
            List<Contact> allContacts = new List<Contact>();
            DataSet ds = await SQLiteHelper.FillDataSet(_con, "SELECT * FROM Contacts");
            if (ds.Tables[0].Rows.Count > 0)
            {
                allContacts.AddRange(from DataRow dr in ds.Tables[0].Rows select new Contact(DateTimeHelper.Parse(dr["Date"]), EnumHelper.Parse<Side>(dr["Side"].ToString()), DateTimeHelper.Parse(dr["ReplacementDate"])));
                allContacts = allContacts.OrderByDescending(contact => contact.Date)
                    .ThenBy(contact => contact.SideToString).ToList();
            }

            return allContacts;
        }

        /// <summary>Modifies an existing <see cref="Contact"/> in the database.</summary>
        /// <param name="originalContact"><see cref="Contact"/> to be modified</param>
        /// <param name="newContact"><see cref="Contact"/> with modifications</param>
        /// <returns>True if successful</returns>
        public Task<bool> ModifyContact(Contact originalContact, Contact newContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "Update Contacts SET [Date] = @newDate, [Side] = @newSide, [ReplacementDate] = @newReplacementDate WHERE [Date] = @oldDate AND [Side] = @oldSide AND [ReplacementDate] = @oldReplacementDate"
            };
            cmd.Parameters.AddWithValue("@newDate", newContact.DateToString);
            cmd.Parameters.AddWithValue("@newSide", newContact.SideToString);
            cmd.Parameters.AddWithValue("@newReplacementDate", newContact.ReplacementDateToString);
            cmd.Parameters.AddWithValue("@oldDate", originalContact.DateToString);
            cmd.Parameters.AddWithValue("@oldSide", originalContact.SideToString);
            cmd.Parameters.AddWithValue("@oldReplacementDate", originalContact.ReplacementDateToString);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        /// <summary>Removes a <see cref="Contact"/> from the database.</summary>
        /// <param name="removeContact"><see cref="Contact"/> to be removed</param>
        /// <returns>True if successful</returns>
        public Task<bool> RemoveContact(Contact removeContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "DELETE FROM Contacts WHERE [Date] = @date AND [Side] = @side AND [ReplacementDate] = @replacementDate"
            };
            cmd.Parameters.AddWithValue("@date", removeContact.DateToString);
            cmd.Parameters.AddWithValue("@side", removeContact.SideToString);
            cmd.Parameters.AddWithValue("@replacementDate", removeContact.ReplacementDateToString);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        #endregion Contact Lens Manipulation

        #endregion Lenses Management
    }
}