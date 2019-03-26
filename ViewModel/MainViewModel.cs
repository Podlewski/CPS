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
            ComputeCommand = new RelayCommand(Compute);
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

            SelectedTab.PointsX = new List<double>();
            SelectedTab.PointsY = new List<double>();
            SelectedTab.Data.Samples = new List<double>();

            Func<double, double> SelectedGeneration = generator.SelectGenerator(SelectedSignal);

            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += 1 / Sampling)
            {
                SelectedTab.Data.Samples.Add(SelectedGeneration(i));
            }

            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += D_DurationOfTheSignal / 500)
            {
                SelectedTab.PointsX.Add(i);
                SelectedTab.PointsY.Add(SelectedGeneration(i));
            }

            SelectedTab.LoadData(SelectedTab.PointsX, SelectedTab.PointsY, false);
            SelectedTab.CalculateSignalInfo(T1_StartTime, T1_StartTime + D_DurationOfTheSignal);
            SelectedTab.DrawCharts();
        }

        public void Compute()
        {
            if (FirstOperationTab.Data.HasData() && SecondOperationTab.Data.HasData())
            {
                if (!SecondOperationTab.Data.IsValid(FirstOperationTab.Data))
                {
                    MessageBox.Show("Given signals are not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataHandler data = new DataHandler();
                List<double> pointsY = new List<double>();

                pointsY = SelectedOperation.SignalOperation(FirstOperationTab.Data.Samples,
                                                            SecondOperationTab.Data.Samples);

                data.StartTime = SelectedTab.Data.StartTime;
                data.Frequency = SelectedTab.Data.Frequency;
                data.Samples = pointsY;
                data.FromSamples = true;
                SelectedTab.IsScattered = true;
                SelectedTab.LoadData(data);
                SelectedTab.DrawCharts();
            }
        }

        public void Load()
        {
            MessageBox.Show("WIP");
            SelectedTab.LoadDataFromFile(LoadPath(true));
            //SelectedTab.DrawCharts(); coś ten
            SelectedTab.CalculateSignalInfo(isDiscrete: true, fromSamples: true);
        }

        public void Save()
        {
            MessageBox.Show("WIP");
            SelectedTab.SaveDataToFile(LoadPath(false));
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