using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskDemo.ViewModel.Base;
using VbotFramework.Delagate;

namespace TaskDemo.Model
{
	public class JTask: ViewModelBase
	{
		#region Field

		/// <summary>
		/// 序号
		/// </summary>
		private int taskId;


        /// <summary>
		/// ManualResetEvent
		/// </summary>
		private EventWaitHandle eventWaitHandle;

		/// <summary>
		/// 进度值
		/// </summary>
		private int taskProgress;

        #region Task 控制

		/// <summary>
		/// CancellationTokenSource
		/// </summary>
		private CancellationTokenSource cts;

		/// <summary>
		/// CancellationToken
		/// </summary>
		private CancellationToken ct;

		#endregion

		#region 按钮状态

		/// <summary>
		/// 可开始
		/// </summary>
		private bool canStart = true;

		/// <summary>
		/// 可结束
		/// </summary>
		private bool canStop = false;

		/// <summary>
		/// 可暂停
		/// </summary>
		private bool canSuspend = false;

		/// <summary>
		/// 可继续
		/// </summary>
		private bool canResume = false;


		#endregion

		#endregion

		#region Property

		/// <summary>
		/// 序号
		/// </summary>
		public int TaskId
		{
			get => taskId;
            set
			{
				taskId = value;
				OnPropertyChanged("TaskId");
			}
		}

        /// <summary>
        /// Task 
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// Thread
        /// </summary>
        public Thread Thread { get; set; }

        /// <summary>
        /// ManualResetEvent
        /// </summary>
        public ManualResetEvent ManualResetEvent { get; set; }

        /// <summary>
		/// 进度
		/// </summary>
		public int TaskProgress
		{
			get => taskProgress;
            set
			{
                taskProgress = value;

                if (DispatcherHelper.MainDispatcher.CheckAccess())
                {
                    OnPropertyChanged("TaskProgress");
                }
                else
                {
                    DispatcherHelper.MainDispatcher.Invoke(() =>
                    {
                        OnPropertyChanged("TaskProgress");
                    });
                }
			}
		}

        /// <summary>
        /// 进度条颜色索引
        /// </summary>
        public int TaskProgressColorIndex { get; set; }

        #region 按钮状态

		/// <summary>
		/// 可开始
		/// </summary>
		public bool CanStart
		{
			get => canStart;
            set
			{
				canStart = value;
				OnPropertyChanged("CanStart");
			}
		}

		/// <summary>
		/// 可结束
		/// </summary>
		public bool CanStop
		{
			get => canStop;
            set
			{
				canStop = value;
				OnPropertyChanged("CanStop");
			}
		}

		/// <summary>
		/// 可暂停
		/// </summary>
		public bool CanSuspend
		{
			get => canSuspend;
            set
			{
				canSuspend = value;
				OnPropertyChanged("CanSuspend");
			}
		}

		/// <summary>
		/// 可继续
		/// </summary>
		public bool CanResume
		{
			get => canResume;
            set
			{
				canResume = value;
				OnPropertyChanged("CanResume");
			}
		}

		#endregion

		#region Commands

		/// <summary>
		/// 开始 (by thread)
		/// </summary>
		public DelegateCommand<object> StartByThreadCommand
		{
			get
			{
				return new DelegateCommand<object>((t) => {
					CanStart = false;
					CanStop = true;
					CanSuspend = true;
					CanResume = false;
					abortThread = false;

					ManualResetEvent = new ManualResetEvent(false);
					Thread = new Thread(StartByThread);
					Thread.Start(ManualResetEvent);
				});
			}
		}

		bool abortThread = false;
		private void StartByThread(object para)
		{
			ManualResetEvent mre = (ManualResetEvent)para;
			var waits = new List<EventWaitHandle>();
			waits.Add(mre);
			for (int i = 1; i < 101; i++)
			{
				while (CanResume)
				{
					Thread.Sleep(5);
					if (abortThread)
					{
						break;
					}
				}
				TaskProgress = i;
				Thread.Sleep(100);
				if (abortThread)
				{
					break;
				}
			}
			mre.Set();
			if (abortThread)
			{
				TaskProgress = 0;
			}
			CanStart = true;
			CanStop = false;
			CanSuspend = false;
			CanResume = false;
		}

		/// <summary>
		/// 开始
		/// </summary>
		public DelegateCommand<object> StartCommand
		{
			get
			{
				return new DelegateCommand<object>((t) => {
					cts = new CancellationTokenSource();
					ct = cts.Token;
					CanStart = false;
					CanStop = true;
					CanSuspend = true;
					CanResume = false;
					Task = new Task(() => {
						try
						{
							for (int i = 1; i < 101; i++)
							{
								while (CanResume)
								{
									Thread.Sleep(5);
									ct.ThrowIfCancellationRequested();
								}
								TaskProgress = i;
								Thread.Sleep(100);
								ct.ThrowIfCancellationRequested();
							}
							CanStart = true;
							CanStop = false;
							CanSuspend = false;
							CanResume = false;
						}
						catch (Exception)
						{
							TaskProgress = 0;
							CanStart = true;
							CanStop = false;
							CanSuspend = false;
							CanResume = false;
						}
					}, ct);
					Task.Start();
				});
			}
		}


		/// <summary>
		/// 暂停
		/// </summary>
		public DelegateCommand<object> SuspendCommand
		{
			get
			{
				return new DelegateCommand<object>((t) => {
					CanSuspend = false;
					if (TaskProgress < 100)
					{
						CanResume = true;
					}
				});
			}
		}

		/// <summary>
		/// 继续
		/// </summary>
		public DelegateCommand<object> ResumeCommand
		{
			get
			{
				return new DelegateCommand<object>((t) => {
					CanResume = false;
					if (TaskProgress < 100)
					{
						CanSuspend = true;
					}
				});
			}
		}

		/// <summary>
		/// 停止
		/// </summary>
		public DelegateCommand<object> StopCommand
		{
			get
			{
				return new DelegateCommand<object>((t) => {
					try
					{
						cts.Cancel();
						abortThread = true;
					}
					catch (Exception)
					{
						abortThread = true;
					}
				});
			}
		}


		#endregion

		#endregion
	}
}
