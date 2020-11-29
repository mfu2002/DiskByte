using DiskByte.ViewModel.Analyser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DiskByte.Helper
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension to the IEnumerable interface to allow for iteration over each object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                return;
            }

            foreach (var cur in enumerable)
            {
                action(cur);
            }
        }

        /// <summary>
        /// Sorts IAnalyseGridItem into order. 
        /// </summary>
        /// <param name="sortColumn">Property that the collection needs to be sorted by.</param>
        /// <param name="sortOrder">Ascending or Descending.</param>
        /// <param name="sortList">List that is being sorted. </param>
        /// <returns>Ordered Enumerable of the sorted list. </returns>
        public static IOrderedEnumerable<IAnalyseGridItem> GetSortedList(this IEnumerable<IAnalyseGridItem> sortList, GridTreeSortColumn sortColumn, ListSortDirection sortOrder)
        {
            switch (sortColumn)
            {
                case GridTreeSortColumn.Name:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.Name);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.Name);
                    }
                case GridTreeSortColumn.Size:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.Size);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.Size);
                    }
                case GridTreeSortColumn.LastChanged:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.LastChanged);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.LastChanged);
                    }
                case GridTreeSortColumn.FileCount:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.FileCount);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.FileCount);
                    }
                case GridTreeSortColumn.DirectoryCount:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.FolderCount);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.FolderCount);
                    }
                case GridTreeSortColumn.TotalCount:
                    if (sortOrder == ListSortDirection.Ascending)
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.TotalCount);
                    }
                    else
                    {
                        return sortList.OrderBy(x => x.GetType().Name).ThenByDescending(x => x.TotalCount);
                    }
            }
            return sortList.OrderBy(x => x.GetType().Name).ThenBy(x => x.Name);

        }
    }
}