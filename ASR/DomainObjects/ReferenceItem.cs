using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
	
	public class ReferenceItem : BaseItem
	{
        private readonly string _currentuser;
	    public ReferenceItem(Item i) : base(i)
	    {
	        _currentuser = Sitecore.Context.User.Name;
            Assembly = i["assembly"];
            Class = i["class"];
            Attributes = i["attributes"];
            PrettyName =  string.Format("{0} ({1})", Name,i.TemplateName); 
	    }


        #region ItemFields
        public string Assembly
        {
            get;
            private set;
        }

        public string Class
        {
            get;
            private set;
        }

        public string Attributes
        {
            get;
            private set;
        } 
        #endregion

        public string ReplacedAttributes
		{
			get
			{
				string replacedAttributes = Parameters.Aggregate(Attributes, (current, pi) => current.Replace(pi.Token, pi.Value));

			    if (replacedAttributes.Contains("$"))
                {
                    replacedAttributes = Util.MakeDateReplacements(replacedAttributes);
                    replacedAttributes = replacedAttributes.Replace("$sc_currentuser", _currentuser); 
                }
			    return replacedAttributes;
			}
		}
		public string FullType
		{
			get
			{
				if (!string.IsNullOrEmpty(Assembly))
				{
					return string.Concat(Class, ", ", Assembly);
				}
				return Class;
			}
		}

		public void SetAttributeValue(string tag, string value)
		{
			try
			{

				ParameterItem pi = Parameters.First(p => p.Name == tag);

				if (pi != null)
				{
					pi.Value = System.Uri.UnescapeDataString(value);
				}
			}
			// can't find element
			catch (InvalidOperationException)
			{
				// do nothing;
			}
		}

		public bool HasParameters
		{
			get
			{
				if (_parameters == null)
				{
					MakeParameterSet();
				}

				return _parameters.Count > 0;
			}
		}

		private HashSet<ParameterItem> _parameters = null;
		public IEnumerable<ParameterItem> Parameters
		{
			get
			{
				if (_parameters == null || _parameters.Count == 0)
				{
					MakeParameterSet();
				}
				return _parameters;
			}
		}

        public string PrettyName
        {
            get;
            private set;
        }

	    private void MakeParameterSet()
		{
			_parameters = new HashSet<ParameterItem>();
            var r = new Regex(Settings.Instance.ParameterRegex);
            var matches = r.Matches(Attributes);
			foreach (Match match in matches)
			{
			    var tag = match.Groups[1].Value;
				var pi = GetParameter(tag);

			    if (pi == null) continue;
			    
                pi.Token = match.Groups[0].Value;
			    _parameters.Add(pi);
			}
		}

		private ParameterItem GetParameter(string name)
		{
			string path = string.Concat(Settings.Instance.ParametersFolder, "/", name);
            
			return new ParameterItem(this.Database.GetItem(path));
		}
	
	}
}
