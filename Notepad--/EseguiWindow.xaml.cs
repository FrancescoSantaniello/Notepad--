using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;
using Notepad__.Models;
namespace Notepad__;
public partial class EseguiWindow : Window
{
    public EseguiWindow(MainWindow _win)
    {
        InitializeComponent();

        win = _win;
    }
    private MainWindow win { get; init; }
    private void buttonScegliProgramma_Click(object sender, EventArgs e)
    {
        OpenFileDialog op = new OpenFileDialog();
        op.Title = "Scegli programma da eseguire";
        op.Filter = "Programma | *.exe | *.cmd | *.bat | *.com | *.*";

        bool? dialog = op.ShowDialog();
        if (dialog == true)
            textBoxProgramma.Text = op.FileName;
    }

    private void buttonCancella_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void MessageBoxInfo(string msg)
    {
        MessageBox.Show(msg, Title, MessageBoxButton.OK,MessageBoxImage.Asterisk);
    }
    private void MessageBoxError(string msg)
    {
        MessageBox.Show(msg, Title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    private void buttonEsegui_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(textBoxKey.Text))
        {
            MessageBoxInfo("Inserire un nome univoco al processo");
            return;
        }

        if (string.IsNullOrEmpty(textBoxProgramma.Text))
        {
            MessageBoxInfo("Inserire un programma da eseguire");
            return;
        }
        
        if (string.IsNullOrEmpty(textBoxProgramma.Text))
        {
            MessageBoxError($"Inserire un programma da eseguire");
            textBoxProgramma.Clear();
            return;
        }

        if (checkBoxMemorizza.IsChecked == true)
        {
            if (win.FindTaskFromKey(textBoxKey.Text) != null)
            {
                switch (MessageBox.Show($"Esiste già un processo precedentemente salvato con la chiave '{textBoxKey.Text}'\nCancellare il processo precedenemente salvato?", Title, MessageBoxButton.YesNo, MessageBoxImage.Asterisk))
                {
                    case MessageBoxResult.Yes:
                        win.RemoveTaskFromKey(textBoxKey.Text);
                        break;

                    case MessageBoxResult.No:
                        ExecuteProcess(false);
                        Close();
                        return;
                }
            }
        }

        ExecuteProcess(checkBoxMemorizza.IsChecked == true);
        Close();
    }
    private void ExecuteProcess(bool addToTaskList = true)
    {
        try
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = textBoxProgramma.Text;

            if (!string.IsNullOrEmpty(textBoxArgomenti.Text))
                info.Arguments = textBoxArgomenti.Text;

            Run p = new Run(textBoxKey.Text) { StartInfo = info };
            p.Start();
            
            if (addToTaskList)
                win.AddTask(p);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
