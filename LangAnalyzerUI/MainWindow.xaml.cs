using LangAnalyzerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TreeContainer;

namespace LangAnalyzerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            treeContainer.Clear();

            try
            {
                var analyzer = new CodeAnalyzer();
                var rootNode = analyzer.Analyze(textBox.Text);

                DrawComponentTree(rootNode);

            }
            catch (Exception ex)
            {
                string messageBoxText = ex.Message;
                string caption = "Ошибка";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

              
            }

        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            treeContainer.Children.Clear();
            textBox.Clear();
        }

        private void DrawComponentTree(AstNode node, TreeNode tnControl = null)
        {
            TreeNode tnSubtreeRoot;
            Button btn = new Button();
            btn.Content = node.Value;

            if (tnControl == null)
            {
                tnSubtreeRoot = treeContainer.AddRoot(btn);
            }
            else
            {
                tnSubtreeRoot = treeContainer.AddNode(btn, tnControl);
            }


            foreach (var childNode in node.Childs)
            {
                DrawComponentTree(childNode, tnSubtreeRoot);
            }



        }
    }
}
