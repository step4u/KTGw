using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Com.Huen.DataModel
{
    public class EmployeeListViewModel
    {
        #region Data

        readonly ObservableCollection<DepartmentViewModel> _firstGeneration;
        readonly DepartmentViewModel _rootDepartment;
        readonly ICommand _searchCommand;

        IEnumerator<DepartmentViewModel> _matchingPeopleEnumerator;
        string _searchText = String.Empty;

        #endregion // Data

        #region Constructor

        public DepartmentTreeViewModel(Department rootDepartment)
        {
            _rootDepartment = new DepartmentViewModel(rootDepartment);

            _firstGeneration = new ObservableCollection<DepartmentViewModel>(
                new DepartmentViewModel[] 
                { 
                    _rootDepartment
                });

            _firstGeneration.CollectionChanged += _firstGeneration_CollectionChanged;

            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        void _firstGeneration_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var x in e.NewItems)
            {
                // do something
            }

            foreach (var y in e.OldItems)
            {
                //do something
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //do something
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //do something
            }
        }

        #endregion // Constructor

        #region Properties

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ObservableCollection<DepartmentViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        #endregion // FirstGeneration

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly DepartmentTreeViewModel _familyTree;

            public SearchFamilyTreeCommand(DepartmentTreeViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingPeopleEnumerator = null;
            }
        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            var matches = this.FindMatches(_searchText, _rootDepartment);
            _matchingPeopleEnumerator = matches.GetEnumerator();

            if (!_matchingPeopleEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<DepartmentViewModel> FindMatches(string searchText, DepartmentViewModel person)
        {
            if (person.NameContainsText(searchText))
                yield return person;

            foreach (DepartmentViewModel child in person.Children)
                foreach (DepartmentViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }
}
