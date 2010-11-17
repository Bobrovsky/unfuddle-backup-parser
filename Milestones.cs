using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace UnfuddleBackupParser
{
    class Milestones
    {
        struct Milestone
        {
            public string id;
            public string name;
            public string dueOn;
        }

        SortedDictionary<int, Milestone> m_milestones;

        public Milestones(XmlElement element)
        {
            m_milestones= new SortedDictionary<int, Milestone>();
            parse(element);
        }

        public string GetMilestoneName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            int nameId = Convert.ToInt32(id);
            Milestone m = m_milestones[nameId];
            return m.name;
        }

        public void AddAllMilestones(StringBuilder sb)
        {
            addMilestone(sb, "id", "name", "due-on");

            foreach (KeyValuePair<int, Milestone> pair in m_milestones)
            {
                Milestone milestone = pair.Value;
                addMilestone(sb, milestone.id, milestone.name, milestone.dueOn);
            }
        }

        private static void addMilestone(StringBuilder sb, string id, string name, string dueOn)
        {
            sb.Append(id);
            sb.Append(',');
            sb.Append(name);
            sb.Append(',');
            sb.Append(dueOn);
            sb.Append('\n');
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Milestone milestone = new Milestone();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string value = HttpUtility.HtmlDecode(child.InnerText);
                    switch (child.Name)
                    {
                        case "id":
                            milestone.id = value;
                            break;

                        case "title":
                            milestone.name = value;
                            break;

                        case "due-on":
                            milestone.dueOn = value;
                            break;
                    }
                }

                m_milestones.Add(Convert.ToInt32(milestone.id), milestone);
            }
        }
    }
}
