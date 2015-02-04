using System.Collections.Generic;
using ASR.Interface;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Specialized;
using Sitecore;
using ASR.DomainObjects;

namespace ASR.Interface
{
	public class Report
	{
		private Dictionary<string, FilterItem> filters;
		private Dictionary<string, ScannerItem> scanners;
		private Dictionary<string, ViewerItem> viewers;

		// buffer for the result of the scanner
		private ArrayList scannerResults;

		// buffer for filtered results i.e. scannerResults => Filter()
		private ArrayList results;

        //buffer for displayelements
        

		IEnumerable<BaseViewer> _viewers;
		public IEnumerable<BaseViewer> Viewers
		{
			get
			{
				if (_viewers == null)
				{
					_viewers = viewers.Select(v => BaseViewer.Create(v.Value.FullType, v.Value.ReplacedAttributes, v.Value.ColumnsXml)).ToList();
				}
				return _viewers;
			}
		}

        private DisplayElement[] displayelements;
		public IEnumerable<DisplayElement> DisplayElements (int start = 0, int count = int.MaxValue)
		{

            count = Math.Min(count, results.Count - start);
            if (displayelements == null)
            {
                var tmp = results
                    .OfType<object>()
                    .DefaultIfEmpty()
                    .Select(IntializeDisplayElement);
                tmp = Sort(tmp);
                displayelements = tmp.ToArray();
            }
            return displayelements.Skip(start).Take(count);
		}

        public void FlushDisplayElements()
        {
            displayelements = null;
        }


		private IEnumerable<DisplayElement> Sort(IEnumerable<DisplayElement> tmp)
		{
            // if no columns to sort by, then just return the tmp variable
            if (SortColumns == null || SortColumns.Count == 0)
            {
                return tmp;
            }

            IOrderedEnumerable<DisplayElement> sortedList = null;

            bool isFirstTimeThrough = true;

            foreach (string columnName in SortColumns.Keys)
            {
                Func<DisplayElement, object> columnValue = null;
                var sortOptions = SortColumns[columnName];
                // need to copy the column name, or it executes against the wrong column when we actually do the sort
                string copyOfColumnName = columnName;

                //// sort datetime values separately
                //if (sortOptions.Contains("DateTime"))
                //{
                //    columnValue = t => ParseDate(t.GetColumnValue(copyOfColumnName));
                //}
                //else
                //{
                   columnValue = t => t.GetColumnSortingValue(copyOfColumnName);
                //}

                // and sort based on order
                if (sortOptions.IsAscending())
                {
                    sortedList = (isFirstTimeThrough) ? tmp.OrderBy(columnValue):sortedList.ThenBy(columnValue);
                    isFirstTimeThrough = false;
                }
                else if (sortOptions.IsDescending())
                {
                    sortedList = (isFirstTimeThrough) ? tmp.OrderByDescending(columnValue):sortedList.ThenByDescending(columnValue);
                    isFirstTimeThrough = false;
                }
            }
            return sortedList;

		}

		private object ParseDate(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (DateUtil.IsIsoDate(value))
				{
					return DateUtil.IsoDateToDateTime(value);
				}
				else
				{
					DateTime result;
					DateTime.TryParse(value, out result);
					return result;
				}
			}
			return null;
		}

		private DisplayElement IntializeDisplayElement(object resultItem)
		{
			var dElement = new DisplayElement(resultItem);
			foreach (var oViewer in Viewers)
			{
				oViewer.Display(dElement);
			}
			return dElement;
		}

		public Report()
		{
			this.scanners = new Dictionary<string, ScannerItem>();
			this.viewers = new Dictionary<string, ViewerItem>();
			this.filters = new Dictionary<string, FilterItem>();
		}

		/// <summary>
		/// Add a filter to the report
		/// </summary>
		/// <param name="name">Name of the filter</param>
		/// <param name="type">Type of the class in the usual format: Fully Qualified Class Name, Assembly</param>
		/// <param name="parameters">Parameters as pipe separated: key=value|key2=value2</param>
		public void AddFilter(FilterItem filter)
		{
			if (!filters.ContainsKey(filter.ID.ToString()))
			{
				filters.Add(filter.ID.ToString(), filter);
				FlushFilterCache();
			}
		}

		/// <summary>
		/// Add a Scanner to the report
		/// </summary>
		/// <param name="name">Name of the scanner</param>
		/// <param name="type">Type of the class in the usual format: Fully Qualified Class Name, Assembly</param>
		/// <param name="parameters">Parameters as pipe separated: key=value|key2=value2</param>
		public void AddScanner(ScannerItem scanner)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(scanner, "scanner");

