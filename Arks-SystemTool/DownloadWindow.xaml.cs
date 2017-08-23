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
using System.Net;

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
        private LogWindow _log;
        private int _errors;
        private int _downloaded;

        public DownloadkWindow(String title, List<Patchlist> patchlist = null)
        {
            InitializeComponent();
            this.Title = title;
            //this._max_threads = 4;
            this._max_threads = 2;
            //this._max_threads = Environment.ProcessorCount;
            this._patchlist = patchlist;
            this._patchsets = new List<List<Patchlist>>(this._max_threads);
            for (int i = 0; i < this._max_threads; ++i)
            {
                this._patchsets.Add(new List<Patchlist>());
            }
            this._bworkers = new BackgroundWorker[this._max_threads];
            this._log = new LogWindow();
            this._downloaded = 0;
            this._errors = 0;
            this._errors_label.Content = "";
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
            this._progress.Value += 1;
            this._progress.Value += e.ProgressPercentage;
            this._downloaded += e.ProgressPercentage;
            double percent = (this._progress.Value / this._progress.Maximum) * 100f;
            if (this._downloaded > 0)
                this._count_label.Content = String.Format("{0} (+{1}) / {2} ({3}%)", this._progress.Value, this._downloaded, this._progress.Maximum, percent.ToString("0.00"));
            else
                this._count_label.Content = String.Format("{0} / {1} ({2}%)", this._progress.Value, this._progress.Maximum, percent.ToString("0.00"));
        }

        private void DownloadkWindow_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Patchlist> patchlist = e.Argument as List<Patchlist>;
            BackgroundWorker worker = sender as BackgroundWorker;

            foreach (var elem in patchlist)
            {
                bool downloaded = false;
                String path = String.Format(@"{0}\{1}", Arks_SystemTool.Properties.Settings.Default.pso2_path, elem.path.Replace("/", @"\"));

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (!File.Exists(path) || !elem.IsSame(path))
                {
                    try
                    {
                        Requests.Download(elem.url, path);
                        downloaded = true;
                    }
                    catch (WebException we)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            String err = DateTime.Now + ": " + we.Message + "\r\n";

                            err += "On: " + we.Response.ResponseUri + "\r\n";
                            err += "For: " + path.Replace(@"\\", @"\") + "\r\n";
                            err += we.StackTrace;
                            this._errors += 1;

                            if (!Application.Current.Windows.OfType<LogWindow>().Any())
                                this._log = new LogWindow();
                            if (!this._log.IsVisible)
                            {
                                this._log.Show();
                                this._log.Focus();
                            }
                            this._errors_label.Content = this._errors;
                            this._log.AppendLine(err, this._errors);
                        }));
                    }
                }
                worker.ReportProgress(downloaded ? 1 : 0);
            }
        }

        private void _button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void _Window_Closing(object sender, CancelEventArgs e)
        {
            if (this._errors > 0)
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_error,
                    Arks_SystemTool.Properties.Resources.title_download_error,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            foreach (var worker in this._bworkers)
            {
                if (worker.IsBusy)
                    worker.CancelAsync();
                worker.Dispose();
            }
            this._timer.Dispose();
            //if (this._log.IsVisible || Application.Current.Windows.OfType<LogWindow>().Any())
            this._log.Close();
        }

        private void _progress_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue >= this._progress.Maximum)
            {
                this._progress.Value += 1;
                this.IsEnabled = false;
                this._timer.Dispose();
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_completed,
                    Arks_SystemTool.Properties.Resources.title_download_completed,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                this.IsEnabled = true;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
