using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
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
using System.Diagnostics;

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
        private Timer _timer;
        private DateTime _start;
        private LogWindow _log;
        private int _errors;
        private int _downloaded;
        private CancellationTokenSource _cts;

        public DownloadkWindow(String title, List<Patchlist> patchlist, Management man, DL_TYPE dl = DL_TYPE.DL_ANY)
        {
            InitializeComponent();

            this.Title = title;
            this._max_threads = Environment.ProcessorCount;
            this._patchlist = patchlist;
            this._patchsets = new List<List<Patchlist>>(this._max_threads);
            for (int i = 0; i < this._max_threads; ++i)
            {
                this._patchsets.Add(new List<Patchlist>());
            }
            this._log = new LogWindow();
            this._downloaded = 0;
            this._errors = 0;
            this._errors_label.Content = "";
            this._cts = new CancellationTokenSource();

            if (dl == DL_TYPE.DL_UPDATE && Properties.Settings.Default.patchlist.Length > 100)
            {
                List<String> savedpl = Properties.Settings.Default.patchlist.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<String> strpatchlist = new List<string>();
                foreach (var e in patchlist)
                    strpatchlist.Add(e.ToString);
                var shortpl = strpatchlist.Except(savedpl).ToList();

                this._patchlist.Clear();
                foreach (var e in shortpl)
                {
                    if (!String.IsNullOrEmpty(e))
                    this._patchlist.Add(new Patchlist(e, man));
                }
            }
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
            this._progress.IsIndeterminate = false;
            ParallelOptions para_opt = new ParallelOptions();

            this._patchlist.Sort((a, b) => (a.size.CompareTo(b.size)));
            para_opt.MaxDegreeOfParallelism = this._max_threads;
            para_opt.CancellationToken = this._cts.Token;
            try
            {
                Task.Factory.StartNew(() =>
                {
                    para_opt.CancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        Parallel.ForEach(this._patchlist, para_opt, (patchlist, loopstate) =>
                        {
                            if (loopstate.ShouldExitCurrentIteration || loopstate.IsExceptional)
                                loopstate.Stop();
                            DoDownload(patchlist);
                        });
                    }
                    catch (OperationCanceledException ope)
                    {
                        Trace.WriteLine(ope.Message);
                        Trace.WriteLine(ope.StackTrace);
                    }

                }, this._cts.Token);
            }
            catch (AggregateException ae)
            {
                Trace.WriteLine(ae.Message);
                Trace.WriteLine(ae.StackTrace);
            }
            finally
            {
            }
        }

        private void DoDownload(Patchlist file)
        {
            String path = String.Format(@"{0}\{1}", Arks_SystemTool.Properties.Settings.Default.pso2_path, file.path.Replace("/", @"\"));
            if (!File.Exists(path) || !file.IsSame(path))
            {
                try
                {
                    Trace.WriteLine(String.Format("Getting: {0} ({1}) ", file.filename, file.size));
                    Requests.Download(file.url, path);
                    this._downloaded += 1;
                }
                catch (WebException we)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        String err = DateTime.Now + ": " + we.Message + "\r\n";

                        if (we.Response != null)
                            err += "On: " + we.Response.ResponseUri + "\r\n";
                        else
                            err += "On: " + we + "\r\n";
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
                        this._errors_label.Foreground = Brushes.Red;
                        this._log.AppendLine(err, this._errors);
                    }));
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.AddProgress(1);
            });
        }

        private void AddProgress(int progress)
        {
            this._progress.Value += progress;
            double percent = (this._progress.Value / this._progress.Maximum) * 100f;
            this._taskbar.ProgressState = TaskbarItemProgressState.Normal;
            this._taskbar.ProgressValue = (double) (percent / 100f);
            if (this._downloaded > 0)
                this._count_label.Content = String.Format("{0} (+{1}) / {2} ({3}%)", this._progress.Value, this._downloaded, this._progress.Maximum, percent.ToString("0.00"));
            else
                this._count_label.Content = String.Format("{0} / {1} ({2}%)", this._progress.Value, this._progress.Maximum, percent.ToString("0.00"));
        }

        private void _button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void _Window_Closing(object sender, CancelEventArgs e)
        {
            this._cts.Cancel();
            this._cts.Dispose();
            if (this._errors > 0)
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_error,
                    Arks_SystemTool.Properties.Resources.title_download_error,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
