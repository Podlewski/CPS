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

        public double A_Amplitude { get; set; } = 1;
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; } = 4;
        public double T_BasicPeroid { get; set; } = 1;
        public double Kw_DutyCycle { get; set; } = 0.5;
        public double Ts_TimeStep { get; set; } = 2;
        public double P_Probability { get; set; } = 0.5;
        public double Sampling { get; set; } = 1;

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
            SelectedSignal = SignalList[0];

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

            Func<double, double> selectedGeneration = generator.SelectGenerator(SelectedSignal);

            if (selectedGeneration != null)
            {
                SignalData signalData = new SignalData(T1_StartTime, Sampling);
                SelectedTab.IsScattered = SelectedSignal.IsGenerationScattered();

                if (SelectedTab.IsScattered == false)
                {
                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal);
                         i += (decimal)D_DurationOfTheSignal / 500)
                    {
                        double j = (double)i;

                        signalData.PointsX.Add(j);
                        signalData.PointsY.Add(selectedGeneration(j));
                    }

                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)Sampling)
                    {
                        double j = (double)i;

                        signalData.SamplesX.Add(j);
                        signalData.SamplesY.Add(selectedGeneration(j));
                    }
                }
                else if (SelectedSignal.Substring(0, 2) == "10")
                {
                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)Sampling)
                    {
                        double j = (double)i;

                        signalData.PointsX.Add(j);
                        signalData.PointsY.Add(selectedGeneration(j));
                    }
                }
                else if (SelectedSignal.Substring(0, 2) == "11")
                {
                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)Sampling)
                    {
                        double j = (double)i;

                        signalData.PointsX.Add(j);
                        signalData.PointsY.Add(selectedGeneration(j));
                    }
                }

                    SelectedTab.SignalData = signalData;
                SelectedTab.CalculateSignalInfo(T1_StartTime, T1_StartTime + D_DurationOfTheSignal);
                SelectedTab.DrawCharts();
            }
        }

        public void Compute()
        {
            if (FirstOperationTab.SignalData.IsEmpty() && SecondOperationTab.SignalData.IsEmpty())
            {
                if (SecondOperationTab.SignalData.IsInvalid(FirstOperationTab.SignalData))
                {
                    MessageBox.Show("Błąd: te sygnały nie pasują do siebie.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SignalData signalData = new SignalData(FirstOperationTab.SignalData.StartTime,
                                                       FirstOperationTab.SignalData.Sampling)
                {
                    SamplesX = FirstOperationTab.SignalData.SamplesX,
                    SamplesY = SelectedOperation.SignalOperation(FirstOperationTab.SignalData.SamplesY,
                                                            SecondOperationTab.SignalData.SamplesY),

                    UsesSamples = true
                };

                SelectedTab.SignalData = signalData;
                SelectedTab.IsScattered = true;
                SelectedTab.DrawCharts();
            }
        }

        public void Load()
        {
            //MessageBox.Show("WIP");
            SelectedTab.LoadDataFromFile(LoadPath(true));
            SelectedTab.DrawCharts(); 
            SelectedTab.CalculateSignalInfo(isDiscrete: true, fromSamples: true);
        }

        public void Save()
        {
            //MessageBox.Show("WIP");
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