using System;
using System.Collections.Generic;
using System.Threading;
using SimuladorComum;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe que controla a simulação
    /// </summary>
    public class Simulator : ISimulator
    {
        // Componentes da simulação
        private CPU _cpu;
        private Parser _parser;
        private ClockConfiguration _clockConfig;
        private ExecutionStatistics _statistics;
        private FileLoader _fileLoader;

        // Estado da simulação
        private bool _isRunning;
        private Thread _simulationThread;
        private SimulationState _currentState;

        // Eventos
        public event EventHandler<SimulationEventArgs> SimulationStateChanged;
        public event EventHandler<SimulationEventArgs> InstructionExecuted;
        public event EventHandler SimulationCompleted;
        public event EventHandler<SimulationEventArgs> SimulationError;

        // Propriedades
        public SimulationState CurrentState => _currentState;
        public bool IsRunning => _isRunning;
        public int ClockSpeed { get; set; }
        public int TypeRInstructionsCount => _statistics.TypeRInstructionsExecuted;
        public int TypeIInstructionsCount => _statistics.TypeIInstructionsExecuted;
        public int TypeJInstructionsCount => _statistics.TypeJInstructionsExecuted;

        // Construtor
        public Simulator()
        {
            _cpu = new CPU();
            _parser = new Parser();
            _clockConfig = new ClockConfiguration();
            _statistics = new ExecutionStatistics();
            _fileLoader = new FileLoader();
            _isRunning = false;
            _currentState = new SimulationState();
            ClockSpeed = 1000000; // 1 MHz padrão
        }

        // Carrega um programa a partir de código assembly
        public void LoadProgram(string assemblyCode)
        {
            try
            {
                List<uint> machineCode = _parser.ParseString(assemblyCode);
                byte[] program = ConvertToByteArray(machineCode);
                _cpu.LoadProgram(program);
                _cpu.Initialize();
                _statistics.Reset();
                UpdateState();

                OnSimulationStateChanged("Programa carregado com sucesso", SimulationEventType.InstructionFetch);
            }
            catch (Exception ex)
            {
                OnSimulationError($"Erro ao carregar programa: {ex.Message}", SimulationEventType.Error);
            }
        }

        // Configura o clock da CPU
        public void SetClockConfiguration(int clockSpeed, int typeRInstructions, int typeIInstructions, int typeJInstructions)
        {
            ClockSpeed = clockSpeed;
            _clockConfig = new ClockConfiguration(clockSpeed, typeRInstructions, typeIInstructions, typeJInstructions);
            _cpu.SetClockConfiguration(_clockConfig);

            OnSimulationStateChanged("Configuração de clock atualizada", SimulationEventType.ClockTick);
        }

        // Executa um ciclo da simulação
        public bool Step()
        {
            if (_isRunning)
                return false;

            try
            {
                bool continueExecution = _cpu.ExecuteCycle();
                UpdateState();

                // Atualiza estatísticas
                _statistics.IncrementClockCycles(1);

                // Determina o tipo da instrução atual e incrementa o contador correspondente
                uint instruction = _currentState.CurrentInstruction;
                int opcode = (int)((instruction >> 26) & 0x3F);

                if (opcode == 0)
                {
                    _statistics.IncrementTypeRInstructions();
                }
                else if (opcode == 0x02 || opcode == 0x03)
                {
                    _statistics.IncrementTypeJInstructions();
                }
                else
                {
                    _statistics.IncrementTypeIInstructions();
                }

                // Calcula o tempo de execução
                _statistics.UpdateExecutionTime(_clockConfig);

                OnInstructionExecuted("Instrução executada", SimulationEventType.InstructionExecute);

                if (!continueExecution)
                {
                    OnSimulationCompleted();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnSimulationError($"Erro ao executar instrução: {ex.Message}", SimulationEventType.Error);
                return false;
            }
        }

        // Inicia a execução contínua da simulação
        public void Run()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            // Inicia a simulação em uma thread separada
            _simulationThread = new Thread(() =>
            {
                try
                {
                    while (_isRunning)
                    {
                        bool continueExecution = Step();

                        if (!continueExecution)
                        {
                            _isRunning = false;
                            break;
                        }

                        // Pausa para simular a frequência do clock
                        int delayMs = 1000 / ClockSpeed;
                        if (delayMs > 0)
                            Thread.Sleep(delayMs);
                    }
                }
                catch (Exception ex)
                {
                    OnSimulationError($"Erro na execução da simulação: {ex.Message}", SimulationEventType.Error);
                    _isRunning = false;
                }
            });

            _simulationThread.Start();

            OnSimulationStateChanged("Simulação iniciada", SimulationEventType.ClockTick);
        }

        // Pausa a execução da simulação
        public void Pause()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            // Aguarda a thread de simulação terminar
            if (_simulationThread != null && _simulationThread.IsAlive)
            {
                _simulationThread.Join(1000);
            }

            OnSimulationStateChanged("Simulação pausada", SimulationEventType.ClockTick);
        }

        // Reinicia a simulação
        public void Reset()
        {
            Pause();
            _cpu.Reset();
            _statistics.Reset();
            UpdateState();

            OnSimulationStateChanged("Simulação reiniciada", SimulationEventType.ClockTick);
        }

        // Converte uma lista de instruções para um array de bytes
        private byte[] ConvertToByteArray(List<uint> instructions)
        {
            byte[] result = new byte[instructions.Count * 4]; // 4 bytes por instrução

            for (int i = 0; i < instructions.Count; i++)
            {
                uint instruction = instructions[i];

                result[i * 4] = (byte)(instruction & 0xFF);
                result[i * 4 + 1] = (byte)((instruction >> 8) & 0xFF);
                result[i * 4 + 2] = (byte)((instruction >> 16) & 0xFF);
                result[i * 4 + 3] = (byte)((instruction >> 24) & 0xFF);
            }

            return result;
        }

        // Atualiza o estado atual da simulação
        private void UpdateState()
        {
            _currentState = _cpu.GetState();
            _currentState.ElapsedClockCycles = _statistics.ClockCycles;
            _currentState.TotalExecutionTime = _statistics.TotalExecutionTime;
            _currentState.TypeRInstructionsExecuted = _statistics.TypeRInstructionsExecuted;
            _currentState.TypeIInstructionsExecuted = _statistics.TypeIInstructionsExecuted;
            _currentState.TypeJInstructionsExecuted = _statistics.TypeJInstructionsExecuted;
        }

        // Obtém a representação hexadecimal de uma instrução
        public string GetInstructionHexRepresentation(uint instruction)
        {
            return $"0x{instruction:X8}";
        }

        // Métodos para disparar eventos
        protected virtual void OnSimulationStateChanged(string message, SimulationEventType eventType)
        {
            SimulationStateChanged?.Invoke(this, new SimulationEventArgs(_currentState, message, eventType));
        }

        protected virtual void OnInstructionExecuted(string message, SimulationEventType eventType)
        {
            InstructionExecuted?.Invoke(this, new SimulationEventArgs(_currentState, message, eventType));
        }

        protected virtual void OnSimulationCompleted()
        {
            SimulationCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSimulationError(string message, SimulationEventType eventType)
        {
            SimulationError?.Invoke(this, new SimulationEventArgs(_currentState, message, eventType));
        }
    }
}