			if (!scanners.ContainsKey(scanner.ID.ToString()))
			{
				scanners.Add(scanner.ID.ToString(), scanner);
				FlushCache();
			}
		}

		/// <summary>
		/// Add a viewer to the report
		/// </summary>
		/// <param name="name">Name of the viewer</param>
		/// <param name="type">Type of the class in the usual format: Fully Qualified Class Name, Assembly</param>
		/// <param name="parameters">Parameters as pipe separated: key=value|key2=value2</param>
		public void AddViewer(ViewerItem viewer)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(viewer, "viewer");

			if (!viewers.ContainsKey(viewer.ID.ToString()))
			{
				viewers.Add(viewer.ID.ToString(), viewer);
			}
		}

		/// <summary>
		/// flush the buffer for the scanner, so next time the report runs, scanner will be queried again.
		/// </summary>
		public void FlushCache()
		{
			scannerResults = null;
		}

		/// <summary>
		/// Flush the cache for the filtered elements, next time the filters will be called again.
		/// </summary>
		public void FlushFilterCache()
		{
			results = null;
            displayelements = null;
		}

		/// <summary>
		/// Run the report. It will only requery the Scanner if FlushCache has been used before calling this method.
		/// </summary>
		/// <param name="parameters">Not used. Only so it can be called as a ProgressBoxMethod delegate</param>
		public void Run(params object[] parameters)
		{
			if (scannerResults == null)
			{
				scannerResults = new ArrayList();
				foreach (var scanner in scanners)
				{
					BaseScanner oScanner = BaseScanner.Create(scanner.Value.FullType, scanner.Value.ReplacedAttributes);
					//TODO throw exception if null??
					scannerResults.AddRange(oScanner.Scan());
				}
			}
			Filter();			
		}

		/// <summary>
		/// Filters the results. All filters are "and" so an item needs to pass all filters to appear on the report.
		/// </summary>
		public void Filter()
		{
			IEnumerable<BaseFilter> oFilters = filters.Select(p => BaseFilter.Create(p.Value.FullType, p.Value.ReplacedAttributes)).ToList();
			results = new ArrayList();

			foreach (var element in scannerResults)
			{
				bool add = true;
				foreach (var filter in oFilters)
				{
					if (!filter.Filter(element))
					{
						add = false;
						break;
					}
				}
				if (add)
				{
					Sitecore.Diagnostics.Assert.IsNotNull(element, "element is null ");
					results.Add(element);
				}
			}
		}

		/// <summary>
		/// Gets the results in this report. Will call Run() if it hasn't been called yet.
		/// </summary>
		/// <returns>All the results from running the report</returns>
		public IEnumerable<DisplayElement> GetResultElements()
		{
			if (results == null)
				Run();
			return GetResultElements(0, results.Count);
		}

		/// <summary>
		/// Gets only a selection of the results elements. Useful for paging.
		/// </summary>
		/// <param name="start">first result to return</param>
		/// <param name="count">number of results to return</param>
		/// <returns>Results of the report</returns>
		public IEnumerable<DisplayElement> GetResultElements(int start, int count)
		{
			return DisplayElements(start,count);
		}

		/// <summary>
		/// Total number of results.
		/// </summary>
		/// <returns>Number of results or -1 if report has not been run yet.</returns>
		public int ResultsCount()
		{
			if (results == null)
				return -1;
			return results.Count;
		}

       
        public void SetOrToggleSortColumn(string columnName)
        {
            if (SortColumns.ContainsKey(columnName))
            {
                SortColumns[columnName] = SortColumns[columnName].Toggle();
            }
            else
            {
                SortColumns.Clear();
                SortColumns.Add(columnName, SortDirection.Ascending());
            }
        }

	  
		private Dictionary<string,SortDirection> _sortColumns;
        protected Dictionary<string, SortDirection> SortColumns
		{
			get
			{
				if (_sortColumns == null)
				{
                    _sortColumns = new Dictionary<string, SortDirection>();
					foreach (var viewer in viewers.Values)
					{
						NameValueCollection parameters = Sitecore.StringUtil.ParseNameValueCollection(viewer.ReplacedAttributes, '|', '=');
						string sortParameter = parameters["sort"];
						if (sortParameter != null)
						{
							var columnsortingdata =  StringUtil.ParseNameValueCollection(sortParameter, '&', ',') ;
                          
                            foreach (var key in columnsortingdata.AllKeys)
                            {
                               
                                _sortColumns.Add(key, SortDirection.Parse(columnsortingdata[key]));
                            }
						}
					}
				}
				return _sortColumns;
			}
		}
	}
    public struct SortDirection
    {
        private int direction;

        public SortDirection Toggle()
        {
            return new SortDirection() { direction = (this.direction + 1) % 2 };
        }

        public static SortDirection Ascending()
        {
            return new SortDirection() { direction = 1 };
        }

        public static SortDirection Descending()
        {
            return new SortDirection() { direction = 0 };
        }

        public bool IsDescending()
        {
            return direction == 0;
        }

        public bool IsAscending()
        {
            return direction == 1;
        }
        public override string ToString()
        {
            if (IsAscending()) return "ASC";
            return "DESC";
        }
        public static SortDirection Parse(string st)
        {
            if (st.Equals("desc", StringComparison.InvariantCultureIgnoreCase)) return SortDirection.Descending();
            return SortDirection.Ascending();
        }
    }
}
