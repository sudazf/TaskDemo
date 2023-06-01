using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
		/// Task 
		/// </summary>
		private Task task;

		/// <summary>
		/// Thread
		/// </summary>
		private Thread thread;

		/// <summary>
		/// ManualResetEvent
		/// </summary>
		private ManualResetEvent manualResetEvent;





		/// <summary>
		/// ManualResetEvent
		/// </summary>
		private EventWaitHandle eventWaitHandle;

		/// <summary>
		/// 进度值
		/// </summary>
		private int taskProgress;

		/// <summary>
		/// 进度条颜色索引
		/// </summary>
		private int taskProgressColorIndex;

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
			get { return taskId; }
			set
			{
				taskId = value;
				OnPropertyChanged("TaskId");
			}
		}

		/// <summary>
		/// Task
		/// </summary>
		public Task Task
		{
			get { return task; }
			set { task = value; }
		}

		/// <summary>
		/// Thread
		/// </summary>
		public Thread Thread
		{
			get { return thread; }
			set { thread = value; }
		}

		/// <summary>
		/// ManualResetEvent
		/// </summary>
		public ManualResetEvent ManualResetEvent
		{
			get { return manualResetEvent; }
			set { manualResetEvent = value; }
		}

		/// <summary>
		/// 进度值
		/// </summary>
		public int TaskProgress
		{
			get { return taskProgress; }
			set
			{
				taskProgress = value;
				OnPropertyChanged("TaskProgress");
			}
		}

		/// <summary>
		/// 进度条颜色索引
		/// </summary>
		public int TaskProgressColorIndex
		{
			get { return taskProgressColorIndex; }
			set { taskProgressColorIndex = value; }
		}

		#region 按钮状态

		/// <summary>
		/// 可开始
		/// </summary>
		public bool CanStart
		{
			get { return canStart; }
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
			get { return canStop; }
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
			get { return canSuspend; }
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
			get { return canResume; }
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
					Thread = new Thread(new ParameterizedThreadStart(StartByThread));
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
