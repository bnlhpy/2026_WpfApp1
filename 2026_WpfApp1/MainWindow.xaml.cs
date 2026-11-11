using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace _2026_WpfApp1
{
    // 主視窗
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenViewer_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer viewer = new MyDocumentViewer();
            viewer.Owner = this;
            viewer.Show();
        }
    }

    // 副視窗 (MyDocumentViewer)
    public class MyDocumentViewer : Window
    {
        private RichTextBox rtbEditor;
        private StatusBarItem txtStatus;
        private ComboBox cmbFont;
        private ComboBox cmbSize;
        private ComboBox cmbColor;

        public MyDocumentViewer()
        {
            Title = "MyDocumentViewer";
            Height = 450;
            Width = 700;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitUI();
        }

        private void InitUI()
        {
            DockPanel dock = new DockPanel();

            ToolBarTray toolBarTray = new ToolBarTray
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6DBBF"))
            };
            DockPanel.SetDock(toolBarTray, Dock.Top);

            // 1. 檔案操作工具列 (還原範例圖示)
            ToolBar fileToolBar = new ToolBar();

            Button btnNew = new Button { ToolTip = "新建" };
            btnNew.Content = CreateIcon("New");
            btnNew.Click += (s, e) => { rtbEditor.Document.Blocks.Clear(); txtStatus.Content = ""; };

            Button btnOpen = new Button { ToolTip = "開啟" };
            btnOpen.Content = CreateIcon("Open");
            btnOpen.Click += OnOpenClick;

            Button btnSave = new Button { ToolTip = "儲存" };
            btnSave.Content = CreateIcon("Save");
            btnSave.Click += OnSaveClick;

            Button btnClear = new Button { ToolTip = "清除" };
            btnClear.Content = CreateIcon("Clear");
            btnClear.Click += (s, e) => { rtbEditor.Document.Blocks.Clear(); txtStatus.Content = ""; };

            fileToolBar.Items.Add(btnNew);
            fileToolBar.Items.Add(btnOpen);
            fileToolBar.Items.Add(btnSave);
            fileToolBar.Items.Add(btnClear);

            // 2. 格式工具列 (還原大字體 B I U 與選單)
            ToolBar formatToolBar = new ToolBar();

            ToggleButton btnBold = new ToggleButton { Command = EditingCommands.ToggleBold, ToolTip = "粗體" };
            btnBold.Content = new TextBlock { Text = "B", FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Georgia"), FontSize = 22, Margin = new Thickness(2, -2, 2, -2) };

            ToggleButton btnItalic = new ToggleButton { Command = EditingCommands.ToggleItalic, ToolTip = "斜體" };
            btnItalic.Content = new TextBlock { Text = "I", FontStyle = FontStyles.Italic, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Georgia"), FontSize = 22, Margin = new Thickness(2, -2, 2, -2) };

            ToggleButton btnUnderline = new ToggleButton { Command = EditingCommands.ToggleUnderline, ToolTip = "底線" };
            btnUnderline.Content = new TextBlock { Text = "U", TextDecorations = TextDecorations.Underline, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Georgia"), FontSize = 22, Margin = new Thickness(2, -2, 2, -2) };

            cmbFont = new ComboBox { Width = 140, ItemsSource = Fonts.SystemFontFamilies };
            cmbFont.SelectionChanged += OnFontChanged;

            cmbSize = new ComboBox { Width = 50 };
            int[] sizes = { 9, 10, 12, 14, 16, 18, 24, 36, 48 };
            foreach (int sz in sizes)
            {
                cmbSize.Items.Add(new ComboBoxItem { Content = sz.ToString() });
            }
            cmbSize.SelectionChanged += OnSizeChanged;

            cmbColor = new ComboBox { Width = 50 };
            cmbColor.Items.Add(new ComboBoxItem { Content = "黑色", Tag = "Black" });
            cmbColor.Items.Add(new ComboBoxItem { Content = "紅色", Tag = "Red" });
            cmbColor.Items.Add(new ComboBoxItem { Content = "藍色", Tag = "Blue" });
            cmbColor.Items.Add(new ComboBoxItem { Content = "綠色", Tag = "Green" });
            cmbColor.SelectionChanged += OnColorChanged;

            formatToolBar.Items.Add(btnBold);
            formatToolBar.Items.Add(btnItalic);
            formatToolBar.Items.Add(btnUnderline);
            formatToolBar.Items.Add(cmbFont);
            formatToolBar.Items.Add(cmbSize);
            formatToolBar.Items.Add(cmbColor);

            toolBarTray.ToolBars.Add(fileToolBar);
            toolBarTray.ToolBars.Add(formatToolBar);

            // 3. 底部狀態列 (空白純灰色區塊)
            StatusBar statusBar = new StatusBar { Height = 25, Background = Brushes.LightGray };
            DockPanel.SetDock(statusBar, Dock.Bottom);
            txtStatus = new StatusBarItem { Content = "" };
            statusBar.Items.Add(txtStatus);

            // 4. 編輯區
            rtbEditor = new RichTextBox
            {
                AcceptsTab = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            dock.Children.Add(toolBarTray);
            dock.Children.Add(statusBar);
            dock.Children.Add(rtbEditor);

            Content = dock;
        }

        private UIElement CreateIcon(string type)
        {
            Canvas canvas = new Canvas { Width = 26, Height = 26 };
            if (type == "New")
            {
                System.Windows.Shapes.Path page = new System.Windows.Shapes.Path
                {
                    Data = Geometry.Parse("M 4,2 L 18,2 L 22,6 L 22,24 L 4,24 Z M 17,2 L 17,7 L 22,7"),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                TextBlock plus = new TextBlock { Text = "+", FontSize = 14, FontWeight = FontWeights.Bold, Margin = new Thickness(7, 5, 0, 0) };
                canvas.Children.Add(page);
                canvas.Children.Add(plus);
            }
            else if (type == "Open")
            {
                System.Windows.Shapes.Path folder = new System.Windows.Shapes.Path
                {
                    Data = Geometry.Parse("M 2,6 L 9,6 L 12,9 L 24,9 L 24,22 L 2,22 Z"),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Add(folder);
            }
            else if (type == "Save")
            {
                System.Windows.Shapes.Path disk = new System.Windows.Shapes.Path
                {
                    Data = Geometry.Parse("M 3,3 L 21,3 L 23,5 L 23,23 L 3,23 Z M 6,3 L 6,10 L 18,10 L 18,3 M 7,23 L 7,14 L 17,14 L 17,23"),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Add(disk);
            }
            else if (type == "Clear")
            {
                System.Windows.Shapes.Path trash = new System.Windows.Shapes.Path
                {
                    Data = Geometry.Parse("M 5,7 L 21,7 M 8,7 L 8,4 L 18,4 L 18,7 M 7,7 L 8,22 L 18,22 L 19,7 M 11,10 L 11,19 M 15,10 L 15,19"),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Add(trash);
            }
            return canvas;
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "RTF檔 (*.rtf)|*.rtf|純文字檔 (*.txt)|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open))
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLower();
                    range.Load(fs, ext == ".rtf" ? DataFormats.Rtf : DataFormats.Text);
                }
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog { Filter = "RTF檔 (*.rtf)|*.rtf|純文字檔 (*.txt)|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLower();
                    range.Save(fs, ext == ".rtf" ? DataFormats.Rtf : DataFormats.Text);
                }
            }
        }

        private void OnFontChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFont.SelectedItem != null && !rtbEditor.Selection.IsEmpty)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFont.SelectedItem);
        }

        private void OnSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSize.SelectedItem is ComboBoxItem item && !rtbEditor.Selection.IsEmpty)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, double.Parse(item.Content.ToString()));
        }

        private void OnColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColor.SelectedItem is ComboBoxItem item && !rtbEditor.Selection.IsEmpty)
            {
                Brush brush = (Brush)new BrushConverter().ConvertFromString(item.Tag.ToString());
                rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
            }
        }
    }
}