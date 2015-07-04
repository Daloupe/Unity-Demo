/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.CoreProjectCode.Settings.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class XmlDocumentSettingsHelpers
    {
        public static IEnumerable<KeyValuePair<string, object>> ReadSettings(string file, bool filterDuplicates)
        {
            var xml = new XmlDocument();
            xml.Load(file);

            if (xml.DocumentElement == null || xml.DocumentElement.Name != "settings")
            {
                throw new FileLoadException("Settings file root node is not \"settings\"!");
            }

            var results = from x in xml.DocumentElement.ChildNodes.OfType<XmlNode>()
                          where x.Name == "entry" && x.Attributes != null && x.Attributes.Count > 0 
                          let key = x.Attributes["key"]
                          where key != null && !String.IsNullOrEmpty(key.Value)
                          select new KeyValuePair<string, object>(key.InnerText, x.InnerText);

            if (filterDuplicates)
            {
                var list = new List<KeyValuePair<string, object>>();
                foreach (var pair in results)
                {
                    if (list.All(x => x.Key != pair.Key))
                    {
                        list.Add(pair);
                    }
                }

                return list;
            }

            return results;
        }
    }
}
