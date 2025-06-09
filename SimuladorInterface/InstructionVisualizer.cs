using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para visualização da instrução atual e sua representação em hexadecimal
    public class InstructionVisualizer
    {
        // Componentes da interface
        private Grid _mainGrid;
        private StackPanel _instructionPanel;
        private GroupBox _instructionGroupBox;
        private TextBlock _pcText;
        private TextBlock _currentInstructionText;
        private TextBlock _hexRepresentationText;
        private TextBlock _assemblyRepresentationText;

        // Construtor
        public InstructionVisualizer(Grid mainGrid)
        {
            _mainGrid = mainGrid;
            InitializeComponents();
        }

        // Inicializa os componentes da interface
        private void InitializeComponents()
        {
            // Cria o GroupBox para a instrução
            _instructionGroupBox = new GroupBox
            {
                Header = "Instrução Atual",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 300
            };

            // Cria o painel para as informações da instrução
            _instructionPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            // Cria os TextBlocks para exibir as informações
            _pcText = new TextBlock
            {
                Text = "PC: 0x00000000",
                Margin = new Thickness(0, 5, 0, 5),
                FontFamily = new FontFamily("Consolas"),
                FontWeight = FontWeights.Bold
            };

            _currentInstructionText = new TextBlock
            {
                Text = "Instrução: 0x00000000",
                Margin = new Thickness(0, 5, 0, 5),
                FontFamily = new FontFamily("Consolas")
            };

            _hexRepresentationText = new TextBlock
            {
                Text = "Representação Hex: 0x00000000",
                Margin = new Thickness(0, 5, 0, 5),
                FontFamily = new FontFamily("Consolas")
            };

            _assemblyRepresentationText = new TextBlock
            {
                Text = "Assembly: ",
                Margin = new Thickness(0, 5, 0, 5),
                FontFamily = new FontFamily("Consolas"),
                TextWrapping = TextWrapping.Wrap
            };

            // Adiciona os TextBlocks ao painel
            _instructionPanel.Children.Add(_pcText);
            _instructionPanel.Children.Add(_currentInstructionText);
            _instructionPanel.Children.Add(_hexRepresentationText);
            _instructionPanel.Children.Add(_assemblyRepresentationText);

            // Adiciona o painel ao GroupBox
            _instructionGroupBox.Content = _instructionPanel;

            // Adiciona o GroupBox ao Grid principal
            _mainGrid.Children.Add(_instructionGroupBox);

            // Posiciona o GroupBox no Grid
            Grid.SetRow(_instructionGroupBox, 3);
            Grid.SetColumn(_instructionGroupBox, 1);
        }

        // Atualiza a visualização da instrução com o estado atual
        public void Update(SimulationState state)
        {
            _pcText.Text = $"PC: 0x{state.PC:X8}";
            _currentInstructionText.Text = $"Instrução: {state.CurrentInstruction}";
            _hexRepresentationText.Text = $"Representação Hex: {state.CurrentInstructionHex}";
            _assemblyRepresentationText.Text = $"Assembly: {state.CurrentInstructionAssembly}";
        }
    }
}
