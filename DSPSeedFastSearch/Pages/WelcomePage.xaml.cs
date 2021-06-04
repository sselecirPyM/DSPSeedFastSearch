﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSPSeedFastSearch.Pages
{
    /// <summary>
    /// WelcomePage.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void FastStarDataButton_Click(object sender, RoutedEventArgs e)
        {
            FastStarDataPage fastStarDataPage = new FastStarDataPage();
            this.NavigationService.Navigate(fastStarDataPage);
        }
    }
}
