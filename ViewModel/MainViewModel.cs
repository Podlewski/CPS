using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using Logic;
using Microsoft.Win32;

namespace ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        #region Commands

        public ICommand AddTabCommand { get; set; }
        public ICommand GenerateCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<TabViewModel> TabList { get; set; }
        public TabViewModel SelectedTab { get; set; }
        public TabViewModel FirstOperationTab { get; set; }
        public TabViewModel SecondOperationTab { get; set; }


        public List<string> SignalList { get; set; }
        public string SelectedSignal { get; set; }

        public List<string> OperationList { get; set; }
        public string SelectedOperation { get; set; }

        #region Factors

        public double A_Amplitude { get; set; }
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; }
        public double T_BasicPeroid { get; set; }
        public double Kw_DutyCycle { get; set; }
        public double Ts_TimeStep { get; set; }
        public double P_Probability { get; set; }
        public double Sampling { get; set; }

        #endregion

        #endregion

        public MainViewModel()
        {
            TabList = new ObservableCollection<TabViewModel>();
            AddTab();
            SelectedTab = TabList[0];
            FirstOperationTab = TabList[0];
            SecondOperationTab = TabList[0];

            SignalList = new List<string>()
            {
                "01) Szum o rozkładzie jednostajnym",
                "02) Szum Gaussowski",
                "03) Sygnał sinusoidalny",
                "04) Sygnał sinusoidalny wyprostowany jednopołówkowo",
                "05) Sygnał sinusoidalny wyprostowany dwupołówkowo",
                "06) Sygnał prostokątny",
                "07) Sygnał prostokątny symetryczny",
                "08) Sygnał trójkątny",
                "09) Skok jednostkowy",
                "10) Impuls jednostkowy",
                "11) Szum impulsowy"
            };
            SelectedSignal = SignalList[2];

            OperationList = new List<string>()
            {
                "1) Dodawanie",
                "2) Odejmowanie",
                "3) Mnożenie",
                "4) Dzielenie",
            };
            SelectedOperation = OperationList[0];

            AddTabCommand = new RelayCommand(AddTab);
            GenerateCommand = new RelayCommand(Generate);
            //ComputeCommand = new RelayCommand(Compute);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
        }

        public void AddTab()
        {
            TabList.Add(new TabViewModel(TabList.Count));
        }

        public void Generate()
        {
            Generator generator = new Generator()
            {
                A = A_Amplitude,
                T1 = T1_StartTime,
                T = T_BasicPeroid,
                Kw = Kw_DutyCycle,
                Ts = Ts_TimeStep,
                P = P_Probability
            };

            Func<double, double> selectedGeneration = generator.SelectGenerator(SelectedSignal);

            if (selectedGeneration != null)
            {
                SignalData signalData = new SignalData(T1_StartTime, Sampling);
                SelectedTab.IsScattered = SelectedSignal.IsGenerationScattered();

                if (SelectedTab.IsScattered == false)
                {
                    for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += D_DurationOfTheSignal / 1000)
                    {
                        signalData.PointsX.Add(i);
                        signalData.PointsY.Add(selectedGeneration(i));
                    }

                    for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += 1 / Sampling)
                    {
                        signalData.SamplesX.Add(i);
                        signalData.SamplesY.Add(selectedGeneration(i));
                    }
                }
                else
                {
                    for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += 1 / Sampling)
                    {
                        signalData.PointsX.Add(i / Sampling);

                        if (SelectedSignal.Substring(0, 2) == "10")
                            signalData.PointsY.Add(selectedGeneration(i / Sampling));
                        else if (SelectedSignal.Substring(0, 2) == "11")
                            signalData.PointsY.Add(0);
                    }
                }

                SelectedTab.SignalData = signalData; 
                SelectedTab.DrawCharts();
            }
        }

            

        //public void Compute()
        //{
        //    if (FirstOperationTab.SignalData.HasData() && SecondOperationTab.SignalData.HasData())
        //    {
        //        if (!SecondOperationTab.SignalData.IsValid(FirstOperationTab.SignalData))
        //        {
        //            MessageBox.Show("Given signals are not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        SignalData data = new SignalData();
        //        List<double> pointsY = new List<double>();

        //        pointsY = SelectedOperation.SignalOperation(FirstOperationTab.SignalData.Samples,
        //                                                    SecondOperationTab.SignalData.Samples);

        //        data.StartTime = SelectedTab.SignalData.StartTime;
        //        data.Sampling = SelectedTab.SignalData.Sampling;
        //        data.Samples = pointsY;
        //        data.FromSamples = true;
        //        SelectedTab.IsScattered = true;
        //        SelectedTab.LoadData(data);
        //        SelectedTab.DrawCharts();
        //    }
        //}

        public void Load()
        {
            MessageBox.Show("WIP");
            //SelectedTab.LoadDataFromFile(LoadPath(true));
            //SelectedTab.DrawCharts(); coś ten
            //SelectedTab.CalculateSignalInfo(isDiscrete: true, fromSamples: true);
        }

        public void Save()
        {
            MessageBox.Show("WIP");
            //SelectedTab.SaveDataToFile(LoadPath(false));
        }

        public string LoadPath(bool loadMode)
        {
            if (loadMode)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Bin File(*.bin)| *.bin",
                    RestoreDirectory = true
                };

                openFileDialog.ShowDialog();

                if (openFileDialog.FileName.Length == 0)
                {
                    MessageBox.Show("No files selected");
                    return null;
                }

                return openFileDialog.FileName;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Bin File(*.bin)| *.bin",
                RestoreDirectory = true
            };

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName.Length == 0)
            {
                MessageBox.Show("No files selected");
                return null;
            }

            return saveFileDialog.FileName;
        }

    }
}