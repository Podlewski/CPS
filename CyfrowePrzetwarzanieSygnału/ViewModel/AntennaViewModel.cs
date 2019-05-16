using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

using Logic;
using Microsoft.Win32;

namespace ViewModel
{
    public class AntennaViewModel : BaseViewModel
    {

        public ICommand CountCommand { get; set; }

        public double TimeUnit { get; set; }
        public double RealSpeed { get; set; }
        public double AbstractSpeed { get; set; }
        public double SignalPeroid { get; set; }
        public double SamplingFrequency { get; set; }
        public double BuffersLength { get; set; }
        public double ReportingPeroid { get; set; }

        public AntennaViewModel()
        {
            CountCommand = new RelayCommand(Count);
        }

        public void Count()
        {

        }
    }
}