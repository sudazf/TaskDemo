using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaskDemo.Model;
using TaskDemo.ViewModel.Base;
using VbotFramework.Delagate;

namespace TaskDemo.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Ctor

		public MainWindowViewModel()
		{
			Tasks = new ObservableCollection<JTask>();
			new Task(() => {
				while (true)
				{
					int result = 0;
					for (int i = 0; i < Tasks.Count; i++)
					{
						if (Tasks[i].TaskProgress == 100)
						{
							result++;
						}
					}

					if (result == Tasks.Count && result > 0)
					{
						TasksAllCompleted = 1;
					}
					else
					{
						TasksAllCompleted = 0;
					}

					Thread.Sleep(10);
				}
			}).Start();
		}

		#endregion

		#region Field

		/// <summary>
		/// 是否全部完成
		/// </summary>
		private int tasksAllCompleted;

		/// <summary>
		/// 是否统一完成
		/// </summary>
		private int tasksCompletedByAll;

		/// <summary>
		/// 任务集合
		/// </summary>
		private ObservableCollection<JTask> tasks;

		#endregion

		#region Property

		/// <summary>
		/// 是否全部完成
		/// </summary>
		public int TasksAllCompleted
		{
			get { return tasksAllCompleted; }
			set
			{
				tasksAllCompleted = value;
				OnPropertyChanged("TasksAllCompleted");
			}
		}

		/// <summary>
		/// 是否统一完成
		/// </summary>
		public int TasksCompletedByAll
		{
			get { return tasksCompletedByAll; }
			set
			{
				tasksCompletedByAll = value;
				OnPropertyChanged("TasksCompletedByAll");
			}
		}

		/// <summary>
		/// 任务集合
		/// </summary>
		public ObservableCollection<JTask> Tasks
		{
			get { return tasks; }
			set
			{
				tasks = value;
				OnPropertyChanged("Tasks");
			}
		}

		/// <summary>
		/// 新建任务
		/// </summary>
		public DelegateCommand<object> NewTaskCommand
		{
			get
			{
				return new DelegateCommand<object>((obj) =>
				{
					JTask task = new JTask();
					task.TaskId = Tasks.Count;
					task.TaskProgress = 0;
					task.TaskProgressColorIndex = task.TaskId % 5;
					Tasks.Add(task);
				}, (obj) => true);
			}
		}

		/// <summary>
		/// 统一执行
		/// </summary>
		public DelegateCommand<object> StartAllTaskCommand
		{
			get
			{
				return new DelegateCommand<object>((obj) =>
				{
					if (ValidateBeforeStart())
					{
						TasksCompletedByAll = 0;
						StartAllTaskByNormal();
					}

				}, (obj) => true);
			}
		}

		/// <summary>
		/// 统一执行
		/// </summary>
		public DelegateCommand<object> StartAllTaskBySyncCommand
		{
			get
			{
				return new DelegateCommand<object>((obj) =>
				{
					if (ValidateBeforeStart())
					{
						TasksCompletedByAll = 0;
						StartAllTaskByAsync();
					}
				}, (obj) => true);
			}
		}

		/// <summary>
		/// 并行运行
		/// </summary>
		public DelegateCommand<object> StartAllTaskParallelCommand
		{
			get
			{
				return new DelegateCommand<object>((obj) =>
				{
					if (ValidateBeforeStart())
					{
						TasksCompletedByAll = 0;
						StartAllTaskByParallel();
					}
				}, (obj) => true);
			}
		}

		/// <summary>
		/// 并行运行
		/// </summary>
		public DelegateCommand<object> StartAllTaskByThreadCommand
		{
			get
			{
				return new DelegateCommand<object>((obj) =>
				{
					if (ValidateBeforeStart())
					{
						TasksCompletedByAll = 0;
						StartAllTaskByThread();
					}
				}, (obj) => true);
			}
		}

		#endregion

		#region Private

		/// <summary>
		/// 运行前验证
		/// </summary>
		/// <returns></returns>
		private bool ValidateBeforeStart()
		{
			if (Tasks.Count == 0)
			{
				MessageBox.Show("无可运行线程，请先添加", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
				return false;
			}

			for (int i = 0; i < Tasks.Count; i++)
			{
				if (Tasks[i].Task != null && Tasks[i].Task.Status == TaskStatus.Running)
				{
					MessageBox.Show("线程正在运行中，不可重复开始", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 正常启动所有 Task
		/// </summary>
		private void StartAllTaskByNormal()
		{
			Parallel.For(0, Tasks.Count, (i) => {
				Tasks[i].StartCommand.Execute(null);
			});
			new Task(() =>
			{
				Task[] ts = new Task[Tasks.Count];
				for (int i = 0; i < Tasks.Count; i++)
				{
					ts[i] = Tasks[i].Task;
				}
				Task.WaitAll(ts);
				TasksCompletedByAll = 1;
			}).Start();
		}

		/// <summary>
		/// 使用 async+await 启动所有 Task
		/// </summary>
		private async void StartAllTaskByAsync()
		{
			await Task.Run(() => {
				Parallel.For(0, Tasks.Count, (i) => {
					Tasks[i].StartCommand.Execute(null);
				});
				Task[] ts = new Task[Tasks.Count];
				for (int i = 0; i < Tasks.Count; i++)
				{
					ts[i] = Tasks[i].Task;
				}
				Task.WaitAll(ts);
				TasksCompletedByAll = 1;
			});
		}

		/// <summary>
		/// 使用 async+await + 并行启动所有 Task
		/// </summary>
		private async void StartAllTaskByParallel()
		{
			Parallel.For(0, Tasks.Count, (i) => {
				Tasks[i].StartCommand.Execute(null);
			});

			Task[] ts = new Task[Tasks.Count];
			for (int i = 0; i < Tasks.Count; i++)
			{
				ts[i] = Tasks[i].Task;
			}
			await Task.WhenAll(ts);
			//Task.WaitAll(ts);
			TasksCompletedByAll = 1;
		}

		/// <summary>
		/// 使用 Thread 并行启动所有 Task
		/// </summary>
		private void StartAllTaskByThread()
		{
			Parallel.For(0, Tasks.Count, (i) => {
				Tasks[i].StartByThreadCommand.Execute(null);
			});

			Task.Run(()=> {
				var waits = new List<EventWaitHandle>();
				for (int i = 0; i < Tasks.Count; i++)
				{
					waits.Add(Tasks[i].ManualResetEvent);
				}
				WaitHandle.WaitAll(waits.ToArray());
				TasksCompletedByAll = 1;
			});
		}

		#endregion
	}
}
