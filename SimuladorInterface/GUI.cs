using System;
using System.Windows;
using System.Windows.Controls;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para interface gráfica do usuário
    public class GUI
    {
        // Referência para o simulador
        private ISimulator _simulator;

        // Componentes da interface
        private MemoryVisualizer _memoryVisualizer;
        private RegisterVisualizer _registerVisualizer;
        private SimulationControls _simulationControls;
        private ClockVisualizer _clockVisualizer;
        private InstructionVisualizer _instructionVisualizer;

        // Construtor
        public GUI(ISimulator simulator, Grid mainGrid)
        {
            _simulator = simulator;

            // Inicializa os componentes da interface
            _memoryVisualizer = new MemoryVisualizer(mainGrid);
            _registerVisualizer = new RegisterVisualizer(mainGrid);
            _simulationControls = new SimulationControls(mainGrid, simulator);
            _clockVisualizer = new ClockVisualizer(mainGrid);
            _instructionVisualizer = new InstructionVisualizer(mainGrid);

            // Registra os manipuladores de eventos
            _simulator.SimulationStateChanged += OnSimulationStateChanged;
            _simulator.InstructionExecuted += OnInstructionExecuted;
            _simulator.SimulationCompleted += OnSimulationCompleted;
            _simulator.SimulationError += OnSimulationError;
        }

        // Atualiza a interface com o estado atual da simulação
        public void UpdateInterface(SimulationState state)
        {
            _memoryVisualizer.Update(state);
            _registerVisualizer.Update(state);
            _clockVisualizer.Update(state);
            _instructionVisualizer.Update(state);
        }

        // Manipuladores de eventos
        private void OnSimulationStateChanged(object sender, SimulationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateInterface(e.State);
            });
        }

        private void OnInstructionExecuted(object sender, SimulationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateInterface(e.State);
            });
        }

        private void OnSimulationCompleted(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Simulação concluída!", "Simulador MIPS", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void OnSimulationError(object sender, SimulationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Erro: {e.Message}", "Simulador MIPS", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
}