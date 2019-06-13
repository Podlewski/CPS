using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Numerics;

using Microsoft.Win32;

using Logic;
using System.Diagnostics;

namespace ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Commands

        public ICommand AddTabCommand { get; set; }
        public ICommand GenerateCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public ICommand SamplingReconstructionInfoCommand { get; set; }
        public ICommand ReconstructCommand { get; set; }
        public ICommand CreateFilterCommand { get; set; }
        public ICommand FilterSignalCommand { get; set; }
        public ICommand TransformationCommand { get; set; }
        public ICommand BackwardTransformationCommand { get; set; }
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

        public List<string> FilterList { get; set; }
        public string SelectedFilter { get; set; }

        public List<string> TransformationList { get; set; }
        public string SelectedTransformation { get; set; }

        public List<string> WindowList { get; set; }
        public string SelectedWindow { get; set; }

        public TabViewModel SignalTab { get; set; }
        public TabViewModel FilterTab { get; set; }


        #region Factors

        public double A_Amplitude { get; set; } = 1;
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; } = 4;
        public double T_BasicPeriod { get; set; } = 1;
        public double Kw_DutyCycle { get; set; } = 0.5;
        public double Ts_TimeStep { get; set; } = 2;
        public double P_Probability { get; set; } = 0.5;
        public int Frequency { get; set; } = 1000;
        public int SamplingFrequency { get; set; } = 10;
        public int QuantizationThresholds { get; set; } = 8;
        public int ReconstructionFrequency { get; set; } = 1000;
        public int ReconstructionSamples { get; set; } = 0;
        public int M_FilterRow { get; set; } = 25;
        public double F0_CutOffFrequency { get; set; } = 10;
        public double Fp_SamplingFrequency { get; set; } = 80;

        #endregion

        #endregion

        public MainViewModel()
        {
            TabList = new ObservableCollection<TabViewModel>();
            AddTab();
            SelectedTab = TabList[0];
            FirstOperationTab = TabList[0];
            SecondOperationTab = TabList[0];
            SignalTab = TabList[0];
            FilterTab = TabList[0];

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
                "5) Splot",
                "6) Korelacja (bezpośrednia)",
                "7) Korelacja (przez Splot)"

            };
            SelectedOperation = OperationList[0];

            FilterList = new List<string>()
            {
                "1) Filtr dolnoprzepustowy",
                "2) Filtr pasmowy",
                "3) Filtr górnoprzepustowy"

            };
            SelectedFilter = FilterList[0];

            WindowList = new List<string>()
            {
                "1) Okno prostokątne",
                "2) Okno Hamminga",
                "3) Okno Hanninga",
                "4) Okno Blackmana"
            };
            SelectedWindow = WindowList[0];

            TransformationList = new List<string>()
            {
                "1) Dyskretna transformacja Fouriera",
                "2) Szybka transformacja Fouriera z decymacją w dziedzinie czasu",
                "3) Trasnfromacja falkowa (DB8)"

            };
            SelectedTransformation = TransformationList[0];

            AddTabCommand = new RelayCommand(AddTab);
            GenerateCommand = new RelayCommand(Generate);
            ComputeCommand = new RelayCommand(Compute);
            SamplingReconstructionInfoCommand = new RelayCommand(SamplingReconstructionInfo);
            ReconstructCommand = new RelayCommand(Reconstruct);
            CreateFilterCommand = new RelayCommand(CreateFilter);
            FilterSignalCommand = new RelayCommand(FilterSignal);
            TransformationCommand = new RelayCommand(Transformation);
            BackwardTransformationCommand = new RelayCommand(BackwardTransformation);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            QuitCommand = new RelayCommand(Quit);
        }

        public void AddTab()
        {
            TabList.Add(new TabViewModel(TabList.Count));
            SelectedTab = TabList[TabList.Count - 1];
            OnPropertyChanged(nameof(SelectedTab));
        }

        public void Generate()
        {
            Generator generator = new Generator()
            {
                A = A_Amplitude,
                T1 = T1_StartTime,
                T = T_BasicPeriod,
                Kw = Kw_DutyCycle,
                Ts = Ts_TimeStep,
                P = P_Probability
            };

            Func<double, double> selectedGeneration = generator.SelectGenerator(SelectedSignal);

            if (selectedGeneration != null)
            {
                SignalData signalData = new SignalData(T1_StartTime, Frequency, SamplingFrequency);

                for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)Frequency)
                {
                    signalData.SamplesX.Add((double)i);
                    signalData.SamplesY.Add(selectedGeneration((double)i));
                }

                if (Frequency % SamplingFrequency == 0)
                {
                    // dla sygnałów generowanych losowo -> dzięki temu próbki są identyczne 
                    for(int i = 0; i < signalData.SamplesX.Count; i += Frequency/SamplingFrequency)
                    {
                        signalData.ConversionSamplesX.Add(signalData.SamplesX[i]);
                        signalData.ConversionSamplesY.Add(signalData.SamplesY[i]);
                    }
                }
                else
                {
                    for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)SamplingFrequency)
                    {
                        signalData.ConversionSamplesX.Add((double)i);
                        signalData.ConversionSamplesY.Add(selectedGeneration((double)i));
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
            if (FirstOperationTab.SignalData.IsNotEmpty() && SecondOperationTab.SignalData.IsNotEmpty())
            {
                if (SelectedOperation == "5) Splot" || SelectedOperation == "6) Korelacja (bezpośrednia)" ||
                    SelectedOperation == "7) Korelacja (przez Splot)")
                {
                    List<double> XSamples = new List<double>();

                    int FirstTabSamplesCounter = FirstOperationTab.SignalData.ConversionSamplesX.Count;
                    int SecondTabSamplesCounter = SecondOperationTab.SignalData.ConversionSamplesX.Count;

                    for (int i = 0; i < FirstTabSamplesCounter + SecondTabSamplesCounter - 1; i++)
                        XSamples.Add(i);

                    SignalData signalData = new SignalData(0)
                    {
                        ConversionSamplesX = XSamples,
                        ConversionSamplesY = SelectedOperation.SignalOperation(FirstOperationTab.SignalData.ConversionSamplesY,
                                                                SecondOperationTab.SignalData.ConversionSamplesY)
                    };

                    SelectedTab.SignalData = signalData;
                    SelectedTab.IsScattered = true;
                    SelectedTab.DrawCharts();
                }

                else
                {
                    string message = "";

                    if (SecondOperationTab.SignalData.IsInvalid(FirstOperationTab.SignalData, message))
                    {
                        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    SignalData signalData = new SignalData(FirstOperationTab.SignalData.StartTime,
                                                           FirstOperationTab.SignalData.Sampling,
                                                           FirstOperationTab.SignalData.ConversionSampling)
                    {
                        ConversionSamplesX = FirstOperationTab.SignalData.ConversionSamplesX,
                        ConversionSamplesY = SelectedOperation.SignalOperation(FirstOperationTab.SignalData.ConversionSamplesY,
                                                                SecondOperationTab.SignalData.ConversionSamplesY)
                    };

                    SelectedTab.SignalData = signalData;
                    SelectedTab.IsScattered = true;
                    SelectedTab.CalculateSignalInfo(signalData.StartTime,
                                                    signalData.StartTime + (signalData.ConversionSamplesY.Count / signalData.ConversionSampling));
                    SelectedTab.DrawCharts();
                }
            }
        }

        public void Reconstruct()
        {
            SignalData signalData = SelectedTab.SignalData;

            Generator generator = new Generator()
            {   T1 = signalData.StartTime };

            double step = 2 * A_Amplitude / (Math.Pow(2, QuantizationThresholds) - 1);
            List<double> steps = new List<double>
                {
                    -A_Amplitude
                };

            for (int i = 1; i < Math.Pow(2, QuantizationThresholds); i++)
            {
                steps.Add(steps[i - 1] + step);
            }

            if (Frequency % SamplingFrequency == 0)
            {
                // dla sygnałów generowanych losowo -> dzięki temu próbki są identyczne 
                for (int i = 0; i < signalData.SamplesX.Count; i += Frequency / SamplingFrequency)
                {
                    double diff = Math.Abs(signalData.SamplesY[i] - steps[0]);
                    int iterator = 0;

                    for (int j = 0; j < Math.Pow(2, QuantizationThresholds); j++)
                    {
                        if (Math.Abs(signalData.SamplesY[i] - steps[j]) <= diff)
                        {
                            diff = Math.Abs(signalData.SamplesY[i] - steps[j]);
                            iterator = j;
                        }
                    }

                    signalData.QuantizationSamplesY.Add(steps[iterator]);
                }
            }
            else
            {
                for (int i = 0; i < signalData.ConversionSamplesX.Count; i++)
                {
                    double diff = signalData.SamplesX.LastOrDefault();
                    int iterator = 0;

                    for (int j = 0; j < Math.Pow(2, QuantizationThresholds); j++)
                    {
                        if (Math.Abs(signalData.ConversionSamplesY[i] - steps[j]) <= diff)
                        {
                            diff = Math.Abs(signalData.ConversionSamplesY[i] - steps[j]);
                            iterator = j;
                        }
                    }

                    signalData.QuantizationSamplesY.Add(steps[iterator]);
                }
            }

            for (decimal i = (decimal)T1_StartTime; i < (decimal)(T1_StartTime + D_DurationOfTheSignal); i += 1 / (decimal)ReconstructionFrequency)
            {
                signalData.ReconstructionSamplesX.Add((double)i);
                signalData.ReconstructionSamplesY.Add(generator.SincReconstruction(signalData.ConversionSamplesX, signalData.QuantizationSamplesY,
                                                                                   (double)i, SamplingFrequency, ReconstructionSamples));
            }

            SelectedTab.SignalData = signalData;
            SelectedTab.IsScattered = SelectedSignal.IsGenerationScattered();
            SelectedTab.CalculateSignalInfo(T1_StartTime, T1_StartTime + D_DurationOfTheSignal);
            SelectedTab.CalculateReconstructionInfo();
            SelectedTab.ReconstructCharts();
        }

        public void CreateFilter()
        {
            List<double> XSamples = new List<double>();
            List<double> YSamples = SelectedFilter.FilterOperation(M_FilterRow, F0_CutOffFrequency, Fp_SamplingFrequency);
            YSamples = SelectedWindow.WindowOperation(YSamples, M_FilterRow);

            for (int i = 0; i < YSamples.Count; i++)
                XSamples.Add(i);

            SignalData signalData = new SignalData(0)
            {
                ConversionSamplesX = XSamples,
                ConversionSamplesY = YSamples
            };

            SelectedTab.SignalData = signalData;
            SelectedTab.IsScattered = true;
            SelectedTab.DrawCharts();
        }

        public void FilterSignal()
        {
            if (SignalTab.SignalData.IsNotEmpty() && FilterTab.SignalData.IsNotEmpty())
            {
                List<double> XSamples = new List<double>();
                List<double> YSamples = new List<double>();

                if (SignalTab.SignalData.QuantizationSamplesY.Count != 0 && FilterTab.SignalData.QuantizationSamplesY.Count != 0)
                    YSamples = Operations.ConvoluteSignals(SignalTab.SignalData.QuantizationSamplesY,
                                                           FilterTab.SignalData.QuantizationSamplesY)
                                         .Skip((FilterTab.SignalData.QuantizationSamplesY.Count - 1) / 2)
                                         .Take(SignalTab.SignalData.QuantizationSamplesY.Count).ToList();

                else if (SignalTab.SignalData.QuantizationSamplesY.Count != 0 && FilterTab.SignalData.QuantizationSamplesY.Count == 0)
                    YSamples = Operations.ConvoluteSignals(SignalTab.SignalData.QuantizationSamplesY,
                                                           FilterTab.SignalData.SamplesY)
                                         .Skip((FilterTab.SignalData.SamplesY.Count - 1) / 2)
                                         .Take(SignalTab.SignalData.QuantizationSamplesY.Count).ToList();

                else if (SignalTab.SignalData.QuantizationSamplesY.Count == 0 && FilterTab.SignalData.QuantizationSamplesY.Count != 0)
                    YSamples = Operations.ConvoluteSignals(SignalTab.SignalData.SamplesY,
                                                           FilterTab.SignalData.QuantizationSamplesY)
                                         .Skip((FilterTab.SignalData.QuantizationSamplesY.Count - 1) / 2)
                                         .Take(SignalTab.SignalData.SamplesY.Count).ToList();

                else
                    YSamples = Operations.ConvoluteSignals(SignalTab.SignalData.SamplesY,
                                                           FilterTab.SignalData.SamplesY)
                                         .Skip((FilterTab.SignalData.SamplesY.Count - 1) / 2)
                                         .Take(SignalTab.SignalData.SamplesY.Count).ToList();

                for (int i = 0; i < YSamples.Count; i++)
                    XSamples.Add(i);

                SignalData signalData = new SignalData(0)
                {
                    ConversionSamplesX = XSamples,
                    ConversionSamplesY = YSamples
                };

                SelectedTab.SignalData = signalData;
                SelectedTab.IsScattered = true;
                SelectedTab.DrawCharts();
            }
        }

        public void SamplingReconstructionInfo()
        {
            MessageBox.Show("Wpisz 0 aby skorzystać ze wszystkich próbek", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Transformation()
        {
            Stopwatch timer = new Stopwatch();

            try
            {
                timer.Start();
                SelectedTab.SignalData.ComplexSamples = SelectedTransformation.TransformOperation(SelectedTab.SignalData.ConversionSamplesY);
                timer.Stop();
                SelectedTab.SetTransformationTime(timer.Elapsed.TotalSeconds);
                SelectedTab.DrawW();
            }
            catch (ArgumentException)
            {
                SelectedTab.SetTransformationTime(0);
                MessageBox.Show("Liczba próbek musi być potęgą dwójki", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void BackwardTransformation()
        {
            SelectedTab.SignalData.ConversionSamplesX = FirstOperationTab.SignalData.ConversionSamplesX;

            try
            {
                SelectedTab.SignalData.ConversionSamplesY = SelectedTransformation.TransformBackwardOperation(FirstOperationTab.SignalData.ComplexSamples);
                SelectedTab.IsScattered = true;
                SelectedTab.DrawCharts();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Wybrana karta nie została poddana wcześniej transformacie", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Liczba próbek musi być potęgą dwójki", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Load()
        {
            try
            {
                SelectedTab.LoadDataFromFile(LoadPath(true));
                SelectedTab.DrawCharts(); 
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