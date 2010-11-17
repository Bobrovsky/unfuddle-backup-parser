using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace UnfuddleBackupParser
{
    class Components
    {
        struct Component
        {
            public string id;
            public string name;
        }

        SortedDictionary<int, Component> m_components;

        public Components(XmlElement element)
        {
            m_components= new SortedDictionary<int, Component>();
            parse(element);
        }

        public void AddAllAreas(StringBuilder sb)
        {
            addArea(sb, "id", "name");

            foreach (KeyValuePair<int, Component> pair in m_components)
            {
                Component component = pair.Value;
                addArea(sb, component.id, component.name);
            }
        }

        public string GetComponentName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            int nameId = Convert.ToInt32(id);
            Component c = m_components[nameId];
            return c.name;
        }

        private static void addArea(StringBuilder sb, string id, string name)
        {
            sb.Append(id);
            sb.Append(',');
            sb.Append(name);
            sb.Append('\n');
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Component component = new Component();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string value = HttpUtility.HtmlDecode(child.InnerText);
                    switch (child.Name)
                    {
                        case "id":
                            component.id = value;
                            break;

                        case "name":
                            component.name = value;
                            break;
                    }
                }

                m_components.Add(Convert.ToInt32(component.id), component);
            }
        }
    }
}
