using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SimuladorComum;

namespace SimuladorInterface.ViewModels
{
    // ViewModel para conectar a lógica com a interface
    public class SimulatorViewModel : INotifyPropertyChanged
    {
        // Referência para o simulador
        private ISimulator _simulator;

        // Estado atual da simulação
        private SimulationState _currentState;

        // Propriedades
        private string _assemblyCode;
        private int _clockFrequency;
        private int _typeRCycles;
        private int _typeICycles;
        private int _typeJCycles;
        private bool _isRunning;

        // Comandos
        public ICommand LoadCommand { get; private set; }
        public ICommand RunCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StepCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        // Evento de notificação de mudança de propriedade
        public event PropertyChangedEventHandler PropertyChanged;

        // Construtor
        public SimulatorViewModel(ISimulator simulator)
        {
            _simulator = simulator;
            _currentState = new SimulationState();

            // Inicializa as propriedades
            _assemblyCode = "";
            _clockFrequency = 1000000; // 1 MHz
            _typeRCycles = 1;
            _typeICycles = 1;
            _typeJCycles = 1;
            _isRunning = false;

            // Inicializa os comandos
            LoadCommand = new RelayCommand(LoadProgram, CanLoadProgram);
            RunCommand = new RelayCommand(RunSimulation, CanRunSimulation);
            PauseCommand = new RelayCommand(PauseSimulation, CanPauseSimulation);
            StepCommand = new RelayCommand(StepSimulation, CanStepSimulation);
            ResetCommand = new RelayCommand(ResetSimulation, CanResetSimulation);

            // Registra os manipuladores de eventos
            _simulator.SimulationStateChanged += OnSimulationStateChanged;
            _simulator.InstructionExecuted += OnInstructionExecuted;
            _simulator.SimulationCompleted += OnSimulationCompleted;
            _simulator.SimulationError += OnSimulationError;
        }

        // Propriedades com notificação de mudança
        public string AssemblyCode
        {
            get { return _assemblyCode; }
            set
            {
                if (_assemblyCode != value)
                {
                    _assemblyCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ClockFrequency
        {
            get { return _clockFrequency; }
            set
            {
                if (_clockFrequency != value)
                {
                    _clockFrequency = value;
                    _simulator.ClockSpeed = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int TypeRCycles
        {
            get { return _typeRCycles; }
            set
            {
                if (_typeRCycles != value)
                {
                    _typeRCycles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int TypeICycles
        {
            get { return _typeICycles; }
            set
            {
                if (_typeICycles != value)
                {
                    _typeICycles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int TypeJCycles
        {
            get { return _typeJCycles; }
            set
            {
                if (_typeJCycles != value)
                {
                    _typeJCycles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SimulationState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                NotifyPropertyChanged();
            }
        }

        // Métodos de comando
        private void LoadProgram(object parameter)
        {
            _simulator.LoadProgram(AssemblyCode);
            _simulator.SetClockConfiguration(ClockFrequency, TypeRCycles, TypeICycles, TypeJCycles);
        }

        private bool CanLoadProgram(object parameter)
        {
            return !string.IsNullOrWhiteSpace(AssemblyCode) && !IsRunning;
        }

        private void RunSimulation(object parameter)
        {
            _simulator.Run();
            IsRunning = true;
        }

        private bool CanRunSimulation(object parameter)
        {
            return !IsRunning && CurrentState != null;
        }

        private void PauseSimulation(object parameter)
        {
            _simulator.Pause();
            IsRunning = false;
        }

        private bool CanPauseSimulation(object parameter)
        {
            return IsRunning;
        }

        private void StepSimulation(object parameter)
        {
            _simulator.Step();
        }

        private bool CanStepSimulation(object parameter)
        {
            return !IsRunning && CurrentState != null;
        }

        private void ResetSimulation(object parameter)
        {
            _simulator.Reset();
            IsRunning = false;
        }

        private bool CanResetSimulation(object parameter)
        {
            return CurrentState != null;
        }

        // Manipuladores de eventos
        private void OnSimulationStateChanged(object sender, SimulationEventArgs e)
        {
            CurrentState = e.State;
            IsRunning = _simulator.IsRunning;
        }

        private void OnInstructionExecuted(object sender, SimulationEventArgs e)
        {
            CurrentState = e.State;
        }

        private void OnSimulationCompleted(object sender, EventArgs e)
        {
            IsRunning = false;
        }

        private void OnSimulationError(object sender, SimulationEventArgs e)
        {
            IsRunning = false;
        }

        // Método para notificar mudança de propriedade
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Classe auxiliar para implementar ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
