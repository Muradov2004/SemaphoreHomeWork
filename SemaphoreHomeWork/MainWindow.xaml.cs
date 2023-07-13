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

    public static int placeInSemaphore { get; set; } = 6;

    Semaphore semaphore;

    int ThreadCount = 1;

    public MainWindow()
    {
        InitializeComponent();

        semaphore = new(placeInSemaphore, placeInSemaphore);

        DataContext = this;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        CreatedThreads.Add($"Thread {ThreadCount++}");
    }

    void ThreadMethod(object state)
    {
        var data = state as ThreadData;
        bool st = false;
        Random rnd = new();
        while (!st)
        {
            if (data!.Semaphore.WaitOne(1500))
            {
                try
                {
                    if (WaitingThreads.Contains(data.Name))
                        Dispatcher.Invoke(() => WaitingThreads.Remove(data.Name));

                    Thread.CurrentThread.Name = data.Name;

                    var threadInfo = $"{Thread.CurrentThread.Name} -> {Thread.CurrentThread.ManagedThreadId}";

                    if (!WorkingThreads.Contains(threadInfo))
                        Dispatcher.Invoke(() => WorkingThreads.Add(threadInfo));
                    Thread.Sleep(rnd.Next(2000, 20000));
                }
                finally
                {
                    st = true;

                    var threadInfo = $"{Thread.CurrentThread.Name} -> {Thread.CurrentThread.ManagedThreadId}";

                    if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    {
                        Dispatcher.Invoke(() => WorkingThreads.Remove(threadInfo));
                    }

                    data.Semaphore.Release();
                }
            }
            else
            {
                if (!WaitingThreads.Contains(data.Name))
                    Dispatcher.Invoke(() => WaitingThreads.Add(data.Name!));
            }
        }
    }

    private void CreatedThreadsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

        if (CreatedThreadsListBox.SelectedItem is not null)
        {
            WaitingThreads.Add(CreatedThreadsListBox.SelectedItem.ToString()!);
            ThreadPool.QueueUserWorkItem(ThreadMethod!, new ThreadData(semaphore,
                CreatedThreadsListBox.SelectedItem.ToString()!));
            CreatedThreads.Remove(CreatedThreadsListBox.SelectedItem.ToString()!);
        }
    }

    private void PlaceInSemaphoreNumberBox_TextChanged(object sender, TextChangedEventArgs e) => placeInSemaphore = Convert.ToInt32(PlaceInSemaphoreNumberBox.Value);

    private void WaitingListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WaitingListBox.SelectedItem is not null)
        {
            CreatedThreads.Add(WaitingListBox.SelectedItem.ToString()!);
            WaitingThreads.Remove(WaitingListBox.SelectedItem.ToString()!);
        }

    }
}
