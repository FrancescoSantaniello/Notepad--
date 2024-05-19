using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Text;
using Notepad__.Models;
namespace Notepad__;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Width = SystemParameters.PrimaryScreenWidth / 2;
        Height = SystemParameters.PrimaryScreenHeight / 2;

        labelTime.Text = DateTime.Now.ToString();

        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (sender, e) =>
        {
            labelTime.Text = DateTime.Now.ToString();
        };
        timer.Start();
    }

    private Document document = new Document();
    private List<Run> tasks = new();
    public void AddTask(Run process)
    {
        if (process is null)
            throw new ArgumentException("Processo non valido");

        tasks.Add(process);

        MenuItem newItem = new MenuItem();
        newItem.Header = process.Key;
        

        newItem.Click += (sender, e) =>
        {
            Run? task = FindTaskFromKey(newItem.Header.ToString());
            if (task != null)
                task.Start();
        };

        menuItemEsegui.Items.Add(newItem);
    }
    public Run? FindTaskFromKey(string key) 
    {
        if (string.IsNullOrEmpty(key))
            return null;

        return tasks.FirstOrDefault((task) => task.Key == key);
    }
    public void RemoveTaskFromKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        for(int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].Key == key)
            {
                tasks.RemoveAt(i);
                menuItemEsegui.Items.RemoveAt(i+4);
                return;
            }
        }
    }
    private void checkBoxStatusBar_Checked(object sender, EventArgs e)
    {
        if (statusBar is not null)
            statusBar.Visibility = Visibility.Visible;
    }
    private void checkBoxStatusBar_Unchecked(object sender, EventArgs e)
    {
        if (statusBar is not null)
            statusBar.Visibility = Visibility.Collapsed;
    }
    private void buttonZoomAvanti_Click(object sender, EventArgs e)
    {
        try
        {
            textBox.FontSize++;
            labelZoom.Text = int.Parse(labelZoom.Text.Replace("%", "")) + 10 + "%";
        }
        catch (Exception) { }
    }
    private void buttonZoomIndietro_Click(object sender, EventArgs e)
    {
        try
        {
            if (textBox.FontSize > 0)
            {
                textBox.FontSize--;
                labelZoom.Text = int.Parse(labelZoom.Text.Replace("%", "")) - 10 + "%";
            }
        }
        catch (Exception) { }
    }
    private void buttonZoomRestore_Click(object sender, EventArgs e)
    {
        textBox.FontSize = 16;
        labelZoom.Text = "100%";
    }
    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            buttonDataOra_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.OemPlus ||
            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Add)
        {
            buttonZoomAvanti_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.OemMinus ||
            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Subtract)
        {
            buttonZoomIndietro_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.D0 ||
            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.NumPad0)
        {
            buttonZoomRestore_Click(sender,e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.F12)
        {
            buttonApri_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.E)
        {
            buttonCercaConGoogle_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.N)
        {
            buttonNuovoFile_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.N &&
            (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        {
            buttonNuovaFinestra_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.S &&
            (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        {
            buttonSalvaConNome_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.S)
        {
            buttonSalva_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.T &&
            (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        {
            buttonPowerShell_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.T)
        {
            buttonPromptDeiComandi_Click(sender, e);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.R)
        {
            buttonEsegui_Click(sender, e);
            return;
        }
    }
    private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            if (e.Delta >= 0)
                buttonZoomAvanti_Click(sender,e);
            else
                buttonZoomIndietro_Click(sender, e);
        }
    }     
    private void buttonEsci_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void buttonNuovoFile_Click(object sender, EventArgs e)
    {
        try
        {
            if (document.IsWrited())
            {
                MessageBoxResult result = MessageBox.Show("Salvare le modifiche apportate?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes)
                    document.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            document = new Document();
            Title = "* Nuovo file - Notepad--";
            textBox.Clear();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void buttonNuovaFinestra_Click(object sender, EventArgs e)
    {
        new MainWindow().Show();
    }
    private void textBox_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;
    }

    private void textBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;
    }

    private void textBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            try
            {
                string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

                if (File.Exists(file))
                {
                    if (document.IsWrited())
                    {
                        MessageBoxResult result = MessageBox.Show("Salvare le modifiche apportate?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                        if (result == MessageBoxResult.Yes)
                            document.Save();
                        else if (result == MessageBoxResult.Cancel)
                            return;
                    }

                    document = new Document(file);
                    textBox.Text = document.Contenuto;
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private void textBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }
    private void textBox_TextChanged(object sender, EventArgs e)
    {
        document.Contenuto = textBox.Text;
    }
    private void textBox_KeyDown(object sender, EventArgs e)
    {
        if (document.Nome is null)
            Title = "* Nuovo file - Notepad--";
        else
        {
            if (document.IsWrited())
                Title = "* ";
            else
                Title = "";

            Title += document.Nome + " - Notepad--";
        }
    }
    private void buttonApri_Click(object sender, EventArgs e)
    {
        try
        {
            if (document.IsWrited())
            {
                MessageBoxResult result = MessageBox.Show("Salvare le modifiche apportate?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes)
                    document.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "File di testo | *.txt | File | *.*";
            
            bool? resultOpenFileDialog = openFileDialog.ShowDialog();
            if (resultOpenFileDialog == true)
            {
                document = new Document(openFileDialog.FileName);

                textBox.Text = document.Contenuto;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void buttonSalva_Click(object sender, EventArgs e)
    {
        try
        {
            if (document.Percorso is null)
                buttonSalvaConNome_Click(sender, e);
            else
                document.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        try
        {
            if (document.IsWrited())
            {
                MessageBoxResult result = MessageBox.Show("Salvare le modifiche apportate?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes)
                    document.Save();
                else if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;                    
            }
            else if (document.Percorso is null)
            {
                MessageBoxResult result = MessageBox.Show("Salvare le modifiche apportate?", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes)
                    buttonSalvaConNome_Click(null, null);
                else if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void buttonSalvaConNome_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = document.Percorso;
            saveFileDialog.Filter = "File di testo | *.txt | File | *.*";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                document.Percorso = saveFileDialog.FileName;
                document.Save();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void buttonCercaConGoogle_Click(object sender , EventArgs e)
    {
        try
        {
            string selectedText = textBox.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.Arguments = $"/c start https://www.google.com/search?q={selectedText.Replace(" ","+")}&sca_esv=faf3bf626897cb5c&sxsrf=ACQVn0-aOb1hUcpG4OGV7npzly2npxoOXA%3A1713611009014&source=hp&ei=AKEjZuX9OtqK9u8PpPO_2A4&iflsig=ANes7DEAAAAAZiOvEQCqXeHQYOjS7-IhOYsLWuTrMtnm&ved=0ahUKEwjl_eO70tCFAxVahf0HHaT5D-sQ4dUDCA8&uact=5&oq=cerca&gs_lp=Egdnd3Mtd2l6IgVjZXJjYTIPECMYgAQYJxiKBRhGGPkBMggQABiABBixAzIOEAAYgAQYsQMYkgMYuAQyCxAAGIAEGJIDGIoFMgsQABiABBixAxiDATILEAAYgAQYsQMYyQMyCBAAGIAEGLEDMggQABiABBixAzIIEAAYgAQYsQMyBRAAGIAESNoEUKkBWI8EcAF4AJABAJgBaKAB7gKqAQMzLjG4AQPIAQD4AQGYAgWgApkDqAIKwgIHECMYJxjqAsICChAjGIAEGCcYigXCAg4QABiABBixAxiDARiKBcICERAuGIAEGLEDGNEDGIMBGMcBwgIOEC4YgAQYsQMYgwEYigXCAgQQIxgnwgIIEC4YgAQYsQPCAgsQLhiABBixAxiDAZgDEpIHAzMuMqAHxi0&sclient=gws-wiz\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;

                p.Start();
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void buttonSelezionaTutto_Click(object sender, EventArgs e)
    {
        textBox.SelectAll();
    }
    private void buttonDataOra_Click(object sender, EventArgs e)
    {
        textBox.SelectedText = DateTime.Now.ToString();
    }

    private void buttonPowerShell_Click(object sender, EventArgs e)
    {
        Process.Start("powershell");
    }

    private void buttonPromptDeiComandi_Click(object sender, EventArgs e)
    {
        Process.Start("cmd","/k cd %userprofile%");
    }

    private void buttonEsegui_Click(object sender, RoutedEventArgs e)
    {
        new EseguiWindow(this).ShowDialog();
    }

    private void radioButtonASCII_Checked(object sender, RoutedEventArgs e)
    {
        if (textBox is not null && labelCodifica is not null)
        {
            textBox.Encoding = Encoding.ASCII;
            labelCodifica.Text = "ASCII";
        }
    }

    private void radioButtonUTF8_Checked(object sender, RoutedEventArgs e)
    {
        if(textBox is not null && labelCodifica is not null)
        {
            textBox.Encoding = Encoding.UTF8;
            labelCodifica.Text = "UTF-8";
        }
    }

    private void radioButtonUTF32_Checked(object sender, RoutedEventArgs e)
    {
        if (textBox is not null && labelCodifica is not null)
        {
            textBox.Encoding = Encoding.UTF32;
            labelCodifica.Text = "UTF-32";
        }
    }

    private void radioButtonUnicode_Checked(object sender, RoutedEventArgs e)
    {
        if (textBox is not null && labelCodifica is not null)
        {
            textBox.Encoding = Encoding.Unicode;
            labelCodifica.Text = "Unicode";
        }
    }
}