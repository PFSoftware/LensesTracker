﻿using PFSoftware.Extensions;
using PFSoftware.Extensions.DataTypeHelpers;
using PFSoftware.Extensions.ListViewHelp;
using PFSoftware.LensesTracker.Models;

using System;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.LensesTracker.Views
{
    /// <summary>Interaction logic for MainPage.xaml</summary>
    public partial class LensesPage
    {
        private ListViewSort _sort = new ListViewSort();
        private Contact _selectedContact = new Contact();

        /// <summary>Adds a new <see cref="Contact"/> to the database</summary>
        /// <param name="sides">Sides on which contacts are being added</param>
        private async void NewContact(params Side[] sides)
        {
            DateTime replacementDate = DateTimeHelper.Parse(DateNewContact.SelectedDate);
            switch (CmbLength.SelectedItem.ToString())
            {
                case "1 week":
                    replacementDate = replacementDate.AddDays(7);
                    break;

                case "2 weeks":
                    replacementDate = replacementDate.AddDays(14);
                    break;

                case "30 days":
                    replacementDate = replacementDate.AddDays(30);
                    break;
            }
            foreach (Side side in sides)
                await AppState.AddContact(new Contact(DateTimeHelper.Parse(DateNewContact.SelectedDate), side, replacementDate));
            RefreshItemsSource();
        }

        /// <summary>Refreshes the ItemsSource of LVContacts.</summary>
        internal void RefreshItemsSource()
        {
            LVContacts.ItemsSource = AppState.AllContacts;
            LVContacts.Items.Refresh();
        }

        /// <summary>Toggles the Add Buttons on the Page.</summary>
        /// <param name="enabled">Should the buttons be enabled?</param>
        private void ToggleButtons(bool enabled)
        {
            BtnAddBoth.IsEnabled = enabled;
            BtnAddLeft.IsEnabled = enabled;
            BtnAddRight.IsEnabled = enabled;
        }

        /// <summary>Toggles the Modify and Delete Buttons on the Page.</summary>
        /// <param name="enabled">Should the buttons be enabled?</param>
        private void ToggleModifyDelete(bool enabled)
        {
            BtnDeleteContact.IsEnabled = enabled;
            BtnModifyContact.IsEnabled = enabled;
        }

        #region Click

        private void BtnAddBoth_Click(object sender, RoutedEventArgs e) => NewContact(Side.Left, Side.Right);

        private void BtnAddLeft_Click(object sender, RoutedEventArgs e) => NewContact(Side.Left);

        private void BtnAddRight_Click(object sender, RoutedEventArgs e) => NewContact(Side.Right);

        private async void BtnDeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.YesNoNotification("Are you sure you want to delete this contact? This action cannot be undone.", "Personal Tracker"))
            {
                await AppState.RemoveContact(_selectedContact);
                RefreshItemsSource();
            }
        }

        private void BtnModifyContact_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ModifyLensesPage(_selectedContact));

        private void LVContactsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort =
            Functions.ListViewColumnHeaderClick(sender, _sort, LVContacts, "#CCCCCC");

        #endregion Click

        #region Page-Manipulation Methods

        public LensesPage()
        {
            InitializeComponent();
            DateNewContact.SelectedDate = DateTime.Today;
            CmbLength.Items.Add("1 week");
            CmbLength.Items.Add("2 weeks");
            CmbLength.Items.Add("30 days");
            CmbLength.SelectedIndex = 0;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppState.Loaded)
            {
                await AppState.FileManagement();
                AppState.Loaded = true;
            }

            RefreshItemsSource();
        }

        private void DateNewContact_SelectedDateChanged(object sender, SelectionChangedEventArgs e) =>
            ToggleButtons(DateNewContact.Text.Length > 0);

        private void LVContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LVContacts.SelectedIndex >= 0)
            {
                ToggleModifyDelete(true);
                _selectedContact = (Contact)LVContacts.SelectedItem;
            }
            else
            {
                ToggleModifyDelete(false);
                _selectedContact = new Contact();
            }

            #endregion Page-Manipulation Methods
        }
    }
}