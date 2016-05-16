using System;
using System.Linq;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Com.Huen.DataModel
{
    public class DepartmentViewModel : INotifyPropertyChanged
    {
        #region Data

        public ObservableCollection<DepartmentViewModel> _children;
        readonly DepartmentViewModel _parent;
        readonly Department _department;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public DepartmentViewModel(Department department)
            : this(department, null)
        {
        }

        public DepartmentViewModel(Department department, DepartmentViewModel parent)
        {
            _department = department;
            _parent = parent;

            _children = new ObservableCollection<DepartmentViewModel>(
                    (from child in _department.Children
                     select new DepartmentViewModel(child, this))
                     .ToList<DepartmentViewModel>());
        }

        #endregion // Constructors

        #region Person Properties

        public ObservableCollection<DepartmentViewModel> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public string Name
        {
            get { return _department.Name; }
        }

        public string Idx
        {
            get { return _department.Idx; }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public DepartmentViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members        

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
