using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for FilecheckWindow.xaml
    /// </summary>
    public partial class DownloadkWindow : Window
    {
        private List<Patchlist> _patchlist;
        private List<List<Patchlist>> _patchsets;
        private int _max_threads;
        private BackgroundWorker[] _bworkers;
        private Timer _timer;
        private DateTime _start;

        public DownloadkWindow(List<Patchlist> patchlist = null)
        {
            InitializeComponent();
            this._patchlist = patchlist;
            this._max_threads = 4;
            this._patchsets = new List<List<Patchlist>>(this._max_threads);
            for (int i = 0; i < this._max_threads; ++i)
            {
                this._patchsets.Add(new List<Patchlist>());
            }
            this._bworkers = new BackgroundWorker[this._max_threads];
        }

        private void _ElapsedTimer(object state)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TimeSpan ts = DateTime.Now - this._start;
                this._timer_label.Content = ts.ToString(@"hh\:mm\:ss");
            }));
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._start = DateTime.Now;
            this._timer = new Timer(_ElapsedTimer, null, 0, 100);
            this._progress.Value = 0;
            this._progress.Maximum = this._patchlist.Count;
            double percent = (this._progress.Value / this._progress.Maximum) * 100f;
            this._count_label.Content = String.Format("{0} / {1} ({2}%)", this._progress.Value, this._progress.Maximum, percent.ToString("0.00"));

            for (int i = 0; i < this._patchlist.Count; ++i)
            {
                this._patchsets[i % this._max_threads].Add(this._patchlist[i]);
            }
            this._progress.IsIndeterminate = false;
            for (int i = 0; i < this._max_threads; ++i)
            {
                this._bworkers[i] = new BackgroundWorker();
                this._bworkers[i].DoWork += DownloadkWindow_DoWork;
                this._bworkers[i].WorkerReportsProgress = true;
                this._bworkers[i].WorkerSupportsCancellation = true;
                this._bworkers[i].ProgressChanged += DownloadkWindow_ProgressChanged;
                this._bworkers[i].RunWorkerAsync(this._patchsets[i]);
            }
        }

        private void DownloadkWindow_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this._progress.Value += e.ProgressPercentage;
            double percent = (this._progress.Value / this._progress.Maximum) * 100f;
            this._count_label.Content = String.Format("{0} / {1} ({2}%)", this._progress.Value, this._progress.Maximum, percent.ToString("0.00"));
        }

        private void DownloadkWindow_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Patchlist> patchlist = e.Argument as List<Patchlist>;
            BackgroundWorker worker = sender as BackgroundWorker;

#if DEBUG
            Console.WriteLine("Starting thread to treat {0} files", patchlist.Count);
#endif
            foreach (var elem in patchlist)
            {
                String path = String.Format(@"{0}\{1}", Arks_SystemTool.Properties.Settings.Default.pso2_path, elem.path.Replace("/", @"\"));

#if DEBUG
                Console.WriteLine(elem.url);
                Console.WriteLine(path);
#endif
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (!File.Exists(path) || !elem.IsSame(path))
                {
#if DEBUG
                    Console.WriteLine(path);
                    Console.WriteLine(elem.url);
                    Console.WriteLine("Missmatch!");
#endif
                    Requests.Download(elem.url, path);
                }
#if DEBUG
                else
                {
                    Console.WriteLine("We have a match!"); 
                }
#endif
                worker.ReportProgress(1);
                //Thread.Sleep(700);
            }
        }

        private void _button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            // Cancel threads!!!!!
            this.Close();
        }

        private void _Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var worker in this._bworkers)
            {
                if (worker.IsBusy)
                    worker.CancelAsync();
                worker.Dispose();
            }
            this._timer.Dispose();
        }

        private void _progress_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue >= this._progress.Maximum)
            {
                this._progress.Value += 1;
                this.IsEnabled = false;
                MessageBox.Show("Download finished!", "Congrats");
                this.IsEnabled = true;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
