using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para visualização do clock e tempo de execução
    public class ClockVisualizer
    {
        // Componentes da interface
        private Grid _mainGrid;
        private StackPanel _clockPanel;
        private GroupBox _clockGroupBox;
        private TextBlock _clockFrequencyText;
        private TextBlock _elapsedCyclesText;
        private TextBlock _executionTimeText;
        private TextBlock _typeRCountText;
        private TextBlock _typeICountText;
        private TextBlock _typeJCountText;

        // Construtor
        public ClockVisualizer(Grid mainGrid)
        {
            _mainGrid = mainGrid;
            InitializeComponents();
        }

        // Inicializa os componentes da interface
        private void InitializeComponents()
        {
            // Cria o GroupBox para o clock
            _clockGroupBox = new GroupBox
            {
                Header = "Clock e Estatísticas",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 300
            };

            // Cria o painel para as informações do clock
            _clockPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            // Cria os TextBlocks para exibir as informações
            _clockFrequencyText = new TextBlock
            {
                Text = "Frequência do Clock: 1 MHz",
                Margin = new Thickness(0, 5, 0, 5)
            };

            _elapsedCyclesText = new TextBlock
            {
                Text = "Ciclos de Clock: 0",
                Margin = new Thickness(0, 5, 0, 5)
            };

            _executionTimeText = new TextBlock
            {
                Text = "Tempo de Execução: 0.0000 s",
                Margin = new Thickness(0, 5, 0, 5)
            };

            _typeRCountText = new TextBlock
            {
                Text = "Instruções Tipo R: 0",
                Margin = new Thickness(0, 5, 0, 5)
            };

            _typeICountText = new TextBlock
            {
                Text = "Instruções Tipo I: 0",
                Margin = new Thickness(0, 5, 0, 5)
            };

            _typeJCountText = new TextBlock
            {
                Text = "Instruções Tipo J: 0",
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Adiciona os TextBlocks ao painel
            _clockPanel.Children.Add(_clockFrequencyText);
            _clockPanel.Children.Add(_elapsedCyclesText);
            _clockPanel.Children.Add(_executionTimeText);
            _clockPanel.Children.Add(_typeRCountText);
            _clockPanel.Children.Add(_typeICountText);
            _clockPanel.Children.Add(_typeJCountText);

            // Adiciona o painel ao GroupBox
            _clockGroupBox.Content = _clockPanel;

            // Adiciona o GroupBox ao Grid principal
            _mainGrid.Children.Add(_clockGroupBox);

            // Posiciona o GroupBox no Grid
            Grid.SetRow(_clockGroupBox, 3);
            Grid.SetColumn(_clockGroupBox, 0);
        }

        // Atualiza a visualização do clock com o estado atual
        public void Update(SimulationState state)
        {
            _elapsedCyclesText.Text = $"Ciclos de Clock: {state.ElapsedClockCycles}";
            _executionTimeText.Text = $"Tempo de Execução: {state.TotalExecutionTime:F4} s";
            _typeRCountText.Text = $"Instruções Tipo R: {state.TypeRInstructionsExecuted}";
            _typeICountText.Text = $"Instruções Tipo I: {state.TypeIInstructionsExecuted}";
            _typeJCountText.Text = $"Instruções Tipo J: {state.TypeJInstructionsExecuted}";
        }

        // Atualiza a frequência do clock
        public void UpdateClockFrequency(int frequency)
        {
            string frequencyText;

            if (frequency >= 1000000)
            {
                frequencyText = $"{frequency / 1000000} MHz";
            }
            else if (frequency >= 1000)
            {
                frequencyText = $"{frequency / 1000} kHz";
            }
            else
            {
                frequencyText = $"{frequency} Hz";
            }

            _clockFrequencyText.Text = $"Frequência do Clock: {frequencyText}";
        }
    }
}