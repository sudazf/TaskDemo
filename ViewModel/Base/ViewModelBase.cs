using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDemo.ViewModel.Base
{
	[Serializable]
	public class ViewModelBase: INotifyPropertyChanged
	{
		[field: NonSerializedAttribute()]
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (null != PropertyChanged)
			{
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
