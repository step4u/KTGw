using System;
using System.Linq;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Com.Huen.DataModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        #region Data

        readonly Employee _employee;

        #endregion // Data


        #region Constructors

        public EmployeeViewModel(Employee employee)
        {
            _employee = employee;
        }

        #endregion // Constructors

        #region Employee Properties

        public string Emp_Idx
        {
            get { return _employee.Emp_Idx; }
        }

        public string Emp_Name
        {
            get { return _employee.Emp_Name; }
        }

        public string Part_Name
        {
            get { return _employee.Part_Name; }
        }

        public string Duty_Name
        {
            get { return _employee.Duty_Name; }
        }

        public string Emp_Tel
        {
            get { return _employee.Emp_Tel; }
        }

        public string Emp_Memo
        {
            get { return _employee.Emp_Memo; }
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
