﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for MarketSegmentsView.xaml
    /// </summary>
    public partial class MarketSegmentsView : Window
    {
        public MarketSegmentsView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.MarketSegmentViewModel();
        }
    }
}
