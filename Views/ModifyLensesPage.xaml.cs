﻿using PFSoftware.Extensions.DataTypeHelpers;
using PFSoftware.LensesTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.LensesTracker.Views
{
    /// <summary>Interaction logic for ModifyContactPage.xaml</summary>
    public partial class ModifyLensesPage : Page
    {
        private Contact _originalContact, _modifiedContact;

        /// <summary>Loads the <see cref="Contact"/> to be modified.</summary>
        /// <param name="loadContact"><see cref="Contact"/> to be modified</param>
        private void LoadContact(Contact loadContact)
        {
            _originalContact = new Contact(loadContact);
            _modifiedContact = new Contact(loadContact);
            InsertionDate.SelectedDate = _originalContact.Date;
            ReplacementDate.SelectedDate = _originalContact.ReplacementDate;
            CmbSide.SelectedItem = _originalContact.SideToString;
        }

        #region Click

        private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            _modifiedContact.Date = InsertionDate.SelectedDate.Value;
            _modifiedContact.ReplacementDate = ReplacementDate.SelectedDate.Value;
            _modifiedContact.Side = EnumHelper.Parse<Side>(CmbSide.SelectedItem.ToString());

            if (_originalContact != _modifiedContact)
            {
                if (await AppState.ModifyContact(_originalContact, _modifiedContact))
                    AppState.GoBack();
                else
                    AppState.DisplayNotification("Unable to modify contact lens.", "Personal Tracker");
            }
            else
                AppState.DisplayNotification("You haven't modified the contact.", "Personal Tracker");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        #endregion Click

        #region Page-Manipulation Methods

        internal ModifyLensesPage(Contact selectedContact)
        {
            InitializeComponent();
            CmbSide.Items.Add("Left");
            CmbSide.Items.Add("Right");
            LoadContact(selectedContact);
        }

        #endregion Page-Manipulation Methods
    }
}