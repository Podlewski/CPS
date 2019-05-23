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

        private Antenna antenna;

        public int NumberOfMeasurement { get; set; }
        public int BasicSignals { get; set; }
        public double TimeUnit { get; set; }
        public double RealSpeed { get; set; }
        public double AbstractSpeed { get; set; }
        public double SignalPeriod { get; set; }
        public double SamplingFrequency { get; set; }
        public double BuffersLength { get; set; }
        public double ReportingPeriod { get; set; }

        public List<double> OriginalList { get; set; }
        public List<double> CountedList { get; set; }
        public List<double> DiffrenceList { get; set; }

        public AntennaViewModel()
        {
            CountCommand = new RelayCommand(Count);

            OriginalList = new List<double>();
            CountedList = new List<double>();
            DiffrenceList = new List<double>();

            NumberOfMeasurement = 10;
            TimeUnit = 10;
            RealSpeed = 10;
            AbstractSpeed = 3000;
            SignalPeriod = 1;
            BasicSignals = 2;
            SamplingFrequency = 100;
            BuffersLength = 500;
            ReportingPeriod = 2;
        }

        public void Count()
        {
            antenna = new Antenna()
            {
                NumberOfMeasurement = NumberOfMeasurement,
                TimeUnit = TimeUnit,
                RealSpeed = RealSpeed,
                AbstractSpeed = AbstractSpeed,
                SignalPeriod = SignalPeriod,
                BasicSignals = BasicSignals,
                SamplingFrequency = SamplingFrequency,
                BuffersLength = BuffersLength,
                ReportingPeriod = ReportingPeriod
            };

            OriginalList = antenna.GetOriginalDistance();
            CountedList = antenna.CountDistances();
            DiffrenceList = antenna.CountDiffrence(OriginalList, CountedList);

            OnPropertyChanged(nameof(OriginalList));
            OnPropertyChanged(nameof(CountedList));
            OnPropertyChanged(nameof(DiffrenceList));
        }
    }
}