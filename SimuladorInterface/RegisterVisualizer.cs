using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para visualização do estado dos registradores
    public class RegisterVisualizer
    {
        // Componentes da interface
        private Grid _mainGrid;
        private DataGrid _registerGrid;
        private GroupBox _registerGroupBox;

        // Nomes dos registradores MIPS
        private static readonly string[] RegisterNames = new string[]
        {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra"
        };

        // Construtor
        public RegisterVisualizer(Grid mainGrid)
        {
            _mainGrid = mainGrid;
            InitializeComponents();
        }

        // Inicializa os componentes da interface
        private void InitializeComponents()
        {
            // Cria o GroupBox para os registradores
            _registerGroupBox = new GroupBox
            {
                Header = "Registradores",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 300
            };

            // Cria o DataGrid para exibir os registradores
            _registerGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                CanUserResizeRows = false,
                CanUserSortColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.All,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            // Adiciona as colunas ao DataGrid
            DataGridTextColumn indexColumn = new DataGridTextColumn
            {
                Header = "Índice",
                Binding = new System.Windows.Data.Binding("Index"),
                Width = 50
            };

            DataGridTextColumn nameColumn = new DataGridTextColumn
            {
                Header = "Nome",
                Binding = new System.Windows.Data.Binding("Name"),
                Width = 50
            };

            DataGridTextColumn hexValueColumn = new DataGridTextColumn
            {
                Header = "Valor (Hex)",
                Binding = new System.Windows.Data.Binding("HexValue"),
                Width = 100
            };

            DataGridTextColumn decValueColumn = new DataGridTextColumn
            {
                Header = "Valor (Dec)",
                Binding = new System.Windows.Data.Binding("DecValue"),
                Width = 80
            };

            _registerGrid.Columns.Add(indexColumn);
            _registerGrid.Columns.Add(nameColumn);
            _registerGrid.Columns.Add(hexValueColumn);
            _registerGrid.Columns.Add(decValueColumn);

            // Adiciona o DataGrid ao GroupBox
            _registerGroupBox.Content = _registerGrid;

            // Adiciona o GroupBox ao Grid principal
            _mainGrid.Children.Add(_registerGroupBox);

            // Posiciona o GroupBox no Grid
            Grid.SetRow(_registerGroupBox, 1);
            Grid.SetColumn(_registerGroupBox, 1);
            Grid.SetRowSpan(_registerGroupBox, 2);
        }

        // Atualiza a visualização dos registradores com o estado atual
        public void Update(SimulationState state)
        {
            // Limpa o DataGrid
            _registerGrid.Items.Clear();

            // Adiciona os valores dos registradores ao DataGrid
            for (int i = 0; i < 32; i++)
            {
                _registerGrid.Items.Add(new RegisterItem
                {
                    Index = i,
                    Name = RegisterNames[i],
                    HexValue = $"0x{state.Registers[i]:X8}",
                    DecValue = state.Registers[i].ToString()
                });
            }
        }

        // Classe para representar um registrador no DataGrid
        private class RegisterItem
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string HexValue { get; set; }
            public string DecValue { get; set; }
        }
    }
}