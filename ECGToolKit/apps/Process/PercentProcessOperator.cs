/*
 * author:Joey Zhao
 * date:2010-12-1
 * describe:带百分比的进度条,使用方法：
 * 1, 实例化一个ProcessOperator对象；
 * 2，赋值BackgroundWork（类型为一个参数，无返回值的委托，其中参数是一个具有一个int类型参数无返回值的委托，用来预报进度）属性为要在后台执行的方法，详见TestForm中的示例
 * 3，调用Start方法开始执行
 * 4, 在事件BackgroundWorkerCompleted中执行后台任务完成后的操作
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Process
{
    public class PercentProcessOperator
    {
        private BackgroundWorker _backgroundWorker;//后台线程
        private ProcessForm _processForm;//进度条窗体
        private BackgroundWorkerEventArgs _eventArgs;//异常参数
        private string _inforMessage;

        public PercentProcessOperator()
        {
            _processForm = new ProcessForm();
            _eventArgs = new BackgroundWorkerEventArgs();
            _processForm.ProcessStyle = System.Windows.Forms.ProgressBarStyle.Continuous;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
        }

        //显示进度
        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _processForm.MessageInfo = _inforMessage + ", Percentage finished：" + e.ProgressPercentage.ToString() + "%";
            _processForm.ProcessValue = e.ProgressPercentage;
        }

        //操作进行完毕后关闭进度条窗体
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_processForm.Visible == true)
            {
                _processForm.Close();
            }
            if (this.BackgroundWorkerCompleted != null)
            {
                this.BackgroundWorkerCompleted(null, _eventArgs);
            }
        }

        //后台执行的操作
        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (BackgroundWork != null)
            {
                try
                {
                    BackgroundWork(this.ReportPercent);
                }
                catch (Exception ex)
                {
                    _eventArgs.BackGroundException = ex;
                }
            }
        }

        #region 公共方法、属性、事件

        /// <summary>
        /// 设置进度条显示的提示信息
        /// </summary>
        public string MessageInfo
        {
            set { _inforMessage = value; }
        }

        /// <summary>
        /// <para>后台执行的操作,参数为一个参数为int型的委托；
        /// 在客户端要执行的后台方法中可以使用Action&lt;int&gt;预报完成进度,如：
        /// <example>
        /// <code>
        /// PercentProcessOperator o = new PercentProcessOperator();
        /// o.BackgroundWork = this.DoWord;
        /// 
        /// private void DoWork(Action&lt;int&gt; Report)
        /// {
        ///     Report(0);
        ///     //do soming;
        ///     Report(10);
        ///     //do soming;
        ///     Report(50);
        ///     //do soming
        ///     Report(100);
        /// }
        /// </code>
        /// </example></para>
        /// </summary>
        public Action<Action<int>> BackgroundWork { get; set; }

        /// <summary>
        /// 后台任务执行完毕后事件
        /// </summary>
        public event EventHandler<BackgroundWorkerEventArgs> BackgroundWorkerCompleted;

        /// <summary>
        /// 开始执行
        /// </summary>
        public void Start()
        {
            _backgroundWorker.RunWorkerAsync();
            _processForm.ShowDialog();
        }
        public void Start(object data)
        {
            _backgroundWorker.RunWorkerAsync(data);
            _processForm.ShowDialog();
        }
        #endregion

        //报告完成百分比
        private void ReportPercent(int i)
        {
            if (i >= 0 && i <= 100)
            {
                _backgroundWorker.ReportProgress(i);
            }
        }
    }
}
