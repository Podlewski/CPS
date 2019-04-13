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
        public ICommand QuitCommand { get; set; }

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
        public int Sampling { get; set; } = 100;
        public int QuantizationSampling { get; set; } = 10;
        public int ReconstructionSamples { get; set; } = 100;

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
            QuitCommand = new RelayCommand(Quit);
        }

        public void AddTab()
        {
            TabList.Add(new TabViewModel(TabList.Count));
        }

        public void Generate()
        {
            if (Sampling < QuantizationSampling)
            {
                MessageBox.Show("Częstość próbkowania jest mniejsza od częstotliwości próbkowania przy kwantyzacji. Zamieniam", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);

                int tmpSampling = Sampling;
                Sampling = QuantizationSampling;
                QuantizationSampling = tmpSampling;
                OnPropertyChanged(nameof(Sampling));
                OnPropertyChanged(nameof(QuantizationSampling));
            }
            if (Sampling < 100)
            {
                MessageBox.Show("Częstość próbkowania zwiększona do 100.", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                Sampling = 100;
                OnPropertyChanged(nameof(Sampling));
            }
            if (Sampling % QuantizationSampling != 0)
            {
                MessageBox.Show("Częstotliwości próbkowania nie są dzielnikiem oraz dzielną. Osobne generowanie", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

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
                SignalData signalData = new SignalData(T1_StartTime, Sampling, QuantizationSampling);

                for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)Sampling)
                {
                    double j = (double)i;

                    signalData.SamplesX.Add(j);
                    signalData.SamplesY.Add(selectedGeneration(j));
                }

                if (Sampling % QuantizationSampling == 0)
                {
                    for(int i = 0; i < signalData.SamplesX.Count; i += Sampling/QuantizationSampling)
                    {
                        signalData.QuantizationSamplesX.Add(signalData.SamplesX[i]);
                        signalData.QuantizationSamplesY.Add(signalData.SamplesY[i]);
                    }
                }
                else
                {
                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)QuantizationSampling)
                    {
                        double j = (double)i;

                        signalData.QuantizationSamplesX.Add(j);
                        signalData.QuantizationSamplesY.Add(selectedGeneration(j));
                    }
                }

                SelectedTab.SignalData = signalData;
                SelectedTab.IsScattered = SelectedSignal.IsGenerationScattered();
                SelectedTab.CalculateSignalInfo(T1_StartTime, T1_StartTime + D_DurationOfTheSignal);
                SelectedTab.DrawCharts();
            }
        }

        public void Compute()
        {
            if (FirstOperationTab.SignalData.IsEmpty() && SecondOperationTab.SignalData.IsEmpty())
            {
                string message = "";

                if (SecondOperationTab.SignalData.IsInvalid(FirstOperationTab.SignalData, message))
                {
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SignalData signalData = new SignalData(FirstOperationTab.SignalData.StartTime,
                                                       FirstOperationTab.SignalData.Sampling,
                                                       FirstOperationTab.SignalData.QuantizationSampling)
                {
                    SamplesX = FirstOperationTab.SignalData.SamplesX,
                    SamplesY = SelectedOperation.SignalOperation(FirstOperationTab.SignalData.SamplesY,
                                                            SecondOperationTab.SignalData.SamplesY)
                };

                SelectedTab.SignalData = signalData;
                SelectedTab.IsScattered = true;
                SelectedTab.CalculateSignalInfo(signalData.StartTime,
                                                signalData.StartTime + (signalData.SamplesY.Count / signalData.Sampling));
                SelectedTab.DrawCharts();
            }
        }

        public void Load()
        {
            try
            {
                SelectedTab.LoadDataFromFile(LoadPath(true));
                SelectedTab.DrawCharts(); 
                SelectedTab.CalculateSignalInfo(isDiscrete: true, fromSamples: true);
            }
            catch
            {

            }
        }

        public void Save()
        {
            try
            {
                SelectedTab.SaveDataToFile(LoadPath(false));
            }
            catch
            {

            }
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

        private void Quit()
        {
            // nie działa "lepsze" rozwiązanie :c
            // Application.Current.Shutdown();

            Environment.Exit(0);
        }
    }
}