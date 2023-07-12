using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace SemaphoreHomeWork;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : UiWindow
{
    public ObservableCollection<string> CreatedThreads { get; set; } = new();
    public ObservableCollection<string> WaitingThreads { get; set; } = new();
    public ObservableCollection<string> WorkingThreads { get; set; } = new();
    static int placeInSemaphore = 1;
    Semaphore semaphore = new(placeInSemaphore, placeInSemaphore);
    int ThreadCount = 1;

    public MainWindow()
    {
        InitializeComponent();

        placeInSemaphore = Convert.ToInt32(PlaceInSemaphoreNumberBox.Value);

        DataContext = this;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        CreatedThreads.Add($"Thread {ThreadCount++}");
    }

    void ThreadMethod(object state)
    {
        var s = state as Semaphore;
        bool st = false;
        Random rnd = new();
        while (!st)
        {
            if (s!.WaitOne(1500))
            {
                try
                {
                    WorkingThreads.Add($"{Thread.CurrentThread.Name} ->{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(rnd.Next(2000, 5000));
                }
                finally
                {
                    st = true;
                    s.Release();
                }
            }
            else
            {
                WaitingThreads.Add(Thread.CurrentThread.Name!);
            }
        }

    }

    private void CreatedThreadsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

        if (CreatedThreadsListBox.SelectedItem is not null)
        {
            WaitingThreads.Add(CreatedThreadsListBox.SelectedItem.ToString()!);
            CreatedThreads.Remove(CreatedThreadsListBox.SelectedItem.ToString()!);
        }
    }
}
