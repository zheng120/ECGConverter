/*
 * author:Joey Zhao
 * date:2010-11-30 
 * describe:进度条，该类可以在后台线程处理一些费时操作，并显示进度条，进度条并未真实显示数据进度
 * 仅仅是告诉用户程序还活着，有待加强。使用方法：
 * 1, 实例化一个ProcessOperator对象；
 * 2，赋值BackgroundWork（类型为一个无参数无返回值的委托）属性为要在后台执行的方法（无参数无返回值的方法）
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
    public class ProcessOperator
    {
        private BackgroundWorker _backgroundWorker;//后台线程
        private ProcessForm _processForm;//进度条窗体
        private BackgroundWorkerEventArgs _eventArgs;//异常参数

        public ProcessOperator()
        {
            _backgroundWorker = new BackgroundWorker();
            _processForm = new ProcessForm();
            _eventArgs = new BackgroundWorkerEventArgs();
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
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
                    BackgroundWork();
                }
                catch (Exception ex)
                {
                    _eventArgs.BackGroundException = ex;
                }
            }
        }

        #region 公共方法、属性、事件

        /// <summary>
        /// 后台执行的操作
        /// </summary>
        public Action BackgroundWork { get; set; }

        /// <summary>
        /// 设置进度条显示的提示信息
        /// </summary>
        public string MessageInfo
        {
            set { _processForm.MessageInfo = value; }
        }

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
    }

    public class BackgroundWorkerEventArgs : EventArgs
    {
        /// <summary>
        /// 后台程序运行时抛出的异常
        /// </summary>
        public Exception BackGroundException { get; set; }
    }
}
