using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VbotFramework.Delagate
{
	public class DelegateCommand : ICommand
	{
		#region Fields

		readonly Action action = null;
		readonly Func<bool> canExecutePredicate = null;

		#endregion Fields

		#region Constructors

		public DelegateCommand(Action action)
			: this(action, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="action">The execution logic.</param>
		/// <param name="canExecutePredicate">The execution status logic.</param>
		public DelegateCommand(Action action, Func<bool> canExecutePredicate)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			this.action = action;
			this.canExecutePredicate = canExecutePredicate;
		}

		#endregion // Constructors

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return canExecutePredicate == null ? true : canExecutePredicate();
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			action();
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
		#endregion
	}
	public class DelegateCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> action = null;
		readonly Predicate<T> canExecutePredicate = null;

		#endregion Fields

		#region Constructors

		public DelegateCommand(Action<T> action)
			: this(action, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="action">The execution logic.</param>
		/// <param name="canExecutePredicate">The execution status logic.</param>
		public DelegateCommand(Action<T> action, Predicate<T> canExecutePredicate)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			this.action = action;
			this.canExecutePredicate = canExecutePredicate;
		}

		#endregion // Constructors

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return canExecutePredicate == null ? true : canExecutePredicate((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			object parameterCopy = parameter;

			if (((parameter != null) && (parameter.GetType() != typeof(T))) && (parameter is IConvertible))
			{
				parameterCopy = Convert.ChangeType(parameter, typeof(T), null);
			}

			if (this.CanExecute(parameterCopy))
			{
				if (parameterCopy == null)
				{
					if (typeof(T).IsValueType)
					{
						action(default(T));
					}
					else
					{
						action((T)parameterCopy);
					}
				}
				else
				{
					action((T)parameterCopy);
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		#endregion
	}
}